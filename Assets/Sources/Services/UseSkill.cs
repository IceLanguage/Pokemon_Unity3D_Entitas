using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;

namespace PokemonBattelePokemon
{
    public interface IUseSkillSpecialEffect
    {
        void UseSkillSpecialEffect();
    }
    public sealed partial class UseSkillManager
    {
        public Dictionary<int, IUseSkillSpecialEffect> UseSkillDic =
            new Dictionary<int, IUseSkillSpecialEffect>();
    }
    public interface IShowUseSkillUI
    {
        void ShowSkillUI();
        void ShowSkillMissUI();
    }
    public sealed partial class UseSkill : IShowUseSkillUI
    {
        
        public void ShowSkillMissUI()
        {
            if (attackPokemon == BattleController.Instance.PlayerCurPokemonData)
            {
                TTUIPage.ShowPage<UIBattle_PlaySkillMiss>();
            }
            else
            {
                TTUIPage.ShowPage<UIBattle_EnemySkillMiss>();
            }
        }

        public void ShowSkillUI()
        {
            if (attackPokemon == BattleController.Instance.PlayerCurPokemonData)
            {
                UIBattle_ShowPlaySkill page = (UIBattle_ShowPlaySkill)TTUIPage.allPages["UIBattle_ShowPlaySkill"];
                page.Show(skill.sname);
            }
            else
            {
                UIBattle_ShowEnemySkill page = (UIBattle_ShowEnemySkill)TTUIPage.allPages["UIBattle_ShowEnemySkill"];
                page.Show(skill.sname);
            }
        }
    }
    public sealed partial class UseSkill
    {
        public static void Attack(
            Skill skill,
            BattlePokemonData AttackPokemon,
            BattlePokemonData DefencePokemon
            )
        {
            if (null == AttackPokemon || null == DefencePokemon)
                return;
            new UseSkill(skill, AttackPokemon, DefencePokemon);           
        }

        private readonly Skill skill;
        private readonly BattlePokemonData attackPokemon;
        private readonly BattlePokemonData defencePokemon;
        private readonly int skillIndex = -1;
        public UseSkill
            (
            Skill skill,
            BattlePokemonData AttackPokemon,
            BattlePokemonData DefencePokemon
            )
        {
            this.skill = skill;
            attackPokemon = AttackPokemon;
            defencePokemon = DefencePokemon;

            GameContext context = Contexts.sharedInstance.game;
            GameEntity attackPokemonEntity = context.GetEntityWithBattlePokemonData(attackPokemon);
            GameEntity defencePokemonEntity = context.GetEntityWithBattlePokemonData(defencePokemon);

            for (int i = 0; i < 4 && i < attackPokemon.skills.Count; ++i)
            {
                if (attackPokemon.skills[i] == skill.SKillID)
                {
                    skillIndex = i;
                    break;
                }

            }
            
            if(IsSkillUseSuccess())
            {
                ShowSkillUI();

                UseSkillEffect();
                UseSkillPP();

                if (IsSkillHitAim())
                {
                    UseSkillDamage();
                    var useSkillManager = ResourceController.Instance.useSkillManager.UseSkillDic;
                    if (useSkillManager.ContainsKey(skill.SKillID))
                    {
                        useSkillManager[skill.SKillID].UseSkillSpecialEffect();
                    }
                    //UseSkillSpecialEffect();
                }
                else
                {
                    ShowSkillMissUI();
                    Debug.Log("技能未命中");
                }


                //attackPokemonEntity.ReplaceBattlePokemonData(attackPokemon);
                //defencePokemonEntity.ReplaceBattlePokemonData(defencePokemon);
            }           
            
        }
        //技能的特殊效果
        //private void UseSkillSpecialEffect() { }
        /// <summary>
        /// 技能是否使用成功
        /// </summary>
        /// <returns></returns>
        private bool IsSkillUseSuccess()
        {
            if (attackPokemon.skillPPs[skillIndex] <= 0)
            {
                Debug.LogError("没有提前判断技能是否可以使用");
                return false;
            }
           //检测是否无法行动
           //TODO
            return true;
        }

        /// <summary>
        /// 技能是否命中目标
        /// </summary>
        /// <returns></returns>
        private bool IsSkillHitAim()
        {
            return RandomService.game.Int(0, 100) <= skill.hitRate;
        }
        /// <summary>
        /// 技能伤害计算
        /// </summary>
        private void UseSkillDamage()
        {
            int damage = PokemonCalculation.CalDamage(attackPokemon, defencePokemon, skill);
            defencePokemon.curHealth -= damage;
            if (defencePokemon.curHealth < 0)
                defencePokemon.curHealth = 0;
            
        }

        /// <summary>
        /// 技能PP消耗
        /// </summary>
        private void UseSkillPP()
        {
            attackPokemon.skillPPs[skillIndex] -= 1;
        }
        /// <summary>
        /// 技能特效的释放
        /// </summary>
        private void UseSkillEffect()
        {
            var pool = ObjectPoolController.SkillEffectsObjectPool;
            SkillEffect effect = skill.effect;
            Transform effectUseTransform ;

            if (effect.IsUseSelf && attackPokemon == BattleController.Instance.PlayerCurPokemonData)
            {
                effectUseTransform = BattleController.Instance.PlayerCurPokemonData.transform;
            }
            else if(!effect.IsUseSelf && defencePokemon == BattleController.Instance.PlayerCurPokemonData)
            {
                effectUseTransform = BattleController.Instance.PlayerCurPokemonData.transform;
            }
            else 
            {
                effectUseTransform = BattleController.Instance.EnemyCurPokemonData.transform;
            }

            GameObject gameObject = pool[skill.SKillID];
            if (null == gameObject || gameObject.activeSelf
                )
            {
                gameObject = UnityEngine.Object.Instantiate(
                    effect.effect,
                    effectUseTransform.position,
                    effectUseTransform.rotation,
                    BattleController.Instance.transform);
            }
            else
            {              
                gameObject.transform.position = effectUseTransform.position;
                gameObject.transform.rotation = effectUseTransform.rotation;
                gameObject.SetActive(true);
            }
            LHCoroutine.CoroutineManager.DoCoroutine(
                DisableEffectGameObject(gameObject, skill.SKillID));

        }

        
        private IEnumerator DisableEffectGameObject(GameObject effect,int skillID)
        {
            yield return new WaitForSeconds(BattleController.Instance.BattleTime);
            if (effect != null && effect.activeSelf)
            {
                effect.SetActive(false);
                ObjectPoolController.SkillEffectsObjectPool[skillID] = effect;
            }
                
        }
    }
    
}
