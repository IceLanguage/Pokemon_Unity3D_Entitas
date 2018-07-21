using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 特性影响
    /// </summary>
    public abstract class AbilityImpact
    {
        //召唤时触发
        public virtual void OnBeCalled(BattlePokemonData self) { }

        //离场时触发
        public virtual void OnLeave(BattlePokemonData self) { }

        //进攻时触发
        public virtual void OnAttack(
            BattlePokemonData self,Skill skill, BattlePokemonData defencePokemon,
            ref float power,ref float Critical) { }
        public virtual void OnAttack(
            BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon,
            ref int HitRateB)
        { }
        public virtual void OnAttack(
            ref bool hit,BattlePokemonData self, Skill skill, BattlePokemonData defencePokemon
            )
        { }
        //对手使用技能时触发
        public virtual void OnEnemySkillUse(BattlePokemonData attackPokemon, int skillIndex, BattlePokemonData self)
        {

        }
        //技能造成伤害时触发
        public virtual void OnSkillCauseDamage(
            BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
            ref int damage)
        { }
        //被进攻时触发
        public virtual void OnBeAttacked(
            BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
            ref int avoid)
        { }
        public virtual void OnBeAttacked(
           BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
           ref bool IsIngnore)
        { }
        public virtual void OnBeAttacked(
            ref bool hit,BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon
            )
        { }
        public virtual void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon,
            ref bool hit, ref float resDamage)
        { }
        //进入异常状态时触发
        public virtual void OnGetAbormal(
            BattlePokemonData self,AbnormalStateEnum newstate,
            ref bool canGet) { }

        //陷入状态变化时触发
        public virtual void OnGetChangeState(
            BattlePokemonData self,ChangeStateEnumForPokemon newstate,
            ref bool canAddState)
        { }

        //当异常状态造成伤害时触发
        public virtual void OnAbnomalCauseDamage(BattlePokemonData self, ref int damage)
        { }

        //当状态变化造成伤害时触发
        public virtual void OnChangeStateCauseDamage(BattlePokemonData self, ref int damage)
        { }

        //准备阶段触发
        public virtual void UpdateInPlayerAround(BattlePokemonData pokemon) { }

        //环境变更时触发
        public virtual void OnEnvironmentChange(BattlePokemonData self) {  }

        //能力阶级变更时触发
        public virtual void OnStatModifiersChange(BattlePokemonData self,ref StatModifiers newvalue) { }

        //遭遇精灵时触发
        public virtual EncounterPokemon OnEncounterPokemon(BattlePokemonData self, EncounterPokemon encounterPokemon) { return encounterPokemon; }

        public virtual void OnEncounterPokemon(BattlePokemonData self, Pokemon pokemon) { }
     
        

        //击败精灵时触发
        public virtual void OnDefeatPokemon(BattlePokemonData self,BattlePokemonData defeatPokemon) { }
    }

    public static class AbilityManager
    {
        public static Dictionary<AbilityType, AbilityImpact> AbilityImpacts = new Dictionary<AbilityType, AbilityImpact>();
    }
}
