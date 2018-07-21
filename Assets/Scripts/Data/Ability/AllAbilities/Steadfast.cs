using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 不屈之心（特性）
    /// </summary>
    class Steadfast :AbilityImpact
    {
        public override void OnGetChangeState(BattlePokemonData self, ChangeStateEnumForPokemon newstate, ref bool canAddState)
        {
            if(ChangeStateEnumForPokemon.Flinch == newstate)
            {
                StatModifiers stat = self.StatModifiers;
                stat.Speed += 1;
                self.StatModifiers = stat;
                DebugHelper.LogFormat("{0}具有不屈之心特性，陷入畏缩状态后速度提升1级", self.Ename);
            }
        }
    }
}
