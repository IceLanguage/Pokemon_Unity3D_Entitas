using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 茂盛（特性）
    /// </summary>
    class Overgrow :AbilityImpact
    {
        public override void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref float power,ref float Critical)
        {
            if(self.curHealth<self.Health/3)
                if (PokemonType.草 == skill.att)
                {
                    DebugHelper.LogFormat("因为茂盛特性{0}的技能{1}的威力威力提高为原来的1.5倍", self.Ename, skill.sname);
                    power *= 1.5f;
                }
                    
        }
    }
}
