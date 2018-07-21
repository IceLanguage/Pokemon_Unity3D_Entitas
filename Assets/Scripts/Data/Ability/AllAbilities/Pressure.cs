using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 压迫感（特性）
    /// </summary>
    class Pressure :AbilityImpact
    {
        public override void OnEnemySkillUse(BattlePokemonData attackPokemon, int skillIndex, BattlePokemonData self)
        {
            string skillName = ResourceController.Instance.allSkillDic[attackPokemon.skills[skillIndex]].sname;
            DebugHelper.LogFormat("{0}具有压迫感特性,{1}的技能{2}多损失一点PP", self.Ename, attackPokemon.Ename,skillName);
            if(0< attackPokemon.skillPPs[skillIndex])
                attackPokemon.skillPPs[skillIndex] -= 1;

        }
    }
}
