using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 再生力（特性）
    /// </summary>
    class Regenerator :AbilityImpact
    {
        public override void OnLeave(BattlePokemonData self)
        {
            int h = self.Health / 3;
            h = Math.Min(h, self.Health - self.curHealth);
            self.curHealth += h;
            DebugHelper.Log(
                new StringBuilder(40)
                .AppendFormat("{0}具有再生力特性，交换下场时，恢复", self.Ename)
                .Append(h)
                .Append("生命")
                .ToString());
        }
    }
}
