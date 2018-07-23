using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 麻痹效果
    /// </summary>
    class ParalysisEffect : EffectWithProbability
    {
        public ParalysisEffect(int probability):base(probability,false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
           
        }


        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Paralysis);
        }
    }

    class BurnsEffect : EffectWithProbability
    {
        public BurnsEffect(int probability) : base(probability, false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Burns);
        }
    }

    class FrostbiteEffect : EffectWithProbability
    {
        public FrostbiteEffect(int probability) : base(probability, false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }


        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Frostbite);
        }
    }

    class PoisonEffect : EffectWithProbability
    {
        public PoisonEffect(int probability) : base(probability, false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }



        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Poisoning);
        }
    }

    class VeryToxicEffect : EffectWithProbability
    {
        public VeryToxicEffect(int probability) : base(probability, false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
           
        }
        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.BadlyPoison);
        }
    }

    class SleepEffect : EffectWithProbability
    {
        public SleepEffect(int probability) : base(probability, false)
        {

        }

        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            
        }

        public override void HitEffect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Sleeping);
        }
    }
}
