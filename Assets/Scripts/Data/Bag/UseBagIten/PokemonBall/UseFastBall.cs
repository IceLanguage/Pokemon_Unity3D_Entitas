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
            {
                CatachCorrection = 4;
                DebugHelper.LogFormat("野生精灵{0}速度种族为{1},速度球发挥了奇效，捕捉概率提高了300%", pokemon.Ename, pokemon.race.speed);
            }
                
            else
            {
                CatachCorrection = 1;
                DebugHelper.LogFormat("野生精灵{0}速度种族为{1},小于100,使用速度球和普通精灵球没有区别", pokemon.Ename, pokemon.race.speed);
            }
                
        }
    }
}
