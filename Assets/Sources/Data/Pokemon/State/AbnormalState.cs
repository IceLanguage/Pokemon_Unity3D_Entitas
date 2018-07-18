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
    public enum AbnormalState
    {
        Normal,
        Burns,
        Frostbite,
        Poisoning,
        Sleeping,
        Paralysis
    }

    //灼烧
    class BurnsState : PokemonState
    {

        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {

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

            if (AbnormalState.Normal == pokemon.Abnormal)
            {
                PokemonState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalState.Burns;
                Debug.Log(pokemon.Ename + "灼烧了");
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
                pokemon.SetAbnormalState(AbnormalState.Normal);
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

            if (AbnormalState.Normal == pokemon.Abnormal)
            {
                PokemonState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalState.Frostbite;
                Debug.Log(pokemon.Ename + "冰冻了");
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
            return RandomService.game.Float(0, 4) < 1;
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

            if (AbnormalState.Normal == pokemon.Abnormal)
            {
                PokemonState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalState.Paralysis;
                Debug.Log(pokemon.Ename + "麻痹了");
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

            if (AbnormalState.Normal == pokemon.Abnormal)
            {
                PokemonState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
                pokemon.Abnormal = AbnormalState.Poisoning;
                Debug.Log(pokemon.Ename + "中毒了");
            }
        }


        public override void UpdateInPlayerAround(BattlePokemonData pokemon)
        {

            pokemon.curHealth -= pokemon.Health / 8;
            if (pokemon.curHealth < 0)
                pokemon.curHealth = 0;

            if (1 == pokemon.curHealth)
            {
                pokemon.SetAbnormalState(AbnormalState.Normal);
            }
        }
    }

    //睡眠
    public class SleepState : PokemonState
    {
        public static Dictionary<int, int> SleepTimeDict = new
            Dictionary<int, int>();
        public override void Init(BattlePokemonData pokemon)
        {
            PokemonState.Abnormalstates[pokemon.Abnormal].LoseState(pokemon);
            pokemon.Abnormal = AbnormalState.Sleeping;
            Debug.Log(pokemon.Ename + "睡着了");
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
                pokemon.SetAbnormalState(AbnormalState.Normal);
        }

        public override bool CanAction(BattlePokemonData pokemon)
        {
            return 0 == SleepTimeDict[pokemon.ID];
        }
    }
}