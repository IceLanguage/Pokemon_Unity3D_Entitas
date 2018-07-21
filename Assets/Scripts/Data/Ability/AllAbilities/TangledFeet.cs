using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 蹒跚（特性）
    /// </summary>
    class TangledFeet:AbilityImpact
    {
        public override void OnBeAttacked(
            BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
            ref int avoid)
        {
            avoid *= 2;
            DebugHelper.LogFormat("{0}处于混乱状态，因为蹒跚特性，闪避率提升100%", self.Ename);
        }
    }
}
