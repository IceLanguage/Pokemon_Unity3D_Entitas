using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    class UseQuickBall : UsePokemonBall
    {
        public UseQuickBall() : base(1, "先机球")
        {

        }
        protected override void ChangeCatachCorrection(BattlePokemonData pokemon)
        {
            if (1 == BattleController.Instance.BattleAroundCount)
            {
                CatachCorrection = 5;
                DebugHelper.Log("你是第一回合使用先机球，捕捉概率提高了400%");
            }
                
            else
            {
                CatachCorrection = 1;
                DebugHelper.Log("你不是第一回合使用先机球，使用先机球和普通精灵球没有区别");
            }
                
        }
    }
}
