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
            if (PokemonType.水 == pokemon.MainPokemonType ||PokemonType.水 == pokemon.SecondPokemonType||
                PokemonType.虫 == pokemon.MainPokemonType || PokemonType.虫 == pokemon.SecondPokemonType)
                CatachCorrection = 3;
            else
                CatachCorrection = 1;
        }
    }
}
