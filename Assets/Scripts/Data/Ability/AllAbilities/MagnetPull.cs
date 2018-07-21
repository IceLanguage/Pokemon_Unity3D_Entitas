using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PokemonBattele
{
    /// <summary>
    /// 磁力（特性）
    /// </summary>
    class MagnetPull :AbilityImpact
    {
        public bool Can(BattlePokemonData pokemon)
        {
            if (PokemonType.钢 == BattleController.Instance.PlayerCurPokemonData.MainPokemonType ||
                    PokemonType.钢 == BattleController.Instance.PlayerCurPokemonData.SecondPokemonType)
            {
                if (PokemonType.幽灵 != BattleController.Instance.PlayerCurPokemonData.MainPokemonType &&
                    PokemonType.幽灵 != BattleController.Instance.PlayerCurPokemonData.SecondPokemonType)
                    return true;
            }
            return false;
        }
        public override void OnBeCalled(BattlePokemonData self)
        {
            if(self == BattleController.Instance.EnemyCurPokemonData)
            {
                if(Can(BattleController.Instance.PlayerCurPokemonData))
                {
                    BattleController.Instance.PlayerCurPokemonData.AddChangeState(ChangeStateEnumForPokemon.CanNotEscape);
                    DebugHelper.LogFormat(
                        "{0}具有磁力特性,对方非幽灵属性的钢属性宝可梦{1}进入无法逃走状态，无法替换或逃走",
                        self.Ename, BattleController.Instance.PlayerCurPokemonData.Ename);
                }
                
            }
            else if (self == BattleController.Instance.PlayerCurPokemonData)
            {
                if (Can(BattleController.Instance.EnemyCurPokemonData))
                {
                    BattleController.Instance.EnemyCurPokemonData.AddChangeState(ChangeStateEnumForPokemon.CanNotEscape);
                    DebugHelper.LogFormat(
                        "{0}具有磁力特性,对方非幽灵属性的钢属性宝可梦{1}进入无法逃走状态，无法替换或逃走",
                        self.Ename, BattleController.Instance.EnemyCurPokemonData.Ename);
                }
            }
        }

        public override void OnLeave(BattlePokemonData self)
        {
            if (self == BattleController.Instance.EnemyCurPokemonData)
            {
                if (Can(BattleController.Instance.PlayerCurPokemonData))
                {
                    BattleController.Instance.PlayerCurPokemonData.RemoveChangeState(ChangeStateEnumForPokemon.CanNotEscape);
                    DebugHelper.LogFormat(
                        "具有磁力特性的{0}已下场，{1}可以逃走了",
                        self.Ename, BattleController.Instance.PlayerCurPokemonData.Ename);
                }
            }
            else if (self == BattleController.Instance.PlayerCurPokemonData)
            {
                if (Can(BattleController.Instance.EnemyCurPokemonData))
                {
                    BattleController.Instance.EnemyCurPokemonData.RemoveChangeState(ChangeStateEnumForPokemon.CanNotEscape);
                    DebugHelper.LogFormat(
                        "具有磁力特性的{0}已下场，{1}可以逃走了",
                        self.Ename, BattleController.Instance.EnemyCurPokemonData.Ename);
                }
            }
        }

        public override EncounterPokemon OnEncounterPokemon(BattlePokemonData self, EncounterPokemon encounterPokemon)
        {
            var res = ScriptableObject.CreateInstance<EncounterPokemon>();
            res.Pokemoms = encounterPokemon.Pokemoms;
            res.EncounterProbabilities = encounterPokemon.EncounterProbabilities;
            int len = Math.Min(res.Pokemoms.Count, res.EncounterProbabilities.Count);
            while(res.Pokemoms.Count>len)
            {
                res.Pokemoms.RemoveAt(res.Pokemoms.Count - 1);
            }
            while (res.EncounterProbabilities.Count > len)
            {
                res.EncounterProbabilities.RemoveAt(res.EncounterProbabilities.Count - 1);
            }
            for (int i=0;i<len;++i)
            {
                Race race = ResourceController.Instance.allRaceDic[res.Pokemoms[i]];
                if (PokemonType.钢 == race.pokemonMainType|| PokemonType.钢 == race.pokemonSecondType)
                {
                    res.EncounterProbabilities[i] = (int)(res.EncounterProbabilities[i] * 1.5f);
                }
            }
            DebugHelper.LogFormat("{0}具有磁力特性,在队伍首位时，如果所在区域存在野生的钢属性宝可梦，出现钢属性野生宝可梦的几率上升50%",
                self.Ename);
            return res;
        }
    }
}
