using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 狙击手（特性）
    /// </summary>
    class Sniper :AbilityImpact
    {
        public override void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref float power, ref float Critical)
        {

            Critical = 2.25f;
            DebugHelper.LogFormat("{0}拥有狙击手特性，一旦击中要害，威力是正常的1.5倍", self.Ename);
        }
    }
}
