using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 个体值
    /// </summary>
    [Serializable]
    public class IndividualValues:PokemonBaseData
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

        private System.Random ran = new System.Random();
        /// <summary>
        /// 随机获取个体值
        /// </summary>
        /// <returns></returns>
        int RandomIV()
        {
            //long tick = DateTime.Now.Ticks;
            return ran.Next(MaxIV + 1);
        }
        public IndividualValues()
        {
            health = RandomIV();
            physicPower = RandomIV();
            physicDefence = RandomIV();
            energyPower = RandomIV();
            energyDefence = RandomIV();
            speed = RandomIV();
        }
        public IndividualValues(int h,int pp,int pd,int ep,int ed,int s)
        {
            health = h;
            physicPower = pp;
            physicDefence = pd;
            energyPower = ep;
            energyDefence = ed;
            speed = s;
        }
        private const int MaxIV = 31;
        /// <summary>
        /// 设置属性
        /// 防止属性超出范围
        /// </summary>
        /// <param name="Value">设置的值</param>
        /// <returns></returns>
        private int SetIV(int Value)
        {
            return Value > MaxIV ? MaxIV : Value > 0?Value: 0;

        }
        public override int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = SetIV(value);
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
                physicPower = SetIV(value);
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
                physicDefence = SetIV(value);
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
                energyPower = SetIV(value);
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
                energyDefence = SetIV(value);
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
               speed = SetIV(value);
            }
        }
    }
}
