using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 诅咒之躯（特性）
    /// </summary>
    class CursedBody :AbilityImpact
    {
        public override void OnBeAttacked(ref bool hit, BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon)
        {
            int skillID = attackPokemon.LastUseSkillID;
            if (hit && -1 != skillID)
            {
                if (RandomService.game.Int(0, 100) < 30)
                {
                    string skillName = ResourceController.Instance.allSkillDic[skillID].sname;
                    
                    attackPokemon.AddChangeState(ChangeStateEnumForPokemon.DisableSkill);

                    DebugHelper.LogFormat("{1}触发了{0}的诅咒之躯特性,{2}被封印了",
                        attackPokemon.Ename,self.Ename, skillName);
                }
            }
        }
    }
}
