using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattelePokemon
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "SkillPool", menuName = "ScriptableObjec/SkillPool")]
#endif
    public class SkillPool :ScriptableObject
    {
        public int PokemonID;
        public List<int> Skills;
    }
}
