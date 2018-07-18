using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;

namespace PokemonBattele
{
    

    public sealed partial class UseSkill 
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
                    if (IsIngnore())
                    {
                        Debug.Log(skill.sname +"没有效果");
                        return;
                    }
                    
                    UseSkillDamage();
                    var useSkillManager =UseSkillEffectManager.UseSkillDic;
                    if (useSkillManager.ContainsKey(skill.SKillID))
                    {
                        BattlePokemonData PokemonUseEffect = skill.isUseForSelf ? attackPokemon : defencePokemon;
                        foreach(var effect in useSkillManager[skill.SKillID])
                            effect.UseSkillSpecialEffect(PokemonUseEffect);
                    }
                    if (PokemonType.火 == skill.att && AbnormalState.Frostbite == defencePokemon.Abnormal)
                    {
                        defencePokemon.SetAbnormalState(AbnormalState.Normal);
                    }
                }
                else
                {
                    ShowSkillMissUI();
                    Debug.Log(skill.sname + "技能未命中");
                }

            }           
            
        }
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
           

            if(attackPokemon.StateForAbnormal.CanAction(attackPokemon))
            {
                return true;
            }
            return false;
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

        private readonly static PokemonType[] IngnoreAttackPokemonType = new PokemonType[6]
        {
            PokemonType.格斗,PokemonType.一般,PokemonType.毒,PokemonType.电,PokemonType.地面,PokemonType.超能
        };
        private readonly static PokemonType[] IngnoreDefencePokemonType = new PokemonType[6]
        {
            PokemonType.幽灵,PokemonType.幽灵,PokemonType.钢,PokemonType.地面,PokemonType.飞行,PokemonType.恶
        };
        private bool IsIngnore()
        {
            for(int i=0;i<6;++i)
            {
                PokemonType a = IngnoreAttackPokemonType[i];
                PokemonType b = IngnoreDefencePokemonType[i];
                bool aflag = a == skill.att;
                if(aflag)
                {
                    bool bflag = b == defencePokemon.MainPokemonType || b == defencePokemon.SecondPokemonType;
                    if (bflag)
                        return true;
                }
                
            }
            return false;
        }
    }
    
}
