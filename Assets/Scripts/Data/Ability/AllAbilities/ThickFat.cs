using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 厚脂肪（特性）
    /// </summary>
    class ThickFat :AbilityImpact
    {
        public override void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref bool hit, ref float resDamage)
        {
            if(PokemonType.冰 == skill.att||PokemonType.火 == skill.att)
            {
                resDamage /= 2;
                DebugHelper.LogFormat("{0}具有厚脂肪特性，受到冰属性或火属性招式攻击时，在伤害计算中对方的攻击或特攻减半", self.Ename);
            }
        }
    }
}
