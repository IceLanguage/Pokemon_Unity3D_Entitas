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
                CatachCorrection = 5;
            else
                CatachCorrection = 1;
        }
    }
}
