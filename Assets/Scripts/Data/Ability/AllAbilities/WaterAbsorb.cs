using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 储水（特性）
    /// </summary>
    class WaterAbsorb:AbilityImpact
    {
        public override void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref bool IsIngnore)
        {
            if(PokemonType.水 == skill.att)
            {
                IsIngnore = true;
                int h = self.Health / 4;
                h = Math.Min(h, self.Health - self.curHealth);
                self.curHealth += h;
                DebugHelper.LogFormat("{0}具有储水特性，不受水属性招式影响，当被水属性招式击中时，回复最大ＨＰ的1⁄4", self.Ename);
            }
        }
    }
}
