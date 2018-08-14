using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;
using System.Linq;

/// <summary>
/// 草地生成系统
/// </summary>
public class AddGrassSystem : ReactiveSystem<GameEntity>
{
    readonly Transform GrassLand = new GameObject("GrassLand").transform;
    readonly GameContext _context;

    private GameObject[] gos;
    private List<GameObject> grassGameObjects = new List<GameObject>(200);
    public AddGrassSystem(Contexts contexts) : base(contexts.game)
    {
        _context = contexts.game;
        GrassLand.gameObject.isStatic = true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (GameEntity e in entities)
        {
            Vector3 grassPos = e.grassPos.pos;

            GameObject glass = UnityEngine.Object.Instantiate(ResourceController.Instance.glassPrefab, GrassLand);
            glass.isStatic = true;
            grassGameObjects.Add(glass);
            glass.transform.position = grassPos;
            
        }
        gos = grassGameObjects.ToArray();

        StaticBatchingUtility.Combine(gos, GrassLand.gameObject);// 静态合并
        grassGameObjects = new List<GameObject>(200);

    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasGrassPos;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.GrassPos);
    }


}
