using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    abstract class ChangeStateForPokemonEffect : EffectWithProbability
    {
        
        public ChangeStateForPokemonEffect(int probability,bool isUseSelf) : base(probability, isUseSelf)
        {

        }
    }

    class ConfusionEffect: ChangeStateForPokemonEffect
    {
        public ConfusionEffect(int probability) : base(probability,false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.AddChangeState(ChangeStateEnumForPokemon.Confusion);
        }
    }

    class FlinchEffect : ChangeStateForPokemonEffect
    {
        public FlinchEffect(int probability) : base(probability,false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {

        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.AddChangeState(ChangeStateEnumForPokemon.Flinch);
        }
    }


    class RockWreckerEffect : ChangeStateForPokemonEffect
    {
        private readonly int skillID;
        
        public RockWreckerEffect(int skillID,int probability) : base(probability,true)
        {
            this.skillID = skillID;

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            NeedReplaceSKill.context[pokemon.ID] = skillID;
            pokemon.AddChangeState(ChangeStateEnumForPokemon.RockWrecker);
        }
    }

    class CanNotEscapeEffect : ChangeStateForPokemonEffect
    {
        public readonly int time;
        public CanNotEscapeEffect(int probability,int time) : base(probability,false)
        {
            this.time = time;

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            CanNotEscapeState.ifCanScape[pokemon.ID] = time;
            pokemon.AddChangeState(ChangeStateEnumForPokemon.RockWrecker);
        }
    }

    class WaitNextAroundEffect : ChangeStateForPokemonEffect
    {
        public readonly int skillID;
        public static Dictionary<int, int> wait = new Dictionary<int, int>();
        public WaitNextAroundEffect(int skillID, int probability) : base(probability,true)
        {
            this.skillID = skillID;

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            if (!wait.ContainsKey(pokemon.ID))
            {
                NeedReplaceSKill.context[pokemon.ID] = skillID;
                pokemon.AddChangeState(ChangeStateEnumForPokemon.WaitNextAround);
                canEnd = true;
                wait[pokemon.ID] = 1;
            }
            else if (wait.ContainsKey(pokemon.ID) &&
               NeedReplaceSKill.context.ContainsKey(pokemon.ID))
            {
                NeedReplaceSKill.context.Remove(pokemon.ID);
                wait.Remove(pokemon.ID);
            }
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
           
        }
    }
}
