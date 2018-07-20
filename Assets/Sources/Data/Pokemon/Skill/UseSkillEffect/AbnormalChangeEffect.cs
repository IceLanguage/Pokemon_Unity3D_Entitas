using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 麻痹效果
    /// </summary>
    class ParalysisEffect : UseSkillEffectWithProbability
    {
        public ParalysisEffect(int probability):base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Paralysis);
        }
    }

    class BurnsEffect : UseSkillEffectWithProbability
    {
        public BurnsEffect(int probability) : base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Burns);
        }


    }

    class FrostbiteEffect : UseSkillEffectWithProbability
    {
        public FrostbiteEffect(int probability) : base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Frostbite);
        }


    }

    class PoisonEffect : UseSkillEffectWithProbability
    {
        public PoisonEffect(int probability) : base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Poisoning);
        }


    }

    class VeryToxicEffect : UseSkillEffectWithProbability
    {
        public VeryToxicEffect(int probability) : base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.BadlyPoison);
        }


    }

    class SleepEffect : UseSkillEffectWithProbability
    {
        public SleepEffect(int probability) : base(probability)
        {

        }

        public override void Effect(BattlePokemonData pokemon)
        {
            pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Sleeping);
        }

    }
}
