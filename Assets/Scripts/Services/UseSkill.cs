using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using System.Text;
using MyUnityEventDispatcher;

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
			if (0 == AttackPokemon.curHealth) return;

			new UseSkill(skill, AttackPokemon, DefencePokemon);
		
			LHCoroutine.CoroutineManager.DoCoroutine(WaitSKillUse(AttackPokemon, skill));
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
			GameEntity attackPokemonEntity = attackPokemon.entity;
			GameEntity defencePokemonEntity = defencePokemon.entity;

			for (int i = 0; i < 4 && i < attackPokemon.skills.Count; ++i)
			{
				if (attackPokemon.skills[i] == skill.SKillID)
				{
					skillIndex = i;
					break;
				}

			}
			DebugHelper.LogFormat("{0}尝试使用技能{1}攻击{2}", attackPokemon.Ename, skill.sname, defencePokemon.Ename);

			//考虑特性
			if (AbilityManager.AbilityImpacts.ContainsKey(defencePokemon.ShowAbility))
				AbilityManager.AbilityImpacts[defencePokemon.ShowAbility]
					.OnEnemySkillUse(attackPokemon, skillIndex, defencePokemon);

			if (IsSkillUseSuccess())
			{
				
				
				DebugHelper.LogFormat("{0}成功使用技能{1}攻击{2}", attackPokemon.Ename, skill.sname, defencePokemon.Ename);
				ShowSkillUI();

				UseSkillEffect();
				UseSkillPP();

				bool endUseSkill = false;
				var useSkillManager = UseSkillEffectManager.UseSkillDic;
				if (useSkillManager.ContainsKey(skill.SKillID))
				{
					foreach (var effect in useSkillManager[skill.SKillID])
					{
						if (effect.isUseSelf)
						{
							effect.BattleStartEffect(attackPokemon, ref endUseSkill);

						}
						else
						{
							effect.BattleStartEffect(defencePokemon, ref endUseSkill);
						}
					}
				}
				if (endUseSkill) return;
				if (IsSkillHitAim())
				{
					if (IsIngnore())
					{
						DebugHelper.LogFormat("{0}的技能{1}对{2}没有效果", attackPokemon.Ename, skill.sname, defencePokemon.Ename);

						return;
					}

					UseSkillDamage();

					
					if (useSkillManager.ContainsKey(skill.SKillID))
					{
						
						foreach(var effect in useSkillManager[skill.SKillID])
						{
							if(effect.isUseSelf)
							{
								effect.SpecialEffect(attackPokemon);

							}
							else
							{
								effect.SpecialEffect(defencePokemon);
							}
						}
							
					}



					if (PokemonType.火 == skill.att &&
						AbnormalStateEnum.Frostbite == defencePokemon.Abnormal &&
						AbilityType.储水 != defencePokemon.ShowAbility)
					{
						DebugHelper.LogFormat("{0}的烧伤状态因为{1}的冰属性技能{2}解除了", defencePokemon.Ename, attackPokemon.Ename, skill.sname);
						defencePokemon.SetAbnormalStateEnum(AbnormalStateEnum.Normal);
					}


						
				}
				else
				{
					ShowSkillMissUI();

					DebugHelper.LogFormat("{0}的技能{1}没有命中{2}", attackPokemon.Ename, skill.sname, defencePokemon.Ename);

				}




			}
			

		}

		static IEnumerator WaitSKillUse(BattlePokemonData AttackPokemon,Skill skill)
		{
			yield return new WaitForSeconds(1f);

			AttackPokemon.LastUseSkillID = skill.SKillID;
			NotificationCenter<bool>.Get().DispatchEvent("BattlePause", false);
		}
		/// <summary>
		/// 技能是否使用成功
		/// </summary>
		/// <returns></returns>
		private bool IsSkillUseSuccess()
		{
			if (skillIndex>=4|| attackPokemon.skillPPs[skillIndex] <= 0)
			{
				Debug.LogError("没有提前判断技能是否可以使用");
				return false;
			}

			bool flag = false;

			if (attackPokemon.StateForAbnormal.CanAction(attackPokemon))
			{
				
				flag = true;
				int cout = attackPokemon.ChangeStateForPokemonEnums.Count;
				for (int i = cout - 1; i >= 0; --i)
				{
					var state = attackPokemon.ChangeStateForPokemonEnums[i];
					if (!ChangeStateForPokemon.ChangeStateForPokemons[state].CanAction(attackPokemon))
					{
						flag = false;
						
					}
				}
				
				

			}
		
			return flag;
		}

		/// <summary>
		/// 技能是否命中目标
		/// </summary>
		/// <returns></returns>
		private bool IsSkillHitAim()
		{
			if (skill.hitRate == 0) return true;
			int A = skill.hitRate * 255 / 100;
			int avoid = defencePokemon.StatModifiers.AvoidanceRate;

			//考虑特性
			if (AbilityManager.AbilityImpacts.ContainsKey(defencePokemon.ShowAbility))
				AbilityManager.AbilityImpacts[defencePokemon.ShowAbility]
					.OnBeAttacked
					(
						defencePokemon, skill, attackPokemon,
						ref avoid
					);

			int B = attackPokemon.StatModifiers.HitRate - avoid ;


			//考虑特性		
			if (AbilityManager.AbilityImpacts.ContainsKey(attackPokemon.ShowAbility))
				AbilityManager.AbilityImpacts[attackPokemon.ShowAbility]
					.OnAttack
					(
						attackPokemon, skill, defencePokemon,
						ref B
					);
			

			if (B > 6) B = 6;
			if (B < -6) B = -6;

			int P = A * StatModifiers.ActualCorrection[B]/100;

			
			DebugHelper.Log(new StringBuilder(40)
				.AppendFormat("{0}的技能{1}的命中率为", attackPokemon.Ename, skill.sname)
				.Append(P / 255f)
				.ToString());

			bool res = RandomService.game.Int(0, 255) <= P;
			//考虑特性
			if (AbilityManager.AbilityImpacts.ContainsKey(defencePokemon.ShowAbility))
				AbilityManager.AbilityImpacts[defencePokemon.ShowAbility]
					.OnBeAttacked
					(
						ref res,defencePokemon, skill, attackPokemon
						
					);
			if (AbilityManager.AbilityImpacts.ContainsKey(attackPokemon.ShowAbility))
				AbilityManager.AbilityImpacts[attackPokemon.ShowAbility]
					.OnAttack
					(
						ref res,attackPokemon, skill, defencePokemon
						
					);
			return res;
		}
		/// <summary>
		/// 技能伤害计算
		/// </summary>
		private void UseSkillDamage()
		{
			int damage = PokemonCalculation.CalDamage(attackPokemon, defencePokemon, skill);

			if (419 == skill.SKillID)
			{
				if (BattleController.Instance.FirstHand == defencePokemon.ID)
				{
					if (SkillType.物理 == defencePokemon.ChooseSkillType||
						SkillType.特殊 == defencePokemon.ChooseSkillType)
					{
						damage *= 2;

						DebugHelper.Log("因为雪崩的技能效果,本回合内被目标使用了攻击招式攻击并被造成了伤害，威力翻倍");
					}
				}
			}



			DebugHelper.Log(
			   new StringBuilder(40)
			   .AppendFormat("{0}的技能{1}对{2}造成", attackPokemon.Ename, skill.sname, defencePokemon.Ename)
			   .Append(damage)
			   .Append("伤害")
			   .ToString());

			//考虑特性
			if (AbilityManager.AbilityImpacts.ContainsKey(defencePokemon.ShowAbility))
				AbilityManager.AbilityImpacts[defencePokemon.ShowAbility]
					.OnSkillCauseDamage
					(
						defencePokemon, skill, attackPokemon,
						ref damage
					);

			defencePokemon.curHealth -= damage;
			if (defencePokemon.curHealth < 0)
				defencePokemon.curHealth = 0;
			if(0 == defencePokemon.curHealth)
			{
				//考虑特性		
				if (AbilityManager.AbilityImpacts.ContainsKey(attackPokemon.ShowAbility))
					AbilityManager.AbilityImpacts[attackPokemon.ShowAbility]
						.OnDefeatPokemon(attackPokemon, defencePokemon);
			}
			
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
			yield return new WaitForSeconds(BattleController.BattleTime);
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
			bool res = false;
			//考虑特性
			if (AbilityManager.AbilityImpacts.ContainsKey(defencePokemon.ShowAbility))
				AbilityManager.AbilityImpacts[defencePokemon.ShowAbility]
					.OnBeAttacked
					(
						defencePokemon, skill, attackPokemon,
						ref res
					);
			return res;
		}
	}
	
}
