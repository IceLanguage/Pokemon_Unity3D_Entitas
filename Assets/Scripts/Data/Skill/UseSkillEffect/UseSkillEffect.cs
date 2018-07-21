using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    public interface IUseSkillSpecialEffect
    {
        void UseSkillSpecialEffect(BattlePokemonData pokemon);
    }
    public static partial class UseSkillEffectManager
    {
        public static Dictionary<int, List<UseSkillEffectWithProbability>> UseSkillDic =
            new Dictionary<int, List<UseSkillEffectWithProbability>>();
    }

    public abstract class UseSkillEffectWithProbability : IUseSkillSpecialEffect
    {
        protected readonly int probability;
        public readonly bool isUseSelf;
        public UseSkillEffectWithProbability(int probability,bool isUseSelf)
        {
            this.probability = probability;
            this.isUseSelf = isUseSelf;
        }
        public virtual void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (-1 == probability)
                Effect(pokemon);
            else if (RandomService.game.Int(0, 100) < probability)
                Effect(pokemon);
           
        }

        public abstract void Effect(BattlePokemonData pokemon);
    }
}
