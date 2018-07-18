using MyUnityEventDispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace PokemonBattele
{
    abstract class BattleState
    {
        public BattleState()
        {
            Init();
        }
        public abstract BattleState ChangeState();
        public abstract void Init();
    }

    class BattleStateForPlayer:BattleState
    {
        public static UnityAction InitEvent;
        public override BattleState ChangeState()
        {
            return new BattleStateForBattle();
        }

        public override void Init()
        {
            InitEvent();
        }
    }

    class BattleStateForBattle : BattleState
    {
        public static UnityAction InitEvent;
        public override BattleState ChangeState()
        {
            return new BattleStateForPlayer();
        }

        public override void Init()
        {
            InitEvent();
        }
    }
}
