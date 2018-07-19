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
    public class StatModifiers
    {

        [SerializeField]
        protected int physicPower = 0;
        [SerializeField]
        protected int physicDefence = 0;
        [SerializeField]
        protected int energyPower = 0;
        [SerializeField]
        protected int energyDefence = 0;
        [SerializeField]
        protected int speed = 0;
        [SerializeField]
        protected int criticalHit_C = 0;

        //命中率
        public int HitRate = 0;

        //回避率
        public int AvoidanceRate = 0;

        //等级-修正的百分比
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

        //击中要害率 等级-修正
        public readonly static Dictionary<int, float> criticalHit_C_To_B = new Dictionary<int, float>()
        {
            { -1,0},
            { 0,1/16},
            { 1,1/8},
            { 2,1/4},
            { 3,1/3},
            { 4,1/2},
        };
        /// <summary>
        /// 设置基础属性修正
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentSetBase"></param>
        /// <returns></returns>
        private int SetBase(int value)
        {
            if (value < -6) return -6;
            if (value > 6) return 6;
            return value;
        }

        /// <summary>
        /// 设置击中要害率修正
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int SetC(int value)
        {
            if (value < -1) return -1;
            if (value > 4) return 4;
            return value;
        }

        public int CriticalHit
        {
            get
            {
                return criticalHit_C;
            }

            set
            {
                criticalHit_C = SetC(value);
            }
        }

        public int PhysicPower
        {
            get
            {
                return physicPower;
            }

            set
            {
                physicPower = SetBase(value);
            }
        }

        public int PhysicDefence
        {
            get
            {
                return physicDefence;
            }

            set
            {
                physicDefence = SetBase(value);
            }
        }

        public int EnergyPower
        {
            get
            {
                return energyPower;
            }

            set
            {
                energyPower = SetBase(value);
            }
        }

        public int EnergyDefence
        {
            get
            {
                return energyDefence;
            }

            set
            {
                energyDefence = SetBase(value);
            }
        }

        public int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = SetBase(value);
            }
        }
    }
}
