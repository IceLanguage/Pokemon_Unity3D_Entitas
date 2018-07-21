using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 飞毛腿（特性）
    /// </summary>
    class QuickFeet:AbilityImpact
    {
        public override void OnGetAbormal(BattlePokemonData self, AbnormalStateEnum newstate,ref bool canGet)
        {

            if (AbnormalStateEnum.Normal != newstate)
            {
                if (AbnormalStateEnum.Normal == self.Abnormal)
                {
                    self.increase.speed *= 1.5f;
                    DebugHelper.LogFormat("{0}具有飞毛腿特性，陷入异常状态时速度提升50%", self.Ename);
                }
            }
            else
            {
                if (AbnormalStateEnum.Normal != self.Abnormal)
                {
                    self.increase.speed /= 1.5f;
                    DebugHelper.LogFormat("{0}具有飞毛腿特性，异常状态恢复速度恢复", self.Ename);
                }
            }
        }
    }
}
