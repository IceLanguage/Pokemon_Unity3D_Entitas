using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LHCoroutine;
using System.Collections;
using UnityEngine;

public class EndBattleSystem : ReactiveSystem<GameEntity>
{
    private readonly GameContext context;
    public static Action EndBattleEvent;
   
    public EndBattleSystem(Contexts contexts) : base(contexts.game)
    {
        context = contexts.game;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        CoroutineManager.DoCoroutine(PreEndBattle());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BattleFlag);
    }
   

    IEnumerator PreEndBattle()
    {
        yield return new WaitWhile(() =>
        {
            return context.isBattleFlag;
        });
        EndBattleEvent();
    }

}
