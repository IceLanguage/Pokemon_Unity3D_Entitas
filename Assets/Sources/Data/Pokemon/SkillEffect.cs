using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace PokemonBattelePokemon
{
    [Serializable]
    public class SkillEffect:ScriptableObject
    {
        [SerializeField]
        public GameObject effect;
    }
}
