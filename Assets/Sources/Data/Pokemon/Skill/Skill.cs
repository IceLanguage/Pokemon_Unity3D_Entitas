#if UNITY_EDITOR
using MyAttribute;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace PokemonBattele
{
    /// <summary>
    /// 技能
    /// </summary>
    [Serializable]
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjec/Skill")]
#endif
    public class Skill : ScriptableObject
    {

        

        public int SKillID;
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("技能名称")]
#endif
        public string sname;

        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("技能类别")]
#endif
        public SkillType type;

        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("威力")]
#endif
        public int power;//威力

        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("命中率")]
#endif
        public int hitRate;//命中率

        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("pp值")]
#endif
        private int fullPP;//pp上限
        //private int curPP;//当前pp值

        //属性
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("属性")]
#endif
        public PokemonType att;

        [SerializeField]
        public int FullPP
        {
            get
            {
                return fullPP;
            }

            set
            {
                fullPP = value;
            }
        }

        public SkillEffect effect;

        //技能效果的作用对象
        [SerializeField]
        //public bool isUseForSelf;

        //技能提升攻击要害率
        public int CriticalHitC;

        //是否需要蓄力
        //public bool needPowerStorage;
    }

    public enum SkillType
    {
        物理,
        特殊,
        变化,
        NULL
    }

    
}
