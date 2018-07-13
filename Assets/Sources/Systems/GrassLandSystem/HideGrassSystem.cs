using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

class HideGrassSystem : ReactiveSystem<GameEntity>, IEndBattleEvent
{
    private readonly GameContext context;
    private bool BattleFlag = false;
    public HideGrassSystem(Contexts contexts):base(contexts.game)
    {
        context = contexts.game;
        EndBattleSystem.EndBattleEvent += EndBattleEvent;
        
    }
    protected override void Execute(List<GameEntity> entities)
    {
        if (BattleFlag == context.isBattleFlag) return;

        var grassList = context.GetEntities(GameMatcher.GrassMeshRender);

        foreach (var e in grassList)
        {
            e.grassMeshRender.meshRenderer.gameObject.SetActive(!context.isBattleFlag);

        }
        BattleFlag = context.isBattleFlag;
    }
    private void Update()
    {
        if (BattleFlag == context.isBattleFlag) return;

        var grassList = context.GetEntities(GameMatcher.GrassMeshRender);

        foreach (var e in grassList)
        {
            e.grassMeshRender.meshRenderer.gameObject.SetActive(!context.isBattleFlag);

        }
        BattleFlag = context.isBattleFlag;
    }
    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        
        return context.CreateCollector(GameMatcher.BattleFlag);
    }

    public void EndBattleEvent()
    {
        if (BattleFlag == context.isBattleFlag) return;

        var grassList = context.GetEntities(GameMatcher.GrassMeshRender);

        foreach (var e in grassList)
        {
            e.grassMeshRender.meshRenderer.gameObject.SetActive(!context.isBattleFlag);

        }
        BattleFlag = context.isBattleFlag;
    }
}
