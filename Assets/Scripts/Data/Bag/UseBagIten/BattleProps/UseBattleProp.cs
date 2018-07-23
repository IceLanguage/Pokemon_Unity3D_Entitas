using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    class UseBattleProp : UseBagItem
    {
        private EffectWithProbability effect;
        public UseBattleProp(string BagItemName, EffectWithProbability effect) :
           base(true, false, false, false, effect.isUseSelf, BagItemName)
        {
            this.effect = effect;
        }
        public override void Effect(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("你选择使用了{0}", BagItemName);
            effect.SpecialEffect(pokemon);
        }
        
    }
}
