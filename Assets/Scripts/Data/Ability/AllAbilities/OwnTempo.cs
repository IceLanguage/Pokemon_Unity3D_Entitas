using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 我行我素（特性）
    /// </summary>
    class OwnTempo :AbilityImpact
    {
        public override void OnGetChangeState(BattlePokemonData self, ChangeStateEnumForPokemon newstate, ref bool canAddState)
        {
            if(ChangeStateEnumForPokemon.Confusion == newstate)
            {
                canAddState = false;
                DebugHelper.LogFormat("{0}具有我行我素特性，不会陷入混乱状态",self.Ename);
            }
           
        }
    }
}
