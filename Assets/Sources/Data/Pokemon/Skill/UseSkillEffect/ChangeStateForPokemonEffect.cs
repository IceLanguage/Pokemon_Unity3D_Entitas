using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    abstract class ChangeStateForPokemonEffect : UseSkillEffectWithProbability
    {
        public ChangeStateForPokemonEffect(int probability) : base(probability)
        {

        }
    }

    class ConfusionEffect: ChangeStateForPokemonEffect
    {
        public ConfusionEffect(int probability) : base(probability)
        {

        }

        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.AddChangeState(ChangeStateEnumForPokemon.Confusion);
            }
            
           
        }
    }
}
