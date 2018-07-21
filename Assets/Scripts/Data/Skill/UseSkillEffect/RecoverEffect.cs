using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    abstract class RecoverEffect : UseSkillEffectWithProbability
    {
        protected int recoverValue;
        public RecoverEffect(int probability,int recoverValue) : base(probability,true)
        {
            this.recoverValue = recoverValue;
        }
    }

    class RecoverScaleHealthEffect : RecoverEffect
    {
        public RecoverScaleHealthEffect(int probability, int recoverValue) : base(probability,recoverValue)
        {
            
        }

        public override void Effect(BattlePokemonData pokemon)
        {
            float scale = recoverValue / 100f;

            int h = (int)(pokemon.Health * scale);
            h = Math.Min(h, pokemon.Health - pokemon.curHealth);
            DebugHelper.LogFormat("{0}的生命恢复了{1}",pokemon.Ename,h);
            pokemon.curHealth += h;
            if (pokemon.curHealth > pokemon.Health)
                pokemon.curHealth = pokemon.Health;
        }

    }
}
