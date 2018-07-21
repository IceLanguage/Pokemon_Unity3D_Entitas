using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
#if UNITY_EDITOR
    /// <summary>
    /// 精灵技能池
    /// </summary>
    [CreateAssetMenu(fileName = "SkillPool", menuName = "ScriptableObjec/SkillPool")]
#endif
    public class SkillPool :ScriptableObject
    {
        public int PokemonID;
        public List<int> Skills;
    }
}
