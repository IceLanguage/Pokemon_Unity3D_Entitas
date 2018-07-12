using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

class CheckPlayerInGlassSystem : IExecuteSystem
{
    private readonly GameContext context;
    private readonly Transform player;
    private List<Vector3> GrassPosList = new List<Vector3>();
    public CheckPlayerInGlassSystem(Contexts contexts,Transform player)
    {
        context = contexts.game;
        this.player = player;
       
    }

    public void Execute()
    {
        GrassPosList = context.GetEntities(GameMatcher.GlassPos)
           .Select(x => x.glassPos.pos).ToList();

        Vector3 playPos = player.position;

        //获取距离玩家最近的草
        var CheckGrassList = GrassPosList.Where(x => Vector3.Distance(playPos, x) < 2f).ToList();
        if (0 == CheckGrassList.Count()) return;

        
        GeometryDetection.Sphere play_detection = new GeometryDetection.Sphere(playPos, 1);
        foreach (Vector3 GlassPos in CheckGrassList)
        {
            //几何检测
            GeometryDetection.AABB grass_detection = 
                    new GeometryDetection.AABB(GlassPos,
                    new Vector3(
                        GlassPos.x + 1.5f,
                        GlassPos.y + 5f,
                        GlassPos.z + 1.5f));
            bool isDetected = GeometryDetection.Overlap_AABB_Sphere(grass_detection, play_detection);

            if (false == isDetected)
                continue;

            GameEntity GlassEntity = context.GetEntityWithGlassPos(GlassPos);

            //施加力
            List<Force> forceList = GlassEntity.glassForces.forceList;

            //和玩家运行方向相同的一个力
            Vector3 force = player.GetComponent<Rigidbody>().velocity;
            force.x = Mathf.Min(1, force.x);
            force.y = Mathf.Min(1, force.y);
            force.z = Mathf.Min(1, force.z);
            force = Vector3.Normalize(force);
            forceList.Add(new Force(force));

            //草地被拨开的力
            Vector3 direction = GlassPos - playPos;
            Vector3 left = new Vector3(force.z, 0, -force.x);
            force = new Vector3(force.z, 0, -force.x);
            if (Vector3.Dot(direction, left) <= 0)
            {
                force = new Vector3(-force.z, 0, force.x);
            }
            force = Vector3.Normalize(force);
            forceList.Add(new Force(force));

            //更新草的受力
            GlassEntity.ReplaceGlassForces(forceList);
        }
    }
}
