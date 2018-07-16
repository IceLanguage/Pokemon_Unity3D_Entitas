using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyUnityEventDispatcher;

public class BattleController : SingletonMonobehavior<BattleController>, IEndBattleEvent
{
    public Transform PlayerTransform;
    public Transform PlayerPokemonTransform;
    public Transform EnemyPokemonTransform;

    private List<BattlePokemonData> wildPokemons = new List<BattlePokemonData>();
    public List<BattlePokemonData> playPokemons = new List<BattlePokemonData>();
    private List<GameObject> BattlePokemonsGameObejcts = new List<GameObject>();
    public BattlePokemonData PlayerCurPokemonData { get; private set; }
    public BattlePokemonData EnemyCurPokemonData { get; private set; }
    private void Start()
    {
        NotificationCenter<int>.Get().AddEventListener("UseSkill", PlayerPokemonUseSkill); 
        EndBattleSystem.EndBattleEvent += EndBattleEvent;
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
    }

    public void EndBattleEvent()
    {
        //初始化精灵数据
        foreach (BattlePokemonData pokemon in playPokemons)
        {
            pokemon.Recover();
        }
        foreach (BattlePokemonData pokemon in wildPokemons)
        {
            pokemon.Recover();
        }
        wildPokemons = new List<BattlePokemonData>();
        playPokemons = new List<BattlePokemonData>();
        PlayerCurPokemonData = null;
        EnemyCurPokemonData = null;
        for (int i = BattlePokemonsGameObejcts.Count - 1; i >= 0; --i)
        {
            Destroy(BattlePokemonsGameObejcts[i]);
            BattlePokemonsGameObejcts[i] = null;
        }
        BattlePokemonsGameObejcts = new List<GameObject>();
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
            Debug.Log("技能PP已用完");
            return;
        }
        Skill skill = ResourceController.Instance.allSkillDic[skillID];
        UseSkill.Attack(skill, PlayerCurPokemonData, EnemyCurPokemonData);
    }
}
