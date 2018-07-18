using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 基础点数?努力值
    /// </summary>
    [Serializable]
    public class Basestats: PokemonBaseData
    {
        [SerializeField]
        public new int health=0;
        [SerializeField]
        protected new  int physicPower = 0;
        [SerializeField]
        protected new  int physicDefence = 0;
        [SerializeField]
        protected new  int energyPower = 0;
        [SerializeField]
        protected new int energyDefence = 0;
        [SerializeField]
        protected new int speed = 0;
        /// <summary>
        /// 努力值总值
        /// </summary>
        private const int MaxStaces = 512;

        /// <summary>
        /// 单项属性最大努力值
        /// </summary>
        private const int MaxOneStat = 252;
       
        /// <summary>
        /// 已使用属性点数
        /// </summary>
        public int CurrentAllBasestats
        {
            get
            {
                return Health + PhysicPower + physicDefence + EnergyPower + EnergyDefence + Speed;
            }
            
        }

        /// <summary>
        /// 设置努力值
        /// 防止努力值超出范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currentSetBase"></param>
        /// <returns></returns>
        private int  SetStat(int value, int currentSetValue)
        {
            int resStat = MaxStaces - (CurrentAllBasestats-currentSetValue);
            resStat = resStat > MaxOneStat ? MaxOneStat : resStat;
            return value > resStat ? resStat : value>0?value:0;
        }

        public override int Health
        {
            get
            {
                return health;
            }

            set
            {
                health = SetStat(value,health);
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
                physicPower = SetStat(value,physicPower);
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
                physicDefence = SetStat(value, physicDefence);
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
                energyPower = SetStat(value,energyPower);
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
                energyDefence = SetStat(value,energyDefence);
            }
        }

        public override  int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = SetStat(value,speed);
            }
        }
    }
}
