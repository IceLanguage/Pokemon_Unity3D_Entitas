using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    public interface IUseSkillSpecialEffect
    {
        void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd);
        void UseSkillSpecialEffect(BattlePokemonData pokemon);
        void HitEffect(BattlePokemonData pokemon);
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
                HitEffect(pokemon);
            else if (RandomService.game.Int(0, 100) < probability)
                HitEffect(pokemon);
           
        }
        public virtual void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd) { }
        public virtual void HitEffect(BattlePokemonData pokemon) { }
    }
}
