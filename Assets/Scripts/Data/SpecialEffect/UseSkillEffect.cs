using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    public interface ISpecialEffect
    {
        void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd);
        void SpecialEffect(BattlePokemonData pokemon);
        void HitEffect(BattlePokemonData pokemon);
    }
    public static partial class UseSkillEffectManager
    {
        public static Dictionary<int, List<EffectWithProbability>> UseSkillDic =
            new Dictionary<int, List<EffectWithProbability>>();
    }

    public abstract class EffectWithProbability : ISpecialEffect
    {
        protected readonly int probability;
        public readonly bool isUseSelf;
        public EffectWithProbability(int probability,bool isUseSelf)
        {
            this.probability = probability;
            this.isUseSelf = isUseSelf;
        }
       
        public virtual void SpecialEffect(BattlePokemonData pokemon)
        {
            
            if (-1 == probability)
                HitEffect(pokemon);
            else if (RandomService.game.Int(0, 100) < probability)
                HitEffect(pokemon);
           
        }
        public virtual void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd) { }
        public virtual void HitEffect(BattlePokemonData pokemon) { }
        protected string Log(string pokemonName, string s, int num)
        {
            return new StringBuilder(40)
              .AppendFormat("{0}{1}", pokemonName, s)
              .Append(num)
              .ToString();
        }
    }
}
