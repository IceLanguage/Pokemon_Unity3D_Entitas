using PokemonBattele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 精灵捕获或精灵属性和伤害数值的计算
/// </summary>
public static class PokemonCalculation
{
    private const int level = 100;
    private static int CalBase(int raceVal, int BaseStatVal, int IVVal)
    {
        float f = (float)level / 100;
        return (int)((raceVal * 2 + BaseStatVal / 4 + IVVal) * f + level + 5);
    }
    /// <summary>
    /// 计算生命属性
    /// </summary>
    /// <param name="raceVal"></param>
    /// <param name="BaseStatVal"></param>
    /// <param name="IVVal"></param>
    /// <returns></returns>
    public static int CalFullHealth(int raceVal, int BaseStatVal, int IVVal)
    {
        return CalBase(raceVal, BaseStatVal, IVVal) + 5;
    }
    /// <summary>
    /// 计算生命属性外的其他属性
    /// </summary>
    /// <param name="raceVal"></param>
    /// <param name="BaseStatVal"></param>
    /// <param name="IVVal"></param>
    /// <returns></returns>
    public static int CalCombatBasePower(int raceVal, int BaseStatVal, int IVVal,float natureEffect,int statModifierEffect)
    {

        return (int)(CalBase(raceVal, BaseStatVal, IVVal) * (natureEffect + 1)* statModifierEffect)/100;
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="attakPokemon"></param>
    /// <param name="defencePokemon"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public static int CalDamage(
        BattlePokemonData attackPokemon,
        BattlePokemonData defencePokemon,
        Skill skill)
    {

        float power;

        if (skill.type == SkillType.物理)
        {
            power = (float)attackPokemon.PhysicPower / defencePokemon.PhysicDefence;


        }
        else if (skill.type == SkillType.特殊)
        {
            power = (float)attackPokemon.EnergyPower / defencePokemon.EnergyDefence;
        }
        else return 0;

        float BaseDamage = (2 * level + 10) / 250.0f * power * skill.power + 2;
        float typeInfos = ResourceController.Instance.GetTypeInfo
            ((int)skill.att,
            (int)defencePokemon.MainPokemonType);
        typeInfos *= ResourceController.Instance.GetTypeInfo
            ((int)skill.att,
            (int)defencePokemon.SecondPokemonType);

        if(typeInfos == 2f)
        {
            Debug.Log(skill.sname+ "效果拔群" + System.DateTime.Now);
        }
        else if (typeInfos == 4f)
        {
            Debug.Log(skill.sname + "效果超群" + System.DateTime.Now);
        }
        else if(typeInfos<1f)
        {
            Debug.Log(skill.sname + "效果不太理想" + typeInfos + System.DateTime.Now);
        }

        if (skill.att == attackPokemon.MainPokemonType ||
            skill.att == attackPokemon.SecondPokemonType)
        {
            typeInfos *= 1.5f;
        }
        float resDamage = BaseDamage * typeInfos;

        //击中要害
        float CriticalHitProbability = StatModifiers.criticalHit_C_To_B
            [attackPokemon.StatModifiers.CriticalHit + skill.CriticalHitC];
        if (RandomService.game.Float(0, 1) < CriticalHitProbability)
            resDamage *= 2;

        return (int)resDamage;
    }


}
