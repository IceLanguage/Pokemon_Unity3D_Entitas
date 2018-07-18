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
    /// 种族
    /// </summary>
    [Serializable]
    public class Race 
    {
        public int raceid;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("种族")]
        #endif
        public string sname;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("HP")]
        #endif
        public int health;
     

        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("物攻")]
        #endif
        public int phyPower;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("物防")]
        #endif
        public int phyDefence;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("特攻")]
        #endif
        public int energyPower;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("特防")]
        #endif
        public int energyDefence;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("速度种族值")]
        #endif
        public int speed;
        [SerializeField]
        #if UNITY_EDITOR
        [FieldLabel("第一属性")]
        #endif
        public PokemonType pokemonMainType;
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("第二属性")]
#endif
        public PokemonType pokemonSecondType;
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("第一特性")]
#endif
        public AbilityType firstAbility;
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("第二特性")]
#endif
        public AbilityType secondAbility;
        [SerializeField]
#if UNITY_EDITOR
        [FieldLabel("隐藏特性")]
#endif
        public AbilityType hideAbility;



        //同一只精灵可能有不同的种族
        [SerializeField]
        public List<int> otherRace = new List<int>();

        /// <summary>
        /// 种族构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hea"></param>
        /// <param name="ppow"></param>
        /// <param name="pdef"></param>
        /// <param name="epow"></param>
        /// <param name="edef"></param>
        /// <param name="speed"></param>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <param name="ability1"></param>
        /// <param name="ability2"></param>
        /// <param name="ability3"></param>
        public Race(int raceid,string name, int hea, int ppow, int pdef, int epow, int edef, int speed,
            PokemonType type1, PokemonType type2, AbilityType ability1, AbilityType ability2, AbilityType ability3)
        {
            this.raceid = raceid;
            sname = name;
            health = hea;
            phyPower = ppow;
            phyDefence = pdef;
            energyPower = epow;
            energyDefence = edef;
            this.speed = speed;
            pokemonMainType = type1;
            pokemonSecondType = type2;
            firstAbility = ability1;
            secondAbility = ability2;
            hideAbility = ability3;
            //otherRace == new List<int>();
        }

       

    }
}
