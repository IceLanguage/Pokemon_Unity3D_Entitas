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
        AbnormalState.Abnormalstates[AbnormalStateEnum.Normal] = new NormalAbnormalState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.Burns] = new BurnsState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.Frostbite] = new FrostbiteState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.Paralysis] = new ParalysisState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.Poisoning] = new PoisonState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.Sleeping] = new SleepState();
        AbnormalState.Abnormalstates[AbnormalStateEnum.BadlyPoison] = new BadlyPoisonState();

        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.Confusion]
            = new ConfusionState();
        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.Flinch]
           = new FlinchState();
        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.RockWrecker]
           = new RockWreckerState(true);
        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.CanNotEscape]
           = new CanNotEscapeState();
        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.WaitNextAround]
          = new WaitNextAroundState(true);
        ChangeStateForPokemon.ChangeStateForPokemons[ChangeStateEnumForPokemon.DisableSkill]
          = new DisableSkillState();
        
    }
}
