using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace PokemonBattele
{
    [Serializable]
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjec/SkillEffect")]
#endif
    public class SkillEffect:ScriptableObject
    {
        [SerializeField]
        public GameObject effect;

        //技能特效是否作用于自己
        public bool IsUseSelf;
    }
}
