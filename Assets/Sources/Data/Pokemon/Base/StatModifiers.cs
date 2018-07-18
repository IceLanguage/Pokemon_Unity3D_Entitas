using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 能力阶级
    /// </summary>
    public class StatModifiers:PokemonBaseData
    {
        [SerializeField]
        public new int health = 0;
        [SerializeField]
        protected new int physicPower = 0;
        [SerializeField]
        protected new int physicDefence = 0;
        [SerializeField]
        protected new int energyPower = 0;
        [SerializeField]
        protected new int energyDefence = 0;
        [SerializeField]
        protected new int speed = 0;

        public readonly static Dictionary<int, int> ActualCorrection = new Dictionary<int, int>()
        {
            {-6,25},
            {-5,29},
            {-4,33},
            {-3,40},
            {-2,50},
            {-1,67},
            { 0,100},
            { 1,150},
            { 2,200},
            { 3,250},
            { 4,300},
            { 5,350},
            { 6,400}
        };
        /// <summary>
        /// 设置努力值
        /// 防止努力值超出范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentSetBase"></param>
        /// <returns></returns>
        private int Set(int value)
        {
            if (value < -6) return -6;
            if (value > 6) return 6;
            return value;
        }

        public override int Health
        {
            get
            {
                return ActualCorrection[health];
            }

            set
            {
                health = Set(value);
            }
        }

        public override int PhysicPower
        {
            get
            {
                return physicPower;
            }

            set
            {
                physicPower = Set(value);
            }
        }

        public override int PhysicDefence
        {
            get
            {
                return physicDefence;
            }

            set
            {
                physicDefence = Set(value);
            }
        }

        public override int EnergyPower
        {
            get
            {
                return energyPower;
            }

            set
            {
                energyPower = Set(value);
            }
        }

        public override int EnergyDefence
        {
            get
            {
                return energyDefence;
            }

            set
            {
                energyDefence = Set(value);
            }
        }

        public override int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = Set(value);
            }
        }
    }
}
