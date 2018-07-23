using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

class UpdatePokemonDataSystem : ReactiveSystem<GameEntity>
{
    readonly GameContext context;
    public UpdatePokemonDataSystem(Contexts contexts):base(contexts.game)
    {
        context = contexts.game;
    }
    protected override void Execute(List<GameEntity> entities)
    {    
        foreach(GameEntity entity in entities)
        {
            if(entity.hasPokemonDataChangeEvent)
            {
                entity.pokemonDataChangeEvent.Event();
            }
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasBattlePokemonData&& null != entity.battlePokemonData;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BattlePokemonData);
    }
}
