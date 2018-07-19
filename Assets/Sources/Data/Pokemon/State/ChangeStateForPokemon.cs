using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    //向精灵施加的状态变化
    abstract class ChangeStateForPokemon : ChangeState
    {
        public readonly bool isUseSelf = false;

        public ChangeStateForPokemon(bool isUseSelf)
        {
            this.isUseSelf = isUseSelf;
        }

        public static Dictionary<ChangeStateEnumForPokemon, PokemonState> ChangeStateForPokemons = new
            Dictionary<ChangeStateEnumForPokemon, PokemonState>();
    }

    /// <summary>
    /// 混乱状态
    /// </summary>
    class ConfusionState : ChangeStateForPokemon
    {
        public static Skill DefaultSkill = BuidlDefaultSkill();
        public static Skill BuidlDefaultSkill()
        {
            Skill skill = UnityEngine.ScriptableObject.CreateInstance<Skill>();
            skill.att = PokemonType.一般;
            skill.power = 40;
            skill.type = SkillType.物理;
            return skill;
        }
        public static Dictionary<int, int> count = new Dictionary<int, int>();
        public ConfusionState(bool isUseSelf = false) : base(isUseSelf)
        {

        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            bool flag = RandomService.game.Int(0, 3) >= 1;
            if (!flag)
            {
                Debug.Log(new StringBuilder(30)
              .AppendFormat("{0}混乱了进攻失败，并对自己造成了伤害 {1}",
                  pokemon.Ename, System.DateTime.Now)
              .ToString());

                int damage = PokemonCalculation.CalDamage(pokemon, pokemon, DefaultSkill);
                pokemon.curHealth -= damage;
                if (pokemon.curHealth < 0)
                    pokemon.curHealth = 0;
            }
            return flag;
        }

        public override void Init(BattlePokemonData pokemon)
        {
            count[pokemon.ID] = RandomService.game.Int(1, 5);
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            count.Remove(pokemon.ID);
            pokemon.ChangeStateForPokemonEnums.Remove(ChangeStateEnumForPokemon.Confusion);
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            if (0 == --count[pokemon.ID])
            {
                LoseState(pokemon);

            }
        }
    }
}
