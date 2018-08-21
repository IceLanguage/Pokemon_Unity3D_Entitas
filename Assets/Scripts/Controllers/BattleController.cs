using PokemonBattele;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using UnityEngine;
using MyUnityEventDispatcher;
using System.Collections;
using TinyTeam.UI;
using Entitas;

public sealed partial class BattleController : SingletonMonobehavior<BattleController>, IEndBattleEvent
{
    public Transform PlayerTransform;
    public Transform PlayerPokemonTransform;
    public Transform EnemyPokemonTransform;

    private List<BattlePokemonData> wildPokemons = new List<BattlePokemonData>(6);
    public List<BattlePokemonData> playPokemons = new List<BattlePokemonData>(6);
    public BattlePokemonData PlayerCurPokemonData { get; private set; }
    public BattlePokemonData EnemyCurPokemonData { get; private set; }

    private GameContext context;
    private BattleState battleState;

    private int PlayChooseSkillID = -1;
    private int EnemyChooseSkillID = -1;

    public static readonly float BattleTime = 2f;
    
    private string PlayerChooseBagItemName = "", EnemyChooseBagItemName = "";

    public int FirstHand = -1;
    private static bool BattlePause = false;
    public int BattleAroundCount = 0;

    private Trainer trainer;
    private void Start()
    {
        context = Contexts.sharedInstance.game;
        NotificationCenter<int>.Get().AddEventListener("UseSkill", PlayerPokemonUseSkill);
        NotificationCenter<int>.Get().AddEventListener("PokemonDeathMessage", PokemonDeathEvent);
        NotificationCenter<string>.Get().AddEventListener("UseBagItem", PlayerUseBagItem);
        NotificationCenter<int>.Get().AddEventListener("CatchPokemon", CatchPokemonResultEvent);
        NotificationCenter<int>.Get().AddEventListener("ExchangePokemon", ExchangePokemon);
        NotificationCenter<bool>.Get().AddEventListener("BattlePause", StopBattlePause);
        EndBattleSystem.EndBattleEvent += EndBattleEvent;

        BattleStateForPlayer.InitEvent += PlayerRound;
        BattleStateForBattle.InitEvent += BattleRound;

        
        //
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
        wildPokemons.Clear();
        foreach(Pokemon pokemon in pokemons)
        {
            wildPokemons.Add(BattlePokemonData.Context[pokemon.GetInstanceID()]);
        }
          
        if (0 == pokemons.Length)
        {
            Debug.LogWarning("没有野生精灵怎么打");
            Contexts.sharedInstance.game.isBattleFlag = false;
            return;
        }

        EnemyCurPokemonData = wildPokemons[0];
    }

    //战斗中使用的精灵每个种族只能有一只
    HashSet<int> battlePokemons = new HashSet<int>();
    public void InitPlayerPokemons(List<Pokemon> pokemons)
    {
        playPokemons.Clear();
        foreach (Pokemon pokemon in pokemons)
        {
            playPokemons.Add(BattlePokemonData.Context[pokemon.GetInstanceID()]);
        }

        battlePokemons.Clear();
        int count = playPokemons.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            BattlePokemonData pokemonData = playPokemons[i];
            int raceid = pokemonData.race.raceid;
            if(battlePokemons.Contains(raceid))
            {
                playPokemons.RemoveAt(i);
               
            }
            else
            {
                battlePokemons.Add(raceid);
            }
        }

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

        //初始化精灵数据
        foreach(BattlePokemonData pokemon in playPokemons)
        {
            pokemon.Recover();
        }
        foreach (BattlePokemonData pokemon in wildPokemons)
        {
            pokemon.Recover();
        }
        //考虑特性
        if (AbilityManager.AbilityImpacts.ContainsKey(PlayerCurPokemonData.ShowAbility))
            AbilityManager.AbilityImpacts[PlayerCurPokemonData.ShowAbility]
                .OnBeCalled(PlayerCurPokemonData);
        //考虑特性
        if (AbilityManager.AbilityImpacts.ContainsKey(EnemyCurPokemonData.ShowAbility))
            AbilityManager.AbilityImpacts[EnemyCurPokemonData.ShowAbility]
                .OnBeCalled(EnemyCurPokemonData);
        battleState = new BattleStateForPlayer();
        trainer = context.playerData.scriptableObject;
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


            GameEntity entity = pokemon.entity;
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

            GameEntity entity = pokemon.entity;
            Action action = entity.pokemonDataChangeEvent.Event;
            action = () => { };
            entity.ReplacePokemonDataChangeEvent(action);
            pokemon.Recover();
        }
        wildPokemons = new List<BattlePokemonData>(6);
        playPokemons = new List<BattlePokemonData>(6);
        PlayerCurPokemonData = null;
        EnemyCurPokemonData = null;
        battleState = null;

        GameEntity[] entities = context.GetEntities(GameMatcher.BattlePokemonData);
        var playerPokemons = context.playerData.scriptableObject.pokemons;
        foreach (var e in entities)
        {
            if(!playerPokemons.Contains(e.battlePokemonData.data.pokemon))
            {
                e.isDestroy = true;
            }
        }
        
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
        if(DisableSkill.context.ContainsKey(PlayerCurPokemonData.ID))
        {
            int disableskillID = DisableSkill.context[PlayerCurPokemonData.ID];
            for(int i=0;i< PlayerCurPokemonData.skills.Count;++i)
            {
                if(disableskillID == PlayerCurPokemonData.skills[i])
                {
                    NotificationCenter<int>.Get().DispatchEvent("DisableSkillButton", i);
                    return;
                }
            }
        }
        PlayChooseSkillID = skillID;
        DebugHelper.LogFormat("你选择使用了技能{0}",ResourceController.Instance.allSkillDic[PlayChooseSkillID].sname);
        UpdateBattleState();
        

       
    }
    private void PlayerUseBagItem(Notification<string> notific)
    {
        PlayerChooseBagItemName = notific.param;
        UpdateBattleState(); 
    }

    private void CatchPokemonResultEvent(Notification<int> notific)
    {
        battleState = null;
        StartCoroutine(WaitBattleEnd());
    }
    private void StopBattlePause(Notification<bool> notific)
    {
        BattlePause = notific.param;
    }
    private void ExchangePokemon(Notification<int> notific)
    {
        if(PlayerCurPokemonData.ChangeStateForPokemonEnums.Contains(ChangeStateEnumForPokemon.CanNotEscape))
        {
            //不允许逃脱
            DebugHelper.LogFormat("当前精灵{0}处于逃脱状态，不允许替换精灵", PlayerCurPokemonData.Ename);

            return;
        }
        TTUIPage.ShowPage<UIBattle_Skills>();

        BattlePokemonData newCallPokemon = playPokemons[notific.param];       
        ExchangePokemon(newCallPokemon);
        UpdateBattleState();
    }

    private void ExchangePokemon(BattlePokemonData newCallPokemon)
    {

        DebugHelper.LogFormat("我方玩家将精灵{0}替换成了{1}", PlayerCurPokemonData.Ename, newCallPokemon.Ename);

        //回收精灵GameObject
        PlayerCurPokemonData.transform.gameObject.SetActive(false);
        ObjectPoolController.PokemonObjectsPool[PlayerCurPokemonData.race.raceid] =
        PlayerCurPokemonData.transform.gameObject;

        //如果剧毒就重置计数
        if (AbnormalStateEnum.BadlyPoison == PlayerCurPokemonData.Abnormal)
        {
            BadlyPoisonState.count[PlayerCurPokemonData.ID] = 1;
        }

        //能力阶级重置
        PlayerCurPokemonData.StatModifiers = new StatModifiers();

        //考虑特性
        if (AbilityManager.AbilityImpacts.ContainsKey(PlayerCurPokemonData.ShowAbility))
            AbilityManager.AbilityImpacts[PlayerCurPokemonData.ShowAbility]
                .OnLeave(PlayerCurPokemonData);

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

        //考虑特性
        if (AbilityManager.AbilityImpacts.ContainsKey(PlayerCurPokemonData.ShowAbility))
            AbilityManager.AbilityImpacts[PlayerCurPokemonData.ShowAbility]
                .OnBeCalled(PlayerCurPokemonData);

        TTUIPage.ShowPage<UIBattle_PokemonInfos>();
 
    }
    /// <summary>
    /// 更新战斗状态
    /// </summary>
    private void UpdateBattleState()
    {
        if(!CanBattle)
        {
            return;
        }

        
        battleState = battleState.ChangeState();
    }

    
    /// <summary>
    /// 玩家回合
    /// </summary>
    private void PlayerRound()
    {
        DebugHelper.Log("开始进入准备回合");
        BattleAroundCount++;
        EnemyAction();
    }

    /// <summary>
    /// 对战回合
    /// </summary>
    private void BattleRound()
    {

        
        if (!CanBattle) return;
        DebugHelper.Log("开始进入对战回合");

        StartCoroutine(IEBattleAround());

        

    }
    private WaitWhile waitBattlePauseEnd = new WaitWhile
        (
            () =>
            {
                return BattlePause;
            }
        );
    IEnumerator IEBattleAround()
    {
        
        //我方玩家使用道具
        
        if (null!= PlayerChooseBagItemName&& PlayerChooseBagItemName .Length>0&& ResourceController.Instance.UseBagUItemDict.ContainsKey(PlayerChooseBagItemName))
        {
            
            UseBagItem use = ResourceController.Instance.UseBagUItemDict[PlayerChooseBagItemName];
            if (null != use&&use.canUseInBattle)
            {
                BattlePause = true;
                if(!use.UseForPlay)
                    use.Effect(EnemyCurPokemonData);
                else
                    use.Effect(PlayerCurPokemonData);
            }
        }
        yield return waitBattlePauseEnd;
        if(CanBattle&&null!=battleState)
        {
            if (null != EnemyChooseBagItemName&& EnemyChooseBagItemName.Length>0 && ResourceController.Instance.UseBagUItemDict.ContainsKey(EnemyChooseBagItemName))
            {
                UseBagItem use = ResourceController.Instance.UseBagUItemDict[EnemyChooseBagItemName];
                if (null != use && use.canUseInBattle)
                {
                    BattlePause = true;
                    if (use.UseForPlay)
                        use.Effect(EnemyCurPokemonData);
                    else
                        use.Effect(PlayerCurPokemonData);
                }
            }
        }
        yield return waitBattlePauseEnd;
        //互相攻击
        if(PlayChooseSkillID!=-1&&EnemyChooseSkillID!=-1)
        {
            if (PlayerCurPokemonData.Speed > EnemyCurPokemonData.Speed)
            {
                FirstHand = PlayerCurPokemonData.ID;
                PlayerBattleAround();
                yield return waitBattlePauseEnd;
                EnemyBattleAround();

            }
            else
            {
                FirstHand = EnemyCurPokemonData.ID;

                EnemyBattleAround();
                yield return waitBattlePauseEnd;
                PlayerBattleAround();

            }
        }
        else if(-1== PlayChooseSkillID && -1!= EnemyChooseSkillID)
        {
            FirstHand = EnemyCurPokemonData.ID;
            EnemyBattleAround();
        }
        else if (-1 == EnemyChooseSkillID && -1 != PlayChooseSkillID)
        {
            FirstHand = PlayerCurPokemonData.ID;
            PlayerBattleAround();
        }
        yield return waitBattlePauseEnd;
        BattleAroundEnd();

    }
    /// <summary>
    /// 更新玩家和精灵数据
    /// </summary>
    private void UpdatePokemonDatas()
    {
        foreach(var data in wildPokemons)
        {
            var entity = data.entity;
            entity.ReplaceBattlePokemonData(data);
        }
        foreach (var data in playPokemons)
        {
            var entity = data.entity;
            entity.ReplaceBattlePokemonData(data);
        }
        context.ReplacePlayerData(context.playerData.scriptableObject);
    }

    private void PlayerBattleAround()
    {
        UpdatePokemonDatas();
        if (CanBattle&&null!=battleState)
        {
            
            DebugHelper.Log("我方开始了行动");
            PlayerCurPokemonData.ChooseSkillType = SkillType.NULL;

            if (NeedReplaceSKill.context.ContainsKey(PlayerCurPokemonData.ID))
            {
                PlayChooseSkillID = NeedReplaceSKill.context[PlayerCurPokemonData.ID];
            }

            if (ResourceController.Instance.allSkillDic.ContainsKey(PlayChooseSkillID))
            {
                

                Skill PlayerSkill = ResourceController.Instance.allSkillDic[PlayChooseSkillID];
                if (null != PlayerSkill)
                {
                    BattlePause = true;
                    PlayerCurPokemonData.ChooseSkillType = PlayerSkill.type;
                    UseSkill.Attack(PlayerSkill, PlayerCurPokemonData, EnemyCurPokemonData);
                }
                    
            }
            
            
        }
    }

    private void EnemyBattleAround()
    {
        UpdatePokemonDatas();
        if (CanBattle && null != battleState)
        {
           
            DebugHelper.Log("敌方开始了行动");
            EnemyCurPokemonData.ChooseSkillType = SkillType.NULL;

            if(NeedReplaceSKill.context.ContainsKey(EnemyCurPokemonData.ID))
            {
                EnemyChooseSkillID = NeedReplaceSKill.context[EnemyCurPokemonData.ID];
            }
            if (ResourceController.Instance.allSkillDic.ContainsKey(EnemyChooseSkillID))
            {
                

                Skill EnemySkill = ResourceController.Instance.allSkillDic[EnemyChooseSkillID];
                if (null != EnemySkill)
                {
                    BattlePause = true;
                    EnemyCurPokemonData.ChooseSkillType = EnemySkill.type;
                    UseSkill.Attack(EnemySkill, EnemyCurPokemonData, PlayerCurPokemonData);
                }
                    
            }

            
        }

    }

    private void BattleAroundEnd()
    {

        PlayerCurPokemonData.StateForAbnormal.UpdateInPlayerAround(PlayerCurPokemonData);
        EnemyCurPokemonData.StateForAbnormal.UpdateInPlayerAround(EnemyCurPokemonData);
        int cout = PlayerCurPokemonData.ChangeStateForPokemonEnums.Count;
        for(int i = cout-1; i>=0;--i)
        {
            var state = PlayerCurPokemonData.ChangeStateForPokemonEnums[i];
            ChangeStateForPokemon.ChangeStateForPokemons[state].UpdateInPlayerAround(PlayerCurPokemonData);
        }
        cout = EnemyCurPokemonData.ChangeStateForPokemonEnums.Count;
        for (int i = cout - 1; i >= 0; --i)
        {
            var state = EnemyCurPokemonData.ChangeStateForPokemonEnums[i];
            ChangeStateForPokemon.ChangeStateForPokemons[state].UpdateInPlayerAround(PlayerCurPokemonData);
        }

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
        if (NeedReplaceSKill.context.ContainsKey(EnemyCurPokemonData.ID))
        {
            EnemyChooseSkillID = NeedReplaceSKill.context[EnemyCurPokemonData.ID];
        }
        DebugHelper.LogFormat("野外的精灵{0}选择使用了技能{1}",
            EnemyCurPokemonData.Ename,
            ResourceController.Instance.allSkillDic[EnemyChooseSkillID].sname);
    }

    /// <summary>
    /// 精灵死亡事件
    /// </summary>
    /// <param name="notific"></param>
    public void PokemonDeathEvent(Notification<int> notific)
    {
        int hashcode = notific.param;
        if (!CanBattle||null==battleState) return;
        if(hashcode == PlayerCurPokemonData.ID)
        {
            DebugHelper.LogFormat("我方精灵{0}不能继续作战了", PlayerCurPokemonData.Ename);


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
                battleState = null;
                StartCoroutine(WaitBattleEnd());
                return;
            }

            ExchangePokemon(newCallPokemon);


        }
        else if(hashcode == EnemyCurPokemonData.ID)
        {
            DebugHelper.LogFormat("敌方精灵{0}不能继续作战了", EnemyCurPokemonData.Ename);
            battleState = null;
            StartCoroutine(WaitBattleEnd());
        }
    }
    private WaitForSeconds endBattleWait = new WaitForSeconds(BattleTime);
    string winer = "敌方";
    private IEnumerator WaitBattleEnd()
    {
        winer = "敌方";
        if (null == EnemyCurPokemonData||EnemyCurPokemonData.curHealth<=0)
        {
            winer = "我方";
            WinResoult();
        }
        DebugHelper.LogFormat("战斗结束，{0}取得了胜利",winer);

        TTUIPage.ShowPage<UIBattleResult>();
        yield return endBattleWait;
        context.isBattleFlag = false;
    }
    private Dictionary<string, UseBagItem>.KeyCollection bagitems;
    /// <summary>
    /// 获胜后的奖励
    /// </summary>
    private void WinResoult()
    {
        if(null == bagitems)
            bagitems = ResourceController.Instance.UseBagUItemDict.Keys;
        var trainerBagItems = trainer.bagItems;
        int randomIndex = RandomService.game.Int(0, bagitems.Count);
        int i = 0;
        string itemName = "";
        foreach (var item in bagitems)
        {
            if(i++ == randomIndex)
            {
                itemName = item;
                break;
            }
        }
        if(ResourceController.Instance.UseBagUItemDict.ContainsKey(itemName))
        {
            int itemCount = RandomService.game.Int(1, 6);
            DebugHelper.Log(
                new StringBuilder(30)
               .AppendFormat("玩家获得 {0}", itemName)
               .Append(itemCount)
               .Append("个")
               .ToString());
          
            var item = trainerBagItems.Find(x => x.ItemName == itemName);
            if (null == item)
                trainerBagItems.Add(BagItems.Build(itemName, itemCount));
            else
                item.count += itemCount; 
            context.ReplacePlayerData(trainer);

        }
    }
}
