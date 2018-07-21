using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 免疫（特性）
    /// </summary>
    class Immunity :AbilityImpact
    {
        public override void OnGetAbormal(BattlePokemonData self, AbnormalStateEnum newstate, ref bool canGet)
        {
            if(AbnormalStateEnum.Poisoning ==newstate ||AbnormalStateEnum.BadlyPoison == newstate)
            {
                canGet = false;
                DebugHelper.LogFormat("{0}具有免疫特性,不会陷入中毒或剧毒状态");
            }
        }
    }
}
