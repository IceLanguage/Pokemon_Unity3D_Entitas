using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    class UseNetBall : UsePokemonBall
    {
        public UseNetBall() : base(1, "捕网球")
        {

        }
        protected override void ChangeCatachCorrection(BattlePokemonData pokemon)
        {
            if (PokemonType.水 == pokemon.MainPokemonType ||PokemonType.水 == pokemon.SecondPokemonType)
            {
                DebugHelper.LogFormat("野生精灵{0}属性为水,捕网球发挥了奇效，捕捉概率提高了200%", pokemon.Ename);
                CatachCorrection = 3;
            }
            else if(  PokemonType.虫 == pokemon.MainPokemonType || PokemonType.虫 == pokemon.SecondPokemonType)
            {
                DebugHelper.LogFormat("野生精灵{0}属性为虫,捕网球发挥了奇效，捕捉概率提高了200%", pokemon.Ename);
                CatachCorrection = 3;
               
            }
                
            else
            {
                DebugHelper.LogFormat("野生精灵{0}属性没有虫和水,使用捕网球和普通精灵球没有区别", pokemon.Ename);
                CatachCorrection = 1;
            }
                
        }
    }
}
