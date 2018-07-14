using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BattleController : SingletonMonobehavior<BattleController>, IEndBattleEvent
{
    public Transform PlayerTransform;
    public Transform PlayerPokemonTransform;
    public Transform EnemyPokemonTransform;

    private List<Pokemon> wildPokemons = new List<Pokemon>();
    private List<Pokemon> playPokemons = new List<Pokemon>();
    private List<GameObject> BattlePokemonsGameObejcts = new List<GameObject>();
    public Pokemon PlayerCurPokemon { get;private set; }
    public Pokemon EnemyCurPokemon { get; private set; }
    private void Start()
    {
        EndBattleSystem.EndBattleEvent += EndBattleEvent;
    }
    public bool CanBattle
    {
        get
        {
            return null != PlayerCurPokemon && null != EnemyCurPokemon;
        }
    }
    public void InitWildPokemon(params Pokemon[] pokemons)
    {
        wildPokemons = new List<Pokemon>(pokemons);
        foreach (var pokemon in wildPokemons)
            new BattlePokemonData(pokemon);
        if (0 == pokemons.Length)
        {
            Debug.LogWarning("没有野生精灵怎么打");
            Contexts.sharedInstance.game.isBattleFlag = false;
            return;
        }
        
        EnemyCurPokemon = wildPokemons[0];
    }
    public void InitPlayerPokemons(List<Pokemon> pokemons)
    {
       
        playPokemons = new List<Pokemon>(pokemons);
        foreach (var pokemon in playPokemons)
            new BattlePokemonData(pokemon);
        if (0 == pokemons.Count)
        {
            Debug.LogError("没有精灵怎么打");
            Contexts.sharedInstance.game.isBattleFlag = false;
            return;
        }
        PlayerCurPokemon = playPokemons[0]; 
    }

    public void BeginBattle()
    {
        //召唤精灵
        GameObject playerPokemon = PokemonFactory.InitPokemon(PlayerCurPokemon.raceID);
        playerPokemon.transform.position = PlayerPokemonTransform.position ;
        playerPokemon.transform.parent = PlayerPokemonTransform;
        PokemonFactory.PokemonBallEffect(playerPokemon.transform.position);
        GameObject enemyPokemon = PokemonFactory.InitPokemon(EnemyCurPokemon.raceID);
        enemyPokemon.transform.position = EnemyPokemonTransform.position ;
        enemyPokemon.transform.parent = EnemyPokemonTransform;
        PokemonFactory.PokemonBallEffect(enemyPokemon.transform.position);

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
        foreach(Pokemon pokemon in playPokemons)
        {
            BattlePokemonData.BattlePokemonDataContext[pokemon.GetInstanceID()].Recover();
        }
        foreach (Pokemon pokemon in wildPokemons)
        {
            BattlePokemonData.BattlePokemonDataContext[pokemon.GetInstanceID()].Recover();
        }
    }

    public void EndBattleEvent()
    {
        wildPokemons = new List<Pokemon>();
        playPokemons = new List<Pokemon>();
        PlayerCurPokemon = null;
        EnemyCurPokemon = null;
        for (int i = BattlePokemonsGameObejcts.Count - 1; i >= 0; --i)
        {
            Destroy(BattlePokemonsGameObejcts[i]);
            BattlePokemonsGameObejcts[i] = null;
        }
        BattlePokemonsGameObejcts = new List<GameObject>();
    }
}
