using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

class WindForGlassSystem : IExecuteSystem
{
    private readonly GameContext context;
    private float timer = 0;
    GameEntity[] entities;
    public WindForGlassSystem(Contexts contexts) 
    {
        context = contexts.game;
    }
    public void Execute()
    {
        timer += Time.deltaTime;
        
        if (timer>5f)
        {
            timer = 0;
            if (null == entities)
                entities = context.GetEntities(GameMatcher.GrassForces);
            foreach (var entity in entities)
            {
                List<Force> newforceList = entity.grassForces.forceList;
                newforceList.Add(new Force(new Vector3(10, 0, 10)));
                entity.ReplaceGrassForces(newforceList);
            }
        }
    }
}
