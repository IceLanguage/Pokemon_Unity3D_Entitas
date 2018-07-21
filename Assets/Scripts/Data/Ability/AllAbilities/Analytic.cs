using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 分析（特性）
    /// </summary>
    class Analytic:AbilityImpact
    {
        public override void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref float power, ref float Critical)
        {
            if(BattleController.Instance.FirstHand != self.ID)
            {
                power *= 1.3f;
                DebugHelper.LogFormat("{0}具有分析特性, 所有宝可梦的行动都在自己之前，使用的招式{1}的威力提高30 %",
                self.Ename, skill.sname);
            }
            
        }
    }
}
