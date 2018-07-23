using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace PokemonBattele
{
    public abstract class UseBagItem 
    {
        public readonly bool canUseInBattle;
        public readonly bool canUseOutBattle;
        public readonly bool NotNeedUseButEffect;
        public readonly bool UseWhilePokemonCarry;
        public UseBagItem(bool canUseInBattle,bool canUseOutBattle,bool NotNeedUseButEffect,bool UseWhilePokemonCarry)
        {
            this.canUseInBattle = canUseInBattle;
            this.canUseOutBattle = canUseOutBattle;
            this.NotNeedUseButEffect = NotNeedUseButEffect;
            this.UseWhilePokemonCarry = UseWhilePokemonCarry;
        }
        public abstract void Effect();
    }
    
}
