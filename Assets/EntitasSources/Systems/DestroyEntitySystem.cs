using System.Collections.Generic;
using Entitas;

public sealed class DestroyEntitySystem : ReactiveSystem<GameEntity>, ICleanupSystem
{

    //readonly IGroup<GameEntity> _group;
    List<GameEntity> PreDestoryEntities = new List<GameEntity>(30);

    public DestroyEntitySystem(Contexts contexts):base(contexts.game)
    {
       
    }

    public void Cleanup()
    {
        if (0 == PreDestoryEntities.Count) return;
        foreach(var entity in PreDestoryEntities)
        {
            entity.Destroy();
        }
        PreDestoryEntities = new List<GameEntity>(30);

    }

    protected override void Execute(List<GameEntity> entities)
    {
       foreach(var entity in entities)
       {
            PreDestoryEntities.Add(entity);
       }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isDestroy;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Destroy);
    }
}