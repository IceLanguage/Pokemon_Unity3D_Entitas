using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 无防守（特性）
    /// </summary>
    class NoGuard :AbilityImpact
    {
        public override void OnAttack(ref bool hit, BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon)
        {
            hit = true;
            DebugHelper.LogFormat("{0}具有无防御特性，攻击一定命中",self.Ename);
        }
        public override void OnBeAttacked(ref bool hit, BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon)
        {
            hit = true;
            DebugHelper.LogFormat("{0}具有无防御特性，一定会被命中", self.Ename);
        }
    
    }
}
