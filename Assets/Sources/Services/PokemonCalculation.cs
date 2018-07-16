using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public static int CalCombatBasePower(int raceVal, int BaseStatVal, int IVVal,float natureEffect)
    {

        return (int)(CalBase(raceVal, BaseStatVal, IVVal) * (natureEffect + 1));
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

            ////考虑到灼烧状态对物攻的影响
            //if (attakPokemon.hasAbnormalState &&
            //    attakPokemon.abnormalState.abnormalState == AbnormalState.Burns)
            //{
            //    power /= 2;
            //}



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
        if (skill.att == attackPokemon.MainPokemonType ||
            skill.att == attackPokemon.SecondPokemonType)
        {
            typeInfos *= 1.5f;
        }
        float resDamage = BaseDamage * typeInfos;

        return (int)resDamage;
    }


}
