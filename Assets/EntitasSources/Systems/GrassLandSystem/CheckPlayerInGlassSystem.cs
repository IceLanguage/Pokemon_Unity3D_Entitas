using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;
using UnityEngine;

class CheckPlayerInGrassSystem : IExecuteSystem
{
    private readonly GameContext context;
    private readonly Transform player;
    private List<Vector3> GrassPosList = new List<Vector3>();
    private Rigidbody PlayerRigidbody;
    private const float EncounterPokemonProbability = 0.2f;
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

        GrassPosList = context.GetEntities(GameMatcher.GrassPos)
           .Select(x => x.grassPos.pos).ToList();

        Vector3 playPos = player.position;

        //获取距离玩家附近的草
        var CheckGrassList = GrassPosList.Where(x => Vector3.Distance(playPos, x) < 2f).ToList();
        if (0 == CheckGrassList.Count()) return;

        
        GeometryDetection.Sphere play_detection = new GeometryDetection.Sphere(playPos, 1.2f);

        //是否碰撞
        int DetectGrassNum = 0;
        
        foreach (Vector3 GrassPos in CheckGrassList)
        {
            //几何检测
            GeometryDetection.AABB grass_detection = 
                    new GeometryDetection.AABB(GrassPos,
                    new Vector3(
                        GrassPos.x + 1f,
                        GrassPos.y + 5f,
                        GrassPos.z + 1f));
            bool isDetected = GeometryDetection.Overlap_AABB_Sphere(grass_detection, play_detection);

            if (false == isDetected)
                continue;
            DetectGrassNum++;
            //GameEntity GrassEntity = context.GetEntityWithGrassPos(GrassPos);

            ////施加力
            //List<Force> forceList = GrassEntity.grassForces.forceList;

            ////和玩家运行方向相同的一个力
            //Vector3 force = PlayerRigidbody.velocity;
            //force.x = Mathf.Min(1, force.x);
            //force.y = Mathf.Min(1, force.y);
            //force.z = Mathf.Min(1, force.z);
            //force = Vector3.Normalize(force);
            //forceList.Add(new Force(force));

            ////草地被拨开的力
            //Vector3 direction = GrassPos - playPos;
            //Vector3 left = new Vector3(force.z, 0, -force.x);
            //force = new Vector3(force.z, 0, -force.x);
            //if (Vector3.Dot(direction, left) <= 0)
            //{
            //    force = new Vector3(-force.z, 0, force.x);
            //}
            //force = Vector3.Normalize(force);
            //forceList.Add(new Force(force));

            ////更新草的受力
            //GrassEntity.ReplaceGrassForces(forceList);
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
