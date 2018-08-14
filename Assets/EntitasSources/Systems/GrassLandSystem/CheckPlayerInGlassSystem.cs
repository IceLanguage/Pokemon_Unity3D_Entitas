using System;
using System.Collections.Generic;
using System.Text;
using Entitas;
using PokemonBattele;
using UnityEngine;
using System.Linq;

class CheckPlayerInGrassSystem : IExecuteSystem
{
    private readonly GameContext context;
    private readonly Transform player;
    //private List<Vector3> GrassPosList = new List<Vector3>();
    private Rigidbody PlayerRigidbody;
    private const float EncounterPokemonProbability = 0.2f;
    private Vector3[] grassPosList;
    public CheckPlayerInGrassSystem(Contexts contexts,Transform player)
    {
        context = contexts.game;
        this.player = player;
        PlayerRigidbody = player.GetComponent<Rigidbody>();
        

    }
    private EncounterPokemon grassPokemons;
    public EncounterPokemon GrassPokemons
    {
        get
        {
            if (null == grassPokemons)
            {
                grassPokemons = ResourceController.Instance.glassPokemons;
                
            }
            return grassPokemons;
        }
    }
    public void Execute()
    {
        if (context.isBattleFlag)
            return;
        if(null == grassPosList || 1>grassPosList.Length)
        {
            grassPosList = context.GetEntities(GameMatcher.GrassPos)
            .Select(x => x.grassPos.pos)
            .ToArray();
        }
            
       
        int DetectGrassNum = 0;
        Vector3 playPos = new Vector3(player.position.x, 0, player.position.z);
        int count = grassPosList.Length;
        for(int i =0;i<count;++i)
        {
            if(Math.Abs(grassPosList[i].x-playPos.x)<=1)
            {
                if (Math.Abs(grassPosList[i].z- playPos.z) <= 1)
                {
                    DetectGrassNum++;
                }
            }
        }


        //遭遇精灵
        if (0 < DetectGrassNum && Vector3.Magnitude(PlayerRigidbody.velocity) > 0.3f)
        {
            float p = EncounterPokemonProbability * DetectGrassNum;
            if (RandomService.game.Float(0, 100) < p)
            {
                //int PokemonID = PokemonFactory.GetPokemonFromEncounterPokemonScriptableObject(GrassPokemons);
                Pokemon pokemom = PokemonFactory.BuildPokemon(GrassPokemons);
                BattleController.Instance.InitWildPokemon(pokemom);
                DebugHelper.LogFormat("你在草地遭遇了精灵{0}", pokemom.ename);
                context.isBattleFlag = true;


            }
        }
    }
}
