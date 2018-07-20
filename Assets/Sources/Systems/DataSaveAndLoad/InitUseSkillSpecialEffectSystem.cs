﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;

class InitUseSkillSpecialEffectSystem : IInitializeSystem
{
    public void Initialize()
    {
        //雷电拳
        UseSkillEffectManager.UseSkillDic[9] =new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(10) };

        //火花
        UseSkillEffectManager.UseSkillDic[52] = new List<IUseSkillSpecialEffect>()
        { new BurnsEffect(10) };

        //暴风雪
        UseSkillEffectManager.UseSkillDic[59] = new List<IUseSkillSpecialEffect>()
        { new FrostbiteEffect(10) };

        //泡沫光线
        UseSkillEffectManager.UseSkillDic[61] = new List<IUseSkillSpecialEffect>()
        { new ChangeSpeedStatModifiersEffect(10,1,false) };

        //极光束
        UseSkillEffectManager.UseSkillDic[61] = new List<IUseSkillSpecialEffect>()
        { new ChangePhysicPowerStatModifiersEffect(10,1,false) };

        //花瓣舞
        UseSkillEffectManager.UseSkillDic[80] = new List<IUseSkillSpecialEffect>()
        { new RockWreckerEffect(80,-1) };

        //电击
        UseSkillEffectManager.UseSkillDic[84] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(10) };

        //十万伏特
        UseSkillEffectManager.UseSkillDic[85] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(10) };

        //电磁波
        UseSkillEffectManager.UseSkillDic[86] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(100) };

        //打雷
        UseSkillEffectManager.UseSkillDic[87] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(30) };

        //神鸟猛击 
        UseSkillEffectManager.UseSkillDic[143] = new List<IUseSkillSpecialEffect>()
        { new FlinchEffect(30),new WaitNextAroundEffect(143,-1) };

        //泡沫
        UseSkillEffectManager.UseSkillDic[145] = new List<IUseSkillSpecialEffect>()
        { new ChangeSpeedStatModifiersEffect(10,1,false) };

        //岩崩
        UseSkillEffectManager.UseSkillDic[157] = new List<IUseSkillSpecialEffect>()
        { new FlinchEffect(30) };

        //火焰轮
        UseSkillEffectManager.UseSkillDic[172] = new List<IUseSkillSpecialEffect>()
        { new BurnsEffect(10) };


        //污泥炸弹
        UseSkillEffectManager.UseSkillDic[188] = new List<IUseSkillSpecialEffect>()
        { new PoisonEffect(30) };

        //电磁炮
        UseSkillEffectManager.UseSkillDic[192] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(100) };

        //电光
        UseSkillEffectManager.UseSkillDic[209] = new List<IUseSkillSpecialEffect>()
        { new ParalysisEffect(30) };

        //光合作用
        UseSkillEffectManager.UseSkillDic[235] = new List<IUseSkillSpecialEffect>()
        { new RecoverScaleHealthEffect(-1,50) };
        
        //暗影球
        UseSkillEffectManager.UseSkillDic[247] = new List<IUseSkillSpecialEffect>()
        { new ChangeEnergyDefenceStatModifiersEffect(20,1,false) };

        //洁净光芒
        UseSkillEffectManager.UseSkillDic[295] = new List<IUseSkillSpecialEffect>()
        { new ChangeEnergyDefenceStatModifiersEffect(50,1,false) };


        //火焰踢
        UseSkillEffectManager.UseSkillDic[299] = new List<IUseSkillSpecialEffect>()
        { new BurnsEffect(10) };

        //加农水炮 308
        UseSkillEffectManager.UseSkillDic[308] = new List<IUseSkillSpecialEffect>()
        {new CanNotEscapeEffect(-1,1) };

        //银色旋风
        UseSkillEffectManager.UseSkillDic[318] = new List<IUseSkillSpecialEffect>()
        {
            new ChangePhysicPowerStatModifiersEffect(10,1,true),
            new ChangePhysicDefenceStatModifiersEffect(10,1,true),
            new ChangeEnergyPowerStatModifiersEffect(10,1,true),
            new ChangeEnergyDefenceStatModifiersEffect(10,1,true),
            new ChangeSpeedStatModifiersEffect(10,1,true)
        };


        //信号光束 
        UseSkillEffectManager.UseSkillDic[324] = new List<IUseSkillSpecialEffect>()
        {new ConfusionEffect(10) };

        //恶之波动 
        UseSkillEffectManager.UseSkillDic[399] = new List<IUseSkillSpecialEffect>()
        { new FlinchEffect(20) };

        //雪崩 419 如果本回合内被目标使用了攻击招式攻击并被造成了伤害，威力翻倍。
        //UseSkill.cs配置

        //火焰牙 
        UseSkillEffectManager.UseSkillDic[424] = new List<IUseSkillSpecialEffect>()
        {new BurnsEffect(10),new FlinchEffect(10) };

        //泥巴炸弹 426
        UseSkillEffectManager.UseSkillDic[426] = new List<IUseSkillSpecialEffect>()
        {new ChangeHitRateStatModifiersEffect(30,1,false) };

        //流星群
        UseSkillEffectManager.UseSkillDic[434] = new List<IUseSkillSpecialEffect>()
        {new ChangeEnergyPowerStatModifiersEffect(100,2,false)};

        //放电
        UseSkillEffectManager.UseSkillDic[435] = new List<IUseSkillSpecialEffect>()
        {new ParalysisEffect(30) };

        //岩石炮
         UseSkillEffectManager.UseSkillDic[439] = new List<IUseSkillSpecialEffect>()
        {new CanNotEscapeEffect(-1,1) };

        //充电光束
        UseSkillEffectManager.UseSkillDic[451] = new List<IUseSkillSpecialEffect>()
        {new ChangeEnergyPowerStatModifiersEffect(70,1,true)};

        //种子闪光
        UseSkillEffectManager.UseSkillDic[465] = new List<IUseSkillSpecialEffect>()
        {new ChangeEnergyDefenceStatModifiersEffect(40,2,false)};

        //蓄能焰袭
        UseSkillEffectManager.UseSkillDic[488] = new List<IUseSkillSpecialEffect>()
        {new ChangePhysicPowerStatModifiersEffect(100,1,true)};

        //酸液炸弹
        UseSkillEffectManager.UseSkillDic[491] = new List<IUseSkillSpecialEffect>()
        {new ChangeEnergyDefenceStatModifiersEffect(100,2,false)};

        //棉花防守
        UseSkillEffectManager.UseSkillDic[538] = new List<IUseSkillSpecialEffect>()
        {new ChangePhysicDefenceStatModifiersEffect(-1,3,true)};

        //暗黑爆破539
        UseSkillEffectManager.UseSkillDic[539] = new List<IUseSkillSpecialEffect>()
        { new ChangeHitRateStatModifiersEffect(40,1,false) };

        //暴风542
        UseSkillEffectManager.UseSkillDic[542] = new List<IUseSkillSpecialEffect>()
        { new ConfusionEffect(30) };
    }
}
