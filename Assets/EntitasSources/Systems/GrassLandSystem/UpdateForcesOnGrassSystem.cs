using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

class UpdateForcesOnGrassSystem : ReactiveSystem<GameEntity>
{
    private readonly GameContext context;
    private const float WaveFrequency = 1.0f;
    private const float Resistance = 0.1f;

    
    public UpdateForcesOnGrassSystem(Contexts contexts) : base(contexts.game)
    {
        context = contexts.game;
    }

    private float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GrassForces);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGrassForces && entity.grassForces.forceList.Count > 0 && entity.hasGrassMeshRender;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (GameEntity grass in entities)
        {

            List<Force> forcelist = grass.grassForces.forceList;
            List<Force> newforceList = new List<Force>();

            Vector3 accForce = Vector3.zero;
            foreach (Force force in forcelist)
            {

                if (force.m_Force.magnitude > 0.1f)
                {
                    // [-1, 1] 正玄波模拟简谐运动  
                    float wave_factor = Mathf.Sin(force.m_Time * WaveFrequency);

                    // 力的指数衰减      
                    float resistance_factor = EaseOutExpo(1, 0, Resistance * Time.deltaTime);

                    force.m_Force *= resistance_factor;
                    force.m_Time += Time.deltaTime;

                    // 累加  
                    accForce += force.m_Force * wave_factor;

                    newforceList.Add(force);
                }


            }
            if (accForce != Vector3.zero)
            {
                Material material = grass.grassMeshRender.meshRenderer.sharedMaterial;
                if (material.HasProperty("_Force"))
                {
                    accForce = grass.grassMeshRender.meshRenderer.transform
                        .InverseTransformVector(accForce); // 世界空间转换到模型本地空间  
                    material.SetVector("_Force", accForce);
                }
            }
            grass.ReplaceGrassForces(newforceList);


        }
    }
}
