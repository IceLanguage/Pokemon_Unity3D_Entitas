using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 威吓（特性）
    /// </summary>
    class Intimidate :AbilityImpact
    {
        public override void OnBeCalled(BattlePokemonData self)
        {
            if(BattleController.Instance.PlayerCurPokemonData == self)
            {
                StatModifiers stat = BattleController.Instance.EnemyCurPokemonData.StatModifiers;
                stat.PhysicPower -= 1;
                BattleController.Instance.EnemyCurPokemonData.StatModifiers = stat;
                DebugHelper.LogFormat("{0}具有威吓特性，{1}攻击下降1级", self.Ename, BattleController.Instance.EnemyCurPokemonData.Ename);
            }
            else if (BattleController.Instance.EnemyCurPokemonData == self)
            {
                StatModifiers stat = BattleController.Instance.PlayerCurPokemonData.StatModifiers;
                stat.PhysicPower -= 1;
                BattleController.Instance.PlayerCurPokemonData.StatModifiers = stat;
                DebugHelper.LogFormat("{0}具有威吓特性，{1}攻击下降1级", self.Ename, BattleController.Instance.PlayerCurPokemonData.Ename);
            }
        }
    }
}
