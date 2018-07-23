﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    class UseTimerBall : UsePokemonBall
    {
        public UseTimerBall() : base(1, "计时球")
        {

        }
        protected override void ChangeCatachCorrection(BattlePokemonData pokemon)
        {
            int around = BattleController.Instance.BattleAroundCount;
            if (10 <= around )
                CatachCorrection = 4;
            else
                CatachCorrection = 1 + 0.3f*around;
        }
    }
}
