using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 魔法防守（特性）
    /// </summary>
    class MagicGuard :AbilityImpact

    {
        public override void OnAbnomalCauseDamage(BattlePokemonData self, ref int damage)
        {
            damage = 0;
            DebugHelper.LogFormat("{0}因为具有魔法防守特性，异常状态不会造成伤害", self.Ename);
        }

        public override void OnChangeStateCauseDamage(BattlePokemonData self, ref int damage)
        {
            damage = 0;
            DebugHelper.LogFormat("{0}因为具有魔法防守特性，混乱以外的状态变化不会造成伤害", self.Ename);
        }
    }
}
