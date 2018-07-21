using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 静电（特性)
    /// </summary>
    class Static:AbilityImpact
    {
        public override void OnBeAttacked(BattlePokemonData self, Skill skill, BattlePokemonData attackPokemon, ref int avoid)
        {
           if(SkillType.物理 == skill.type&&attackPokemon.Abnormal!=AbnormalStateEnum.Normal)
            {
                if(RandomService.game.Int(0,100)<30)
                {
                    DebugHelper.LogFormat("{0}触发了{1}的静态特性，{0}麻痹了",attackPokemon.Ename,self.Ename);
                    attackPokemon.SetAbnormalStateEnum(AbnormalStateEnum.Paralysis);
                }
            }
        }
        public override EncounterPokemon OnEncounterPokemon(BattlePokemonData self, EncounterPokemon encounterPokemon)
        {
            var res = ScriptableObject.CreateInstance<EncounterPokemon>();
            res.Pokemoms = encounterPokemon.Pokemoms;
            res.EncounterProbabilities = encounterPokemon.EncounterProbabilities;
            int len = Math.Min(res.Pokemoms.Count, res.EncounterProbabilities.Count);
            while (res.Pokemoms.Count > len)
            {
                res.Pokemoms.RemoveAt(res.Pokemoms.Count - 1);
            }
            while (res.EncounterProbabilities.Count > len)
            {
                res.EncounterProbabilities.RemoveAt(res.EncounterProbabilities.Count - 1);
            }
            for (int i = 0; i < len; ++i)
            {
                Race race = ResourceController.Instance.allRaceDic[res.Pokemoms[i]];
                if (PokemonType.电 == race.pokemonMainType || PokemonType.电 == race.pokemonSecondType)
                {
                    res.EncounterProbabilities[i] = (int)(res.EncounterProbabilities[i] * 1.5f);
                }
            }
            DebugHelper.LogFormat("{0}具有静电特性,在队伍首位时，如果所在区域存在野生的电属性宝可梦，出现钢属性野生宝可梦的几率上升50%",
                self.Ename);
            return res;
        }
    }
}
