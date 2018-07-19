using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    abstract class RecoverEffect : UseSkillEffectWithProbability
    {
        protected int recoverValue;
        public RecoverEffect(int probability,int recoverValue) : base(probability)
        {
            
        }
    }

    class RecoverScaleHealthEffect : RecoverEffect
    {
        public RecoverScaleHealthEffect(int probability, int recoverValue) : base(probability,recoverValue)
        {

        }

        public override void UseSkillSpecialEffect(BattlePokemonData pokemon)
        {
            if (RandomService.game.Int(0, 100) < probability)
            {
                float scale = recoverValue / 100f;
                pokemon.curHealth += (int)(pokemon.Health * scale);
                if (pokemon.curHealth > pokemon.Health)
                    pokemon.curHealth = pokemon.Health;
            }
            
        }
    }
}
