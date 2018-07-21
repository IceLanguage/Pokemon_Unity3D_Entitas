using PokemonBattele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{ 
    /// <summary>
    /// 健壮胸肌（特性）
    /// </summary>
    class BigPecks :AbilityImpact
    {
        public override void OnStatModifiersChange(BattlePokemonData self, ref StatModifiers newvalue)
        {
            if (newvalue.PhysicDefence < self.StatModifiers.PhysicDefence)
            {
                newvalue.PhysicDefence = self.StatModifiers.PhysicDefence;
                DebugHelper.LogFormat("{0}具有健壮胸肌特性，防御能力不会下降", self.Ename);
            }
        }
    }
}
