using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 基础6维
    /// </summary>
    public enum Base6
    {
        特攻,
        攻击,
        特防,
        防御,
        速度,
        生命,
        NULL

    }
    /// <summary>
    ///精灵基础6维
    /// </summary>
    [Serializable]
    public class PokemonBaseData
    {

        protected int health = 0;

        protected int physicPower = 0;

        protected int physicDefence = 0;

        protected int energyPower = 0;

        protected int energyDefence = 0;

        protected int speed = 0;
        /// <summary>
        /// 生命
        /// </summary>
        public virtual int Health
        {
            get
            {
                return health;
            }

            set
            {
                health =value;
            }
        }

        /// <summary>
        /// 物攻
        /// </summary>
        public virtual int PhysicPower
        {
            get
            {
                return physicPower;
            }

            set
            {
                physicPower = value;
            }
        }

        /// <summary>
        /// 物防
        /// </summary>
        public virtual int PhysicDefence
        {
            get
            {
                return physicDefence;
            }

            set
            {
                physicDefence =value;
            }
        }

        /// <summary>
        /// 特攻
        /// </summary>
        public virtual int EnergyPower
        {
            get
            {
                return energyPower;
            }

            set
            {
                energyPower = value;
            }
        }

        /// <summary>
        /// 特防
        /// </summary>
        public virtual int EnergyDefence
        {
            get
            {
                return energyDefence;
            }

            set
            {
                energyDefence = value;
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        public virtual int Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed =value;
            }
        }
    }
}
