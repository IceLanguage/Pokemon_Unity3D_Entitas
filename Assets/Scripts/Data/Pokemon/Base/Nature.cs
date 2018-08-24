using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using MyAttribute;
#endif
namespace PokemonBattele
{
    /// <summary>
    /// 性格
    /// </summary>
    [Serializable]
    public class Nature
    {
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("精灵性格")]
        #endif
        public NatureType natureType;

        //提高的属性
        #if UNITY_EDITOR
        [FieldLabel("提升的能力")]
        #endif
        [SerializeField]
        public Base6 upAtt;

        //降低的属性
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("降低的能力")]
#endif
        public Base6 downAtt;

        public string NatureName { get { return natureType.ToString(); } }

        /// <summary>
        /// 计算性格对各项能力的影响
        /// </summary>
        /// <param name="base6"></param>
        /// <returns></returns>
        float NatureAffect(Base6 base6)
        {
            return upAtt == base6 ? 0.1f : downAtt == base6 ? -0.1f : 0;
        }
        public float PhysicPowerAffect
        {
            get
            {
                return NatureAffect(Base6.攻击);
            }
        }

    

        public float PhysicDefenceAffect
        {
            get
            {
                return NatureAffect(Base6.防御);
            }
        }
        public float EnergyPowerEffect
        {
            get
            {
                return NatureAffect(Base6.特攻);
            }
        }
        public float EnergyDefenceEffect
        {
            get
            {
                return NatureAffect(Base6.特防);
            }
        }
        public float SpeedAffect
        {
            get
            {
                return NatureAffect(Base6.速度);
            }
        }
    }
    public enum NatureType
    {
        马虎,
        顽皮,
        认真,
        自大,
        胆小,
        爽朗,
        温顺,
        温和,
        淘气,
        浮躁,
        慢吞吞,
        慎重,
        悠闲,
        急躁,
        怕寂寞,
        害羞,
        天真,
        大胆,
        坦率,
        固执,
        勤奋,
        勇敢,
        冷静,
        内敛,
        乐天
    }

    public class NatureType_EnumComare : IEqualityComparer<NatureType>
    {
        public bool Equals(NatureType x, NatureType y)
        {
            return x == y;
        }

        public int GetHashCode(NatureType obj)
        {
            return (int)obj;
        }
    }
}
