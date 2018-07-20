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
            DebugHelper.Log(scale);
            DebugHelper.Log(pokemon.Health * scale);
            int h = (int)(pokemon.Health * scale);
            DebugHelper.LogFormat("{0}的生命恢复了{1}",pokemon.Ename,h);
            pokemon.curHealth += h;
            if (pokemon.curHealth > pokemon.Health)
                pokemon.curHealth = pokemon.Health;
        }

    }
}
