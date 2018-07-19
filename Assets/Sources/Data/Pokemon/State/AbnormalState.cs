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
            pokemon.Abnormal = AbnormalStateEnum.Normal;
            Debug.Log(new StringBuilder(30)
                .AppendFormat("{0}异常状态恢复了 {1}",
                    pokemon.Ename , System.DateTime.Now)
                .ToString());
        }
    }

    //灼烧
    class BurnsState : PokemonState
    {

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            Debug.Log(new StringBuilder(30)
                .AppendFormat("{0}因为烧伤受到了伤害 {1}",
                    pokemon.Ename, System.DateTime.Now)
                .ToString());
            pokemon.curHealth -= pokemon.Health / 16;
            if (pokemon.curHealth < 0)
                pokemon.curHealth = 0;
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
            pokemon.PhysicPower *= 2;
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
                Debug.Log(new StringBuilder(30)
                .AppendFormat("{0}灼烧了 {1}",
                    pokemon.Ename, System.DateTime.Now)
                .ToString());
                pokemon.PhysicPower /= 2;
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
                pokemon.SetAbnormalStateEnum(AbnormalStateEnum.Normal);
            }
            else
            {
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}冰冻了无法行动 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
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
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}冰冻了 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
            }

        }
    }

    //麻痹
    class ParalysisState : PokemonState
    {



        public override void LoseState(BattlePokemonData pokemon)
        {
            pokemon.Speed *= 2;
        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            bool flag = RandomService.game.Float(0, 4) < 1;
            if(!flag)
            {
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}麻痹了无法行动 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
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
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}麻痹了 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
                pokemon.Speed /= 2;
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
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}中毒了 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
            }
        }


        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}因为中毒受到了伤害 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
            pokemon.curHealth -= pokemon.Health / 8;
            if (pokemon.curHealth < 0)
                pokemon.curHealth = 0;

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
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}中剧毒了 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());

                count[pokemon.ID] = 1;
            }
        }


        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {
            Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}因为剧毒受到了伤害 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
            pokemon.curHealth -= pokemon.Health / 16 * (count[pokemon.ID]++);
            if (pokemon.curHealth < 0)
                pokemon.curHealth = 0;


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
            Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}睡着了 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());

            SleepTimeDict[pokemon.ID] = RandomService.game.Int(1, 4);
        }

        public override void LoseState(BattlePokemonData pokemon)
        {
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
                Debug.Log(new StringBuilder(30)
               .AppendFormat("{0}还在睡觉不能行动 {1}",
                   pokemon.Ename, System.DateTime.Now)
               .ToString());
            }
            return flag;
        }
    }
}