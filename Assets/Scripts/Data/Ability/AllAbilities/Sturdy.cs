using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 结实（特性）
    /// </summary>
    class Sturdy:AbilityImpact
    {
        public override void OnSkillCauseDamage(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref int damage)
        {
            if(damage>=self.Health&&self.curHealth== self.Health)
            {
                damage = self.Health - 1;
                DebugHelper.LogFormat("{0}具有结实特性，在满ＨＰ状态下被攻击时，至少会保留1点ＨＰ");
            }
        }
    }
}
