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
            Debug.Log(grassPosList);
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

        //Vector3 playPos = player.position;

        ////获取距离玩家附近的草
        //var CheckGrassList = GrassPosList.Where(x => Vector3.Distance(playPos, x) < 2f).ToList();
        //if (0 == CheckGrassList.Count()) return;


        //GeometryDetection.Sphere play_detection = new GeometryDetection.Sphere(playPos, 1.2f);

        ////是否碰撞
        //int DetectGrassNum = 0;

        //foreach (Vector3 GrassPos in CheckGrassList)
        //{
        //    //几何检测
        //    GeometryDetection.AABB grass_detection = 
        //            new GeometryDetection.AABB(GrassPos,
        //            new Vector3(
        //                GrassPos.x + 1f,
        //                GrassPos.y + 5f,
        //                GrassPos.z + 1f));
        //    bool isDetected = GeometryDetection.Overlap_AABB_Sphere(grass_detection, play_detection);

        //    if (false == isDetected)
        //        continue;
        //    DetectGrassNum++;
        //}

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
