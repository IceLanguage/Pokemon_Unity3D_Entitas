using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 碎裂铠甲（特性）
    /// </summary>
    class WeakArmor :AbilityImpact
    {
        public override void OnBeAttacked(ref bool hit, BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon)
        {
            if(hit)
            {
                if(SkillType.物理 == skill.type)
                {
                    StatModifiers stat = self.StatModifiers;
                    stat.PhysicDefence -= 1;
                    stat.Speed += 2;
                    DebugHelper.LogFormat("{0}具有碎裂铠甲特性，受到物理攻击时，防御降低1级，速度提升2级", self.Ename);
                }
            }
        }
    }
}
