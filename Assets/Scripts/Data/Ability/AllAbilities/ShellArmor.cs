using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 硬壳盔甲（特性）
    /// </summary>
    class ShellArmor:AbilityImpact
    {
        public override void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref bool hit,ref float resDamage)
        {
            hit = false;
            DebugHelper.LogFormat("{0}具有硬壳盔甲特性，不会被击中要害,必定命中要害的招式也不会击中要害", self.Ename);
        }
    }
}
