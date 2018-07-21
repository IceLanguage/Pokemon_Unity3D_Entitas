using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 激流（特性）
    /// </summary>
    class Torrent : AbilityImpact
    {
        public override void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref float power, ref float Critical)
        {
            if (self.curHealth < self.Health / 3)
                if (PokemonType.水 == skill.att)
                {
                    DebugHelper.LogFormat("因为激流特性{0}的水属性技能{1}的威力提高为原来的1.5倍", self.Ename, skill.sname);
                    power *= 1.5f;
                }

        }
    }
}
