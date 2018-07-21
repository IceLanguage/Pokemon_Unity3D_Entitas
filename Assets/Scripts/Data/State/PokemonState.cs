using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    
    /// <summary>
    /// 精灵状态
    /// </summary>
     public class PokemonState
    {
        

        

        ////对战中发生的事
        //public virtual void UpdateInBattleAround(BattlePokemonData pokemon) { }

        //非对战阶段发生的事
        public virtual void UpdateInPlayerAround(BattlePokemonData pokemon) { }

        //失去该状态发生的事，在PokemonState内调用
        public virtual void LoseState(BattlePokemonData pokemon) { }

        //判断状态是否影响了行动
        public virtual bool CanAction(BattlePokemonData pokemon)
        {
            return true;
        }

        //进入状态时发生的变化，如果状态不允许进入可在这一步恢复状态
        public virtual void Init(BattlePokemonData pokemon)
        {
            
        }

    }

    

}
