using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 避雷针（特性）
    /// </summary>
    class LightningRod:AbilityImpact
    {
        public override void OnBeAttacked(
            BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
            ref bool IsIngnore)
        {
            if (PokemonType.电 == skill.att)
            {
                IsIngnore = true;
                attackPokemon.SetAbnormalStateEnum(AbnormalStateEnum.Paralysis);
                StatModifiers stat = self.StatModifiers;
                stat.EnergyPower += 1;
                self.StatModifiers = stat;
                DebugHelper.LogFormat("{0}具有避雷针特性，电属性技能对{0}无效，被电属性招式击中时，特攻提升1级", self.Ename);
            }
        }
    }
}
