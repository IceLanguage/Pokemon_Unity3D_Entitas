using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 毅力（特性）
    /// </summary>
    class Guts :AbilityImpact
    {
        public override void OnAttack(BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon, ref float power, ref float Critical)
        {
            if (SkillType.物理 == skill.type)
            {
                if (AbnormalStateEnum.Burns == self.Abnormal)
                {
                    power *= 3f;
                    DebugHelper.LogFormat("{0}具有毅力特性,灼伤时攻击提升50%，并无视灼伤的攻击减半效果",self.Ename);
                }

                else if (AbnormalStateEnum.Paralysis == self.Abnormal||
                         AbnormalStateEnum.Poisoning == self.Abnormal||
                         AbnormalStateEnum.BadlyPoison == self.Abnormal||
                         AbnormalStateEnum.Sleeping == self.Abnormal)
                {
                    power *= 1.5f;
                    DebugHelper.LogFormat("{0}具有毅力特性,麻痹、中毒、睡眠时攻击提升50%", self.Ename);
                }
               
            }
        }
    }
}
