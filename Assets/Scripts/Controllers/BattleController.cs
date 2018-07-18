using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyUnityEventDispatcher;
using System.Collections;
using TinyTeam.UI;

public sealed partial class BattleController : SingletonMonobehavior<BattleController>, IEndBattleEvent
{
    public Transform PlayerTransform;
    public Transform PlayerPokemonTransform;
    public Transform EnemyPokemonTransform;

    private List<BattlePokemonData> wildPokemons = new List<BattlePokemonData>();
    public List<BattlePokemonData> playPokemons = new List<BattlePokemonData>();
    //private List<GameObject> BattlePokemonsGameObejcts = new List<GameObject>();
    public BattlePokemonData PlayerCurPokemonData { get; private set; }
    public BattlePokemonData EnemyCurPokemonData { get; private set; }

    private GameContext context;
    private BattleState battleState;

    private int PlayChooseSkillID = -1;
    private int EnemyChooseSkillID = -1;

    public readonly float BattleTime = 3f;
    public readonly float BattleInterval = 1f;
    private string PlayerChooseBagItemName = "", EnemyChooseBagItemName = "";

    private void Start()
    {
        context = Contexts.sharedInstance.game;
        NotificationCenter<int>.Get().AddEventListener("UseSkill", PlayerPokemonUseSkill);
        NotificationCenter<int>.Get().AddEventListener("PokemonDeathMessage", PokemonDeathEvent);
        NotificationCenter<string>.Get().AddEventListener("UseBagItem", PlayerUseBagItem);
        NotificationCenter<int>.Get().AddEventListener("CatchPokemon", CatchPokemonResultEvent);
        NotificationCenter<int>.Get().AddEventListener("ExchangePokemon", ExchangePokemon);
        EndBattleSystem.EndBattleEvent += EndBattleEvent;

        BattleStateForPlayer.InitEvent += PlayerRound;
        BattleStateForBattle.InitEvent += BattleRound;
    }
    public bool CanBattle
    {
        get
        {
            return null != PlayerCurPokemonData && null != EnemyCurPokemonData;
        }
    }
    public void InitWildPokemon(params Pokemon[] pokemons)
    {
        wildPokemons = new List<Pokemon>(pokemons)
            .Select(x=>BattlePokemonData.Context[x.GetInstanceID()])
            .ToList();           
        if (0 == pokemons.Length)
        {
            Debug.LogWarning("没有野生精灵怎么打");
            Contexts.sharedInstance.game.isBattleFlag = false;
            return;
        }

        EnemyCurPokemonData = wildPokemons[0];
    }
    public void InitPlayerPokemons(List<Pokemon> pokemons)
    {
       
        playPokemons = pokemons
            .Select(x => BattlePokemonData.Context[x.GetInstanceID()])
            .ToList();
        if (0 == pokemons.Count)
        {
            Debug.LogError("没有精灵怎么打");
            Contexts.sharedInstance.game.isBattleFlag = false;
            return;
        }
        PlayerCurPokemonData = playPokemons[0];
    }

    public void BeginBattle()
    {
        //召唤精灵
        GameObject playerPokemon = PokemonFactory.InitPokemon(PlayerCurPokemonData.race.raceid);
        playerPokemon.transform.position = PlayerPokemonTransform.position ;
        playerPokemon.transform.parent = PlayerPokemonTransform;
        PokemonFactory.PokemonBallEffect(playerPokemon.transform.position);
        GameObject enemyPokemon = PokemonFactory.InitPokemon(EnemyCurPokemonData.race.raceid);
        enemyPokemon.transform.position = EnemyPokemonTransform.position ;
        enemyPokemon.transform.parent = EnemyPokemonTransform;
        PokemonFactory.PokemonBallEffect(enemyPokemon.transform.position);
        PlayerCurPokemonData.transform = playerPokemon.transform;
        EnemyCurPokemonData.transform = enemyPokemon.transform;

        //控制精灵和训练家朝向
        playerPokemon.transform.LookAt(enemyPokemon.transform);
        enemyPokemon.transform.LookAt(playerPokemon.transform);
        PlayerController.Instance.transform.LookAt(enemyPokemon.transform);
        Quaternion quaternion = PlayerController.Instance.transform.rotation;
        quaternion.x = 0;
        quaternion.z = 0;
        PlayerController.Instance.transform.rotation = quaternion;

        //BattlePokemonsGameObejcts.Add(playerPokemon);
        //BattlePokemonsGameObejcts.Add(enemyPokemon);

        //初始化精灵数据
        foreach(BattlePokemonData pokemon in playPokemons)
        {
            pokemon.Recover();
        }
        foreach (BattlePokemonData pokemon in wildPokemons)
        {
            pokemon.Recover();
        }
       
        battleState = new BattleStateForPlayer();
        
    }
    public void EndBattleEvent()
    {

        //初始化精灵数据
        foreach (BattlePokemonData pokemon in playPokemons)
        {
            if(pokemon.transform!= null)
            {
                pokemon.transform.gameObject.SetActive(false);
                ObjectPoolController.PokemonObjectsPool[pokemon.race.raceid] =
                pokemon.transform.gameObject;
            }
           

            GameEntity entity = context.GetEntityWithBattlePokemonData(pokemon);
            Action action = entity.pokemonDataChangeEvent.Event;
            action = () => { };
            entity.ReplacePokemonDataChangeEvent(action);
            pokemon.Recover();
        }
        foreach (BattlePokemonData pokemon in wildPokemons)
        {
            pokemon.transform.gameObject.SetActive(false);
            ObjectPoolController.PokemonObjectsPool[pokemon.race.raceid] =
            pokemon.transform.gameObject;

            GameEntity entity = context.GetEntityWithBattlePokemonData(pokemon);
            Action action = entity.pokemonDataChangeEvent.Event;
            action = () => { };
            entity.ReplacePokemonDataChangeEvent(action);
            pokemon.Recover();
        }
        wildPokemons = new List<BattlePokemonData>();
        playPokemons = new List<BattlePokemonData>();
        PlayerCurPokemonData = null;
        EnemyCurPokemonData = null;
        battleState = null;
    }

    /// <summary>
    /// 我方精灵使用技能
    /// </summary>
    /// <param name="notific"></param>
    private void PlayerPokemonUseSkill(Notification<int> notific)
    {
        int skillID = PlayerCurPokemonData.skills[notific.param];
        int pp = PlayerCurPokemonData.skillPPs[notific.param];
        if (pp <= 0)
        {
            NotificationCenter<int>.Get().DispatchEvent("DisableSkillButton", notific.param);
            return;
        }
        PlayChooseSkillID = skillID;
        UpdateBattleState();
        

       
    }
    private void PlayerUseBagItem(Notification<string> notific)
    {
        PlayerChooseBagItemName = notific.param;
        UpdateBattleState(); 
    }

    private void CatchPokemonResultEvent(Notification<int> notific)
    {
        
        StartCoroutine(WaitBattleEnd());
    }

    private void ExchangePokemon(Notification<int> notific)
    {
        BattlePokemonData newCallPokemon = playPokemons[notific.param];
        ExchangePokemon(newCallPokemon);
        UpdateBattleState();
    }

    private void ExchangePokemon(BattlePokemonData newCallPokemon)
    {
        //回收精灵GameObject
        PlayerCurPokemonData.transform.gameObject.SetActive(false);
        ObjectPoolController.PokemonObjectsPool[PlayerCurPokemonData.race.raceid] =
        PlayerCurPokemonData.transform.gameObject;

        PlayerCurPokemonData = newCallPokemon;
        PlayChooseSkillID = -1;

        //召唤新的精灵
        GameObject playerPokemon = PokemonFactory.InitPokemon(PlayerCurPokemonData.race.raceid);
        playerPokemon.transform.position = PlayerPokemonTransform.position;
        playerPokemon.transform.parent = PlayerPokemonTransform;
        PokemonFactory.PokemonBallEffect(playerPokemon.transform.position);
        PlayerCurPokemonData.transform = playerPokemon.transform;

        //控制精灵和训练家朝向
        playerPokemon.transform.LookAt(EnemyCurPokemonData.transform);
    }
    /// <summary>
    /// 更新战斗状态
    /// </summary>
    private void UpdateBattleState()
    {
       
        battleState = battleState.ChangeState();
    }

    
    /// <summary>
    /// 玩家回合
    /// </summary>
    private void PlayerRound()
    {

        EnemyAction();



    }

    /// <summary>
    /// 对战回合
    /// </summary>
    private void BattleRound()
    {

        if (!CanBattle) return;
        //互相攻击
        if(PlayerCurPokemonData.Speed>EnemyCurPokemonData.Speed)
        {
            PlayerBattleAround();
            StartCoroutine(WaitEnemySkillUse());

        }
        else
        {
            EnemyBattleAround();
            StartCoroutine(WaitPlayerSkillUse());

        }
        StartCoroutine(WaitBattleAroundEnd());


    }
    /// <summary>
    /// 更新玩家和精灵数据
    /// </summary>
    private void UpdatePokemonDatas()
    {
        foreach(var data in wildPokemons)
        {
            var entity = context.GetEntityWithBattlePokemonData(data);
            entity.ReplaceBattlePokemonData(data);
        }
        foreach (var data in playPokemons)
        {
            var entity = context.GetEntityWithBattlePokemonData(data);
            entity.ReplaceBattlePokemonData(data);
        }
        context.ReplacePlayerData(context.playerData.scriptableObject);
    }

    private void PlayerBattleAround()
    {
        if (CanBattle)
        {
            if(ResourceController.Instance.allSkillDic.ContainsKey(PlayChooseSkillID))
            {
                Skill PlayerSkill = ResourceController.Instance.allSkillDic[PlayChooseSkillID];
                if (null != PlayerSkill)
                    UseSkill.Attack(PlayerSkill, PlayerCurPokemonData, EnemyCurPokemonData);
            }
            
            if(ResourceController.Instance.UseBagUItemDict.ContainsKey(PlayerChooseBagItemName))
            {
                UseBagItem use = ResourceController.Instance.UseBagUItemDict[PlayerChooseBagItemName];
                if(null != use)
                {
                    use.Effect();
                }
            }
        }
    }

    private void EnemyBattleAround()
    {
        if (CanBattle)
        {
            if (ResourceController.Instance.allSkillDic.ContainsKey(EnemyChooseSkillID))
            {
                Skill EnemySkill = ResourceController.Instance.allSkillDic[EnemyChooseSkillID];
                if (null != EnemySkill)
                    UseSkill.Attack(EnemySkill, EnemyCurPokemonData, PlayerCurPokemonData);
            }

            if (ResourceController.Instance.UseBagUItemDict.ContainsKey(EnemyChooseBagItemName))
            {
                UseBagItem use = ResourceController.Instance.UseBagUItemDict[EnemyChooseBagItemName];
                if (null != use)
                {
                    use.Effect();
                }
            }
        }

    }

    IEnumerator WaitEnemySkillUse()
    {
        yield return new WaitForSeconds(BattleInterval);
        EnemyBattleAround();
    }
    IEnumerator WaitPlayerSkillUse()
    {
        yield return new WaitForSeconds(BattleInterval);
        PlayerBattleAround();
    }
    IEnumerator WaitBattleAroundEnd()
    {
        yield return new WaitForSeconds(BattleTime);
        UpdatePokemonDatas();
        EnemyChooseSkillID = -1;
        PlayChooseSkillID = -1;
        PlayerChooseBagItemName = "";
        EnemyChooseBagItemName = "";
        if (null != battleState && CanBattle)
            UpdateBattleState();
    }
    /// <summary>
    /// 敌方行动
    /// </summary>
    private void EnemyAction()
    {
        int index = RandomService.game.Int(0, EnemyCurPokemonData.skills.Count);
        EnemyChooseSkillID = EnemyCurPokemonData.skills[index];
    }

    /// <summary>
    /// 精灵死亡事件
    /// </summary>
    /// <param name="notific"></param>
    public void PokemonDeathEvent(Notification<int> notific)
    {
        int hashcode = notific.param;

        if(hashcode == PlayerCurPokemonData.ID)
        {
            

            BattlePokemonData newCallPokemon = null;
            foreach (BattlePokemonData pokemon in playPokemons)
            {
                if(pokemon.curHealth>0)
                {
                   
                    newCallPokemon = pokemon;
                    break;
                }
            }

            if(null == newCallPokemon)
            {
                PlayerCurPokemonData.transform.gameObject.SetActive(false);
                ObjectPoolController.PokemonObjectsPool[PlayerCurPokemonData.race.raceid] =
                PlayerCurPokemonData.transform.gameObject;
                PlayerCurPokemonData = null;
                StartCoroutine(WaitBattleEnd());
                return;
            }

            ExchangePokemon(newCallPokemon);


        }
        else if(hashcode == EnemyCurPokemonData.ID)
        {
            EnemyCurPokemonData = null;
            StartCoroutine(WaitBattleEnd());
        }
    }

    private IEnumerator WaitBattleEnd()
    {
        TTUIPage.ShowPage<UIBattleResult>();
        yield return new WaitForSeconds(BattleTime+0.5f);
        context.isBattleFlag = false;
    }

    
}
