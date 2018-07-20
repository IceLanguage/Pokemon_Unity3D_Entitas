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
        public static Dictionary<int, List<IUseSkillSpecialEffect>> UseSkillDic =
            new Dictionary<int, List<IUseSkillSpecialEffect>>();
    }

    public abstract class UseSkillEffectWithProbability : IUseSkillSpecialEffect
    {
        protected readonly int probability;
        public UseSkillEffectWithProbability(int probability)
        {
            this.probability = probability;
        }
        public virtual void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
                Effect(pokemon);
            else if(-1 == probability)
                Effect(pokemon);
        }

        public abstract void Effect(BattlePokemonData pokemon);
    }
}
