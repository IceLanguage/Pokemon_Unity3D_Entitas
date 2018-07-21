using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 引火（特性）
    /// </summary>
    class FlashFire :AbilityImpact
    {
        public static Dictionary<int, bool> can = new Dictionary<int, bool>();
        public override void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref bool IsIngnore)
        {
            if(PokemonType.火 == skill.att)
            {
                can[self.ID] = true;
                IsIngnore = true;
                DebugHelper.LogFormat("{0}具有引火特性,被火属性招式{1}击中，引火特性被激发", self.Ename,skill.sname);
            }
        }
        public override void OnAttack(BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon, ref float power, ref float Critical)
        {
            bool fire =false;
            if(PokemonType.火 == skill.att)
            {
                if (can.TryGetValue(self.ID, out fire))
                {
                    if(fire)
                    {
                        power *= 1.5f;
                        DebugHelper.LogFormat("{0}已处于引火状态，使用火属性攻击招式时，攻击和特攻×1.5", self.Ename);
                    }
                }
            }
            
        }
        public override void OnLeave(BattlePokemonData self)
        {
            bool fire = false;
            if (can.TryGetValue(self.ID, out fire))
            {
                if (fire)
                {
                    can.Remove(self.ID);
                    DebugHelper.LogFormat("{0}下场，引火状态取消", self.Ename);
                }
            }
        }
    }
}
