using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 自信过度（特性）
    /// </summary>
    class Moxie :AbilityImpact
    {
        public override void OnDefeatPokemon(BattlePokemonData self, BattlePokemonData defeatPokemon)
        {
            StatModifiers stat = self.StatModifiers;
            stat.PhysicPower += 1;
            self.StatModifiers = stat;
            DebugHelper.LogFormat("{0}击败了{1}，自信过度特性激发，自身攻击提升1级",
                self.Ename, defeatPokemon.Ename);
        }
    }
}
