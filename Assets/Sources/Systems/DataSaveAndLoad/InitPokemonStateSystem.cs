using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;

class InitPokemonStateSystem : IInitializeSystem
{
    public void Initialize()
    {
        PokemonState.Abnormalstates[AbnormalState.Normal] = new PokemonState();
        PokemonState.Abnormalstates[AbnormalState.Burns] = new BurnsState();
        PokemonState.Abnormalstates[AbnormalState.Frostbite] = new FrostbiteState();
        PokemonState.Abnormalstates[AbnormalState.Paralysis] = new ParalysisState();
        PokemonState.Abnormalstates[AbnormalState.Poisoning] = new PoisonState();
        PokemonState.Abnormalstates[AbnormalState.Sleeping] = new SleepState();
    }
}
