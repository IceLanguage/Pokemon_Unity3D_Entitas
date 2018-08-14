using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    abstract class RecoverEffect : EffectWithProbability
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
        public override void BattleStartEffect(BattlePokemonData pokemon, ref bool canEnd)
        {
            canEnd = true;
            float scale = recoverValue / 100f;

            int h = (int)(pokemon.Health * scale);
            h = Math.Min(h, pokemon.Health - pokemon.curHealth);
            DebugHelper.Log(
              new StringBuilder(40)
              .AppendFormat("{0}的生命恢复了", pokemon.Ename)
              .Append(h)
              .ToString());
            
            pokemon.curHealth += h;
            if (pokemon.curHealth > pokemon.Health)
                pokemon.curHealth = pokemon.Health;
        }


    }
}
