using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

class HideGrassSystem : IEndBattleEvent,IInitializeSystem
{
    private readonly GameContext context;
    public HideGrassSystem(Contexts contexts)
    {
        context = contexts.game;
        
    }
   
    

    public void EndBattleEvent()
    {


        var grassList = context.GetEntities(GameMatcher.GrassMeshRender);

        foreach (var e in grassList)
        {
            e.grassMeshRender.meshRenderer.gameObject.SetActive(!context.isBattleFlag);

        }
    }

    public void HideGlassLand()
    {
        var grassList = context.GetEntities(GameMatcher.GrassMeshRender);

        foreach (var e in grassList)
        {
            e.grassMeshRender.meshRenderer.gameObject.SetActive(!context.isBattleFlag);

        }
    }

    public void Initialize()
    {
        EndBattleSystem.EndBattleEvent += EndBattleEvent;
        BeginBattleSystem.BeginBattleEvent += HideGlassLand;
    }
}
