using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    class UseFastBall:UsePokemonBall
    {
        public UseFastBall():base(1, "速度球")
        {

        }
        protected override void ChangeCatachCorrection(BattlePokemonData pokemon)
        {
            if (pokemon.race.speed >= 100)
                CatachCorrection = 4;
            else
                CatachCorrection = 1;
        }
    }
}
