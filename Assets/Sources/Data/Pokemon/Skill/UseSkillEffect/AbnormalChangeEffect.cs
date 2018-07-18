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
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if(RandomService.game.Int(0,100)< probability)
            {
                pokemon.SetAbnormalState(AbnormalState.Paralysis);
            }
        }
    }

    class BurnsEffect : UseSkillEffectWithProbability
    {
        public BurnsEffect(int probability) : base(probability)
        {

        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.SetAbnormalState(AbnormalState.Burns);
            }
        }
    }

    class FrostbiteEffect : UseSkillEffectWithProbability
    {
        public FrostbiteEffect(int probability) : base(probability)
        {

        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.SetAbnormalState(AbnormalState.Frostbite);
            }
        }
    }

    class PoisonEffect : UseSkillEffectWithProbability
    {
        public PoisonEffect(int probability) : base(probability)
        {

        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.SetAbnormalState(AbnormalState.Poisoning);
            }
        }
    }

    class SleepEffect : UseSkillEffectWithProbability
    {
        public SleepEffect(int probability) : base(probability)
        {

        }
        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                pokemon.SetAbnormalState(AbnormalState.Sleeping);
            }
        }
    }
}
