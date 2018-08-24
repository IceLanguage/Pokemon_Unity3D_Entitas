using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 状态变化
    /// </summary>
    public enum ChangeStateEnumForPokemon
    {
        Confusion,
        Flinch,
        RockWrecker,
        CanNotEscape,
        WaitNextAround,
        DisableSkill
    }

    public class ChangeStateEnumForPokemon_EnumComare : IEqualityComparer<ChangeStateEnumForPokemon>
    {
        public bool Equals(ChangeStateEnumForPokemon x, ChangeStateEnumForPokemon y)
        {
            return x == y;
        }

        public int GetHashCode(ChangeStateEnumForPokemon obj)
        {
            return (int)obj;
        }
    }
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
    /// 定身法（状态）
    /// </summary>
    class DisableSkillState : ChangeStateForPokemon
    {
        public static Dictionary<int, int> count = new Dictionary<int, int>();
       
        public DisableSkillState(bool isUseSelf = false) : base(isUseSelf)
        {

        }
        public override void Init(BattlePokemonData pokemon)
        {            
            count[pokemon.ID]= 4;
            DisableSkill.context[pokemon.ID] = pokemon.LastUseSkillID;
        }
        public override void LoseState(BattlePokemonData pokemon)
        {
            count.Remove(pokemon.ID);
            DisableSkill.context.Remove(pokemon.ID);
        }
        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            if (0 == count[pokemon.ID]--)
                LoseState(pokemon);
        }
    }
    /// <summary>
    /// 技能下一回合释放
    /// </summary>
    class WaitNextAroundState: ChangeStateForPokemon
    {
        public static Dictionary<int, int> count = new Dictionary<int, int>();
        public WaitNextAroundState(bool isUseSelf = false) : base(isUseSelf)
        {

        }
        public override void Init(BattlePokemonData pokemon)
        {
            count[pokemon.ID] = 1;
            DebugHelper.LogFormat("{0}正在蓄力", pokemon.Ename);
           
        }
        public override bool CanAction(BattlePokemonData pokemon)
        {
            if(1 == count[pokemon.ID])
            {
                LoseState(pokemon);
                return true;

            }
            return false;

        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            NeedReplaceSKill.context.Remove(pokemon.ID);
            DebugHelper.LogFormat("{0}蓄力结束", pokemon.Ename);
            pokemon.RemoveChangeState(ChangeStateEnumForPokemon.WaitNextAround);
        }


    }

    /// <summary>
    /// 无法逃脱状态
    /// </summary>
    class CanNotEscapeState : ChangeStateForPokemon
    {
        //-1不能逃脱,>0则是不能逃脱的回合数
        //ifCanScape[ID]= 1//一回合不能逃脱
        public static Dictionary<int, int> ifCanScape = new Dictionary<int, int>();

        public CanNotEscapeState(bool isUseSelf = false) : base(isUseSelf)
        {

        }

        public override void Init(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}陷入了无法逃脱状态", pokemon.Ename);
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {

            if (ifCanScape[pokemon.ID]>0)
            {
                ifCanScape[pokemon.ID]--;
            }
            else if (0 == ifCanScape[pokemon.ID])
            {
                LoseState(pokemon);
            }
            else if(-1 == ifCanScape[pokemon.ID])
            {
                return;
            }
            
        }
        
        public override void LoseState(BattlePokemonData pokemon)
        {
            ifCanScape.Remove(pokemon.ID);
            DebugHelper.LogFormat("{0}无法逃脱状态解除", pokemon.Ename);
            pokemon.RemoveChangeState(ChangeStateEnumForPokemon.CanNotEscape);
        }
    }

    /// <summary>
    /// 大闹一番状态
    /// </summary>
    class RockWreckerState: ChangeStateForPokemon
    {
        public static Dictionary<int, int> count = new Dictionary<int, int>();
       //public static Dictionary<int, int> RockWreckerSkillID = new Dictionary<int, int>();
        public RockWreckerState(bool isUseSelf = true) : base(isUseSelf)
        {
           
        }
        public override void Init(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}陷入了大闹一番状态", pokemon.Ename);
            count[pokemon.ID] = RandomService.game.Int(2, 4);
        }
        public override void LoseState(BattlePokemonData pokemon)
        {
            NeedReplaceSKill.context.Remove(pokemon.ID);
            DebugHelper.LogFormat("{0}大闹一番状态解除", pokemon.Ename);

            pokemon.RemoveChangeState(ChangeStateEnumForPokemon.RockWrecker);

            DebugHelper.LogFormat("{0}因为大闹一番混乱了", pokemon.Ename);
            pokemon.AddChangeState(ChangeStateEnumForPokemon.Flinch);
        }
        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {

            if (0 == --count[pokemon.ID])
            {
                LoseState(pokemon);

            }
            else
            {
                DebugHelper.Log(
                new StringBuilder(40)
                .AppendFormat("{0}还需要", pokemon.Ename)
                .Append(count[pokemon.ID])
                .Append("回合解除大闹一番状态")
                .ToString());
            }
        }
    }
    /// <summary>
    /// 畏缩状态
    /// </summary>
    class FlinchState : ChangeStateForPokemon
    {
        public FlinchState(bool isUseSelf = false) : base(isUseSelf)
        {

        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}畏惧了,进攻失败", pokemon.Ename);
            return false;
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            LoseState(pokemon);
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}畏缩状态解除", pokemon.Ename);

            pokemon.RemoveChangeState(ChangeStateEnumForPokemon.Flinch);
        }
        public override void Init(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}畏惧了", pokemon.Ename);
        }
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
            DebugHelper.LogFormat("{0}混乱了", pokemon.Ename);
            bool flag = RandomService.game.Int(0, 3) >= 1;
            if (!flag)
            {

                DebugHelper.LogFormat("{0}混乱了,进攻失败，并对自己造成了伤害", pokemon.Ename);


                int damage = PokemonCalculation.CalDamage(pokemon, pokemon, DefaultSkill);
                pokemon.curHealth -= damage;
                if (pokemon.curHealth < 0)
                    pokemon.curHealth = 0;
            }

            return flag;
        }

        public override void Init(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}混乱了", pokemon.Ename);
            count[pokemon.ID] = RandomService.game.Int(1, 5);
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}混乱状态解除", pokemon.Ename);
            count.Remove(pokemon.ID);
            pokemon.RemoveChangeState(ChangeStateEnumForPokemon.Confusion);
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {

            if (!count.ContainsKey(pokemon.ID)|| 0 == --count[pokemon.ID])
            {
                LoseState(pokemon);

            }
            else
            {
                DebugHelper.LogFormat("{0}还需要{1}回合解除混乱状态", pokemon.Ename, count[pokemon.ID]);
            }
        }
    }
}
