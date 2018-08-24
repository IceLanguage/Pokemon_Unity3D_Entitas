using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 异常状态
    /// </summary>
    [Serializable]
    public enum AbnormalStateEnum
    {
        Normal,
        Burns,
        Frostbite,
        Poisoning,
        Sleeping,
        Paralysis,
        BadlyPoison
    }

    public class AbnormalStateEnum_EnumComare : IEqualityComparer<AbnormalStateEnum>
    {
        public bool Equals(AbnormalStateEnum x, AbnormalStateEnum y)
        {
            return x == y;
        }

        public int GetHashCode(AbnormalStateEnum obj)
        {
            return (int)obj;
        }
    }
    /// <summary>
    /// 异常变化
    /// </summary>
    abstract class AbnormalState : PokemonState
    {
        public static Dictionary<AbnormalStateEnum, PokemonState> Abnormalstates = new
            Dictionary<AbnormalStateEnum, PokemonState>();

    }

    class NormalAbnormalState: PokemonState
    {
        public override void Init(BattlePokemonData pokemon)
        {
            AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
            if(AbnormalStateEnum.Normal!=pokemon.Abnormal)
                DebugHelper.LogFormat("{0}异常状态恢复了", pokemon.Ename);
            pokemon.Abnormal = AbnormalStateEnum.Normal;
            
            

        }
    }

    //灼烧
    class BurnsState : PokemonState
    {

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            

            int h = pokemon.Health / 16;
            h = Math.Min(h, pokemon.curHealth);
            //考虑特性
            if (AbilityManager.AbilityImpacts.ContainsKey(pokemon.ShowAbility))
                AbilityManager.AbilityImpacts[pokemon.ShowAbility]
                    .OnAbnomalCauseDamage
                    (
                        pokemon, ref h
                    );
            DebugHelper.Log(
                new StringBuilder(40)
                .AppendFormat("{0}因为烧伤受到了", pokemon.Ename)
                .Append(h)
                .Append("伤害")
                .ToString());
            
            pokemon.curHealth -= h;

        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            pokemon.increase.physicPower*= 2;
            DebugHelper.LogFormat("{0}灼烧状态解除,物攻恢复了", pokemon.Ename);
        }

        public override void Init(BattlePokemonData pokemon)
        {
            if (PokemonType.火 == pokemon.MainPokemonType ||
                PokemonType.火 == pokemon.SecondPokemonType)
                return;

            //特性检查
            //TODO
            //技能状态检查
            //TODO

            if (AbnormalStateEnum.Normal == pokemon.Abnormal)
            {
                AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalStateEnum.Burns;
                DebugHelper.LogFormat("{0}灼烧了,物攻降低了一半", pokemon.Ename);

                pokemon.increase.physicPower /= 2;
            }

        }


    }

    //冰冻
    class FrostbiteState : PokemonState
    {
        public override bool CanAction(BattlePokemonData pokemon)
        {
            bool isfalg = RandomService.game.Float(0, 5) < 1;
            if (isfalg)
            {
                DebugHelper.LogFormat("{0}冰冻状态解除", pokemon.Ename);
                pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Normal);
            }
            else
            {
                DebugHelper.LogFormat("{0}冰冻了无法行动", pokemon.Ename);
        
            }
            return isfalg;
        }

        public override void Init(BattlePokemonData pokemon)
        {
            if (PokemonType.冰 == pokemon.MainPokemonType ||
                PokemonType.冰 == pokemon.SecondPokemonType)
                return;

            //特性检查
            //TODO
            //技能状态检查
            //TODO

            if (AbnormalStateEnum.Normal == pokemon.Abnormal)
            {
                AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalStateEnum.Frostbite;
                DebugHelper.LogFormat("{0}冰冻了", pokemon.Ename);
         
            }

        }
    }

    //麻痹
    class ParalysisState : PokemonState
    {



        public override void LoseState(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}麻痹状态解除，速度恢复了", pokemon.Ename);
            
            if (AbilityType.飞毛腿 != pokemon.ShowAbility)
            {
                //pokemon.increase.speed /= 1.5f;
            }
            else
            {
                pokemon.increase.speed *= 2;
            }

        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            bool flag = RandomService.game.Float(0, 4) < 1;
            if(!flag)
            {
                DebugHelper.LogFormat("{0}麻痹了无法行动", pokemon.Ename);
               
            }
            return flag;
        }

        public override void Init(BattlePokemonData pokemon)
        {
            if (PokemonType.电 == pokemon.MainPokemonType ||
                PokemonType.电 == pokemon.SecondPokemonType)
                return;

            //特性检查
            //TODO
            //技能状态检查
            //TODO

            if (AbnormalStateEnum.Normal == pokemon.Abnormal)
            {
                AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalStateEnum.Paralysis;

                
                if (AbilityType.飞毛腿 == pokemon.ShowAbility)
                {
                    //pokemon.increase.speed *=1.5f;
                    DebugHelper.LogFormat("{0}麻痹了，{0}具有飞毛腿特性，无视麻痹的速度降低至50%的效果", pokemon.Ename);
                }
                else
                {
                    pokemon.increase.speed /= 2;
                    DebugHelper.LogFormat("{0}麻痹了，速度减半", pokemon.Ename);
                }

            }
        }
    }

    //中毒
    public class PoisonState : PokemonState
    {
        public override void Init(BattlePokemonData pokemon)
        {
            if (PokemonType.毒 == pokemon.MainPokemonType ||
                PokemonType.毒 == pokemon.SecondPokemonType ||
                PokemonType.钢 == pokemon.MainPokemonType ||
                PokemonType.钢 == pokemon.SecondPokemonType)
                return;

            //特性检查
            //TODO
            //技能状态检查
            //TODO

            if (AbnormalStateEnum.Normal == pokemon.Abnormal)
            {
                AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalStateEnum.Poisoning;
                DebugHelper.LogFormat("{0}中毒了", pokemon.Ename);
                
            }
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}中毒状态解除", pokemon.Ename);
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            
            int h = pokemon.Health / 8;
            h = Math.Min(h, pokemon.curHealth);
            //考虑特性
            if (AbilityManager.AbilityImpacts.ContainsKey(pokemon.ShowAbility))
                AbilityManager.AbilityImpacts[pokemon.ShowAbility]
                    .OnAbnomalCauseDamage
                    (
                        pokemon, ref h
                    );
            DebugHelper.Log(
               new StringBuilder(40)
               .AppendFormat("{0}因为中毒受到了", pokemon.Ename)
               .Append(h)
               .Append("伤害")
               .ToString());
            pokemon.curHealth -= h;

            if (1 == pokemon.curHealth)
            {
                pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Normal);
            }
        }
    }

    //剧毒
    public class BadlyPoisonState : PokemonState
    {
        public static Dictionary<int, int> count = new Dictionary<int, int>();
        public override void Init(BattlePokemonData pokemon)
        {
            if (PokemonType.毒 == pokemon.MainPokemonType ||
                PokemonType.毒 == pokemon.SecondPokemonType ||
                PokemonType.钢 == pokemon.MainPokemonType ||
                PokemonType.钢 == pokemon.SecondPokemonType)
                return;

            //特性检查
            //TODO
            //技能状态检查
            //TODO

            if (AbnormalStateEnum.Normal == pokemon.Abnormal)
            {
                AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalStateEnum.BadlyPoison;
                DebugHelper.LogFormat("{0}中剧毒了", pokemon.Ename);
                

                count[pokemon.ID] = 1;
            }
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            count.Remove(pokemon.ID);
            DebugHelper.LogFormat("{0}剧毒状态解除", pokemon.Ename);
        }
        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            int h = pokemon.Health / 16 * (count[pokemon.ID]);
            h = Math.Min(h, pokemon.curHealth);
            //考虑特性
            if (AbilityManager.AbilityImpacts.ContainsKey(pokemon.ShowAbility))
                AbilityManager.AbilityImpacts[pokemon.ShowAbility]
                    .OnAbnomalCauseDamage
                    (
                        pokemon, ref h
                    );
            DebugHelper.Log(
                new StringBuilder(40)
                .AppendFormat("{0}因为剧毒受到了", pokemon.Ename)
                .Append(h)
                .Append("伤害,当前剧毒时间计数为")
                .Append(count[pokemon.ID]++)
                .ToString());
            pokemon.curHealth -= h;

        }
    }

    //睡眠
    public class SleepState : PokemonState
    {
        public static Dictionary<int, int> SleepTimeDict = new
            Dictionary<int, int>();
        public override void Init(BattlePokemonData pokemon)
        {
            AbnormalState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
            pokemon.Abnormal = AbnormalStateEnum.Sleeping;
            DebugHelper.Log(
                new StringBuilder(40)
                .AppendFormat("{0}睡着了,还有", pokemon.Ename)
                .Append(SleepTimeDict[pokemon.ID])
                .Append("回合醒来")
                .ToString());


            SleepTimeDict[pokemon.ID] = RandomService.game.Int(1, 4);
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            DebugHelper.LogFormat("{0}醒来了", pokemon.Ename);
            SleepTimeDict.Remove(pokemon.ID);
        }

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            SleepTimeDict[pokemon.ID] -= 1;
            if(0 == SleepTimeDict[pokemon.ID])
                pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Normal);
        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            bool flag = 0 == SleepTimeDict[pokemon.ID];
            if(!flag)
            {
                DebugHelper.Log(
                 new StringBuilder(40)
                 .AppendFormat("{0}还在睡觉不能行动,还有", pokemon.Ename)
                 .Append(SleepTimeDict[pokemon.ID])
                 .Append("回合醒来")
                 .ToString());

            }
            return flag;
        }
    }
}