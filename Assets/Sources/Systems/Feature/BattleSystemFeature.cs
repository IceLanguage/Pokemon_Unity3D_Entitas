using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BattleSystemFeature:Feature
{
    public BattleSystemFeature(Contexts contexts)
    {
        Add(new PokemonDeathReactiveSystem(contexts));
        Add(new EndBattleSystem(contexts));
        Add(new BeginBattleSystem(contexts));
        Add(new UpdatePokemonDataSystem(contexts));
    }
}
