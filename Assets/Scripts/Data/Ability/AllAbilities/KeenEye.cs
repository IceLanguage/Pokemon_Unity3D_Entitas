using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 锐利目光（特性）
    /// </summary>
    class KeenEye :AbilityImpact
    {
        public override void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref int HitRateB)
        {
            HitRateB = self.StatModifiers.CriticalHit;
            if (defencePokemon.StatModifiers.HitRate < 0)
                HitRateB -= defencePokemon.StatModifiers.HitRate;
            DebugHelper.LogFormat("{0}具有锐利目光特性，无视对方闪避率能力阶级的提升",self.Ename);
        }

        public override void OnStatModifiersChange(BattlePokemonData self, ref StatModifiers newvalue)
        {
            if(newvalue.HitRate<self.StatModifiers.HitRate)
            {
                newvalue.HitRate = self.StatModifiers.HitRate;
                DebugHelper.LogFormat("{0}具有锐利目光特性，命中率能力阶级不会被对方以任何方式降低", self.Ename);
            }
        }
    }
}
