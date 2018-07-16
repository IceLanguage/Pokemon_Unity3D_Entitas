using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyUnityEventDispatcher;
using System.Collections;

public sealed partial class BattleController : SingletonMonobehavior<BattleController>, IEndBattleEvent
{
    public Transform PlayerTransform;
    public Transform PlayerPokemonTransform;
    public Transform EnemyPokemonTransform;

    private List<BattlePokemonData> wildPokemons = new List<BattlePokemonData>();
    public List<BattlePokemonData> playPokemons = new List<BattlePokemonData>();
    private List<GameObject> BattlePokemonsGameObejcts = new List<GameObject>();
    public BattlePokemonData PlayerCurPokemonData { get; private set; }
    public BattlePokemonData EnemyCurPokemonData { get; private set; }

    private GameContext context;
    private BattleState battleState;

    private int PlayChooseSkillID = -1;
    private int EnemyChooseSkillID = -1;

    public readonly float BattleTime = 2f;
    private void Start()
    {
        context = Contexts.sharedInstance.game;
        NotificationCenter<int>.Get().AddEventListener("UseSkill", PlayerPokemonUseSkill);
        NotificationCenter<int>.Get().AddEventListener("PokemonDeathMessage", PokemonDeathEvent);
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

        BattlePokemonsGameObejcts.Add(playerPokemon);
        BattlePokemonsGameObejcts.Add(enemyPokemon);

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
        
        
        for (int i = BattlePokemonsGameObejcts.Count - 1; i >= 0; --i)
        {
            Destroy(BattlePokemonsGameObejcts[i]);
            BattlePokemonsGameObejcts[i] = null;
        }
        BattlePokemonsGameObejcts = new List<GameObject>();
        //初始化精灵数据
        foreach (BattlePokemonData pokemon in playPokemons)
        {
            GameEntity entity = context.GetEntityWithBattlePokemonData(pokemon);
            Action action = entity.pokemonDataChangeEvent.Event;
            action = () => { };
            entity.ReplacePokemonDataChangeEvent(action);
            pokemon.Recover();
        }
        foreach (BattlePokemonData pokemon in wildPokemons)
        {
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
    public void PlayerPokemonUseSkill(Notification<int> notific)
    {
        int skillID = PlayerCurPokemonData.skills[notific.param];
        int pp = PlayerCurPokemonData.skillPPs[notific.param];
        if (pp <= 0)
        {
            NotificationCenter<int>.Get().DispatchEvent("DisableSkillButton", notific.param);
            return;
        }
        PlayChooseSkillID = skillID;
        battleState = battleState.ChangeState();

       
    }

    /// <summary>
    /// 玩家回合
    /// </summary>
    private void PlayerRound()
    {
        EnemyAction();
        int i = 0;
        for (i = 0; i < PlayerCurPokemonData.skills.Count; i++)
        {
            if(PlayerCurPokemonData.skillPPs[i]>0)
                NotificationCenter<int>.Get().DispatchEvent("EnableSkillButton", i);
        }
        while(i<4)
        {
            NotificationCenter<int>.Get().DispatchEvent("EnableSkillButton", i);
            i++;
        }
    }

    /// <summary>
    /// 对战回合
    /// </summary>
    private void BattleRound()
    {
        for(int i=0;i<4;i++)
        {
            NotificationCenter<int>.Get().DispatchEvent("DisableSkillButton", i);
        }
        if(PlayerCurPokemonData.Speed>EnemyCurPokemonData.Speed)
        {
            if (CanBattle&& ResourceController.Instance.allSkillDic.ContainsKey(PlayChooseSkillID))
            {
                Skill PlayerSkill = ResourceController.Instance.allSkillDic[PlayChooseSkillID];
                if (null != PlayerSkill)
                    UseSkill.Attack(PlayerSkill, PlayerCurPokemonData, EnemyCurPokemonData);
            }
            StartCoroutine(WaitEnemySkillUse());

        }
        else
        {
            if (CanBattle && ResourceController.Instance.allSkillDic.ContainsKey(EnemyChooseSkillID))
            {
                Skill EnemySkill = ResourceController.Instance.allSkillDic[EnemyChooseSkillID];
                if (null != EnemySkill)
                    UseSkill.Attack(EnemySkill, EnemyCurPokemonData, PlayerCurPokemonData);
            }
            StartCoroutine(WaitPlayerSkillUse());

        }
        StartCoroutine(WaitBattleAroundEnd());

    }
    IEnumerator WaitEnemySkillUse()
    {
        yield return new WaitForSeconds(0.5f);
        if (CanBattle && ResourceController.Instance.allSkillDic.ContainsKey(EnemyChooseSkillID))
        {
            Skill EnemySkill = ResourceController.Instance.allSkillDic[EnemyChooseSkillID];
            if (null != EnemySkill)
                UseSkill.Attack(EnemySkill, EnemyCurPokemonData, PlayerCurPokemonData);
        }
    }
    IEnumerator WaitPlayerSkillUse()
    {
        yield return new WaitForSeconds(0.5f);
        if (CanBattle && ResourceController.Instance.allSkillDic.ContainsKey(PlayChooseSkillID))
        {
            Skill PlayerSkill = ResourceController.Instance.allSkillDic[PlayChooseSkillID];
            if (null != PlayerSkill)
                UseSkill.Attack(PlayerSkill, PlayerCurPokemonData, EnemyCurPokemonData);
        }
    }
    IEnumerator WaitBattleAroundEnd()
    {
        yield return new WaitForSeconds(BattleTime);
        if(null != battleState&&CanBattle)
            battleState = battleState.ChangeState();
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
            Destroy(PlayerCurPokemonData.transform.gameObject);
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
                PlayerCurPokemonData = null;
                StartCoroutine(WaitBattleEnd());
                return;
            }
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

            BattlePokemonsGameObejcts.Add(playerPokemon);



        }
        else if(hashcode == EnemyCurPokemonData.ID)
        {
            EnemyCurPokemonData = null;
            StartCoroutine(WaitBattleEnd());
        }
    }

    private IEnumerator WaitBattleEnd()
    {
        yield return new WaitForSeconds(BattleTime+0.5f);
        context.isBattleFlag = false;
    }
}
