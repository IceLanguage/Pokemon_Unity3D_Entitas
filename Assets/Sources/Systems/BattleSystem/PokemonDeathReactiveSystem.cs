using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using MyUnityEventDispatcher;

class PokemonDeathReactiveSystem : ReactiveSystem<GameEntity>
{
    readonly GameContext context;
    public PokemonDeathReactiveSystem(Contexts contexts):base(contexts.game)
    {
        context = contexts.game;
    }
    protected override void Execute(List<GameEntity> entities)
    {
        foreach(GameEntity entity in entities)
        {
            NotificationCenter<int>.Get().DispatchEvent("PokemonDeathMessage"
                , entity.battlePokemonData.data.ID);
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return context.isBattleFlag&&
            entity.hasBattlePokemonData && 
            0 == entity.battlePokemonData.data.curHealth;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BattlePokemonData);
    }
}
