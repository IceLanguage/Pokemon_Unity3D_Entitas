using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 精神力（特性）
    /// </summary>
    class InnerFocus :AbilityImpact
    {
        public override void OnGetChangeState(BattlePokemonData self, ChangeStateEnumForPokemon newstate,
            ref bool canAddState)
        {
            if(ChangeStateEnumForPokemon.Flinch == newstate)
            {
                canAddState = false;
                DebugHelper.LogFormat("{0}具有精神力特性，不会陷入畏缩状态", self.Ename);
            }
        }
    }
}
