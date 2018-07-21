using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattele
{
    /// <summary>
    /// 同步（特性）
    /// </summary>
    class Synchronize :AbilityImpact
    {
        public override void OnGetAbormal(BattlePokemonData self, AbnormalStateEnum newstate, ref bool canGet)
        {
            if (AbnormalStateEnum.Normal == self.Abnormal)
            {
                if (BattleController.Instance.PlayerCurPokemonData != self)
                {
                    if (AbnormalStateEnum.Normal == BattleController.Instance.PlayerCurPokemonData.Abnormal)
                    {
                        BattleController.Instance.PlayerCurPokemonData.SetAbnormalStateEnum(newstate);
                        DebugHelper.LogFormat("{0}因为具有同步特性,异常状态对对手{1}同步", self.Ename, BattleController.Instance.PlayerCurPokemonData.Ename);
                    }

                }
                else
                {
                    if (AbnormalStateEnum.Normal == BattleController.Instance.EnemyCurPokemonData.Abnormal)
                    {
                        BattleController.Instance.EnemyCurPokemonData.SetAbnormalStateEnum(newstate);
                        DebugHelper.LogFormat("{0}因为具有同步特性,异常状态对对手同步", self.Ename, BattleController.Instance.EnemyCurPokemonData.Ename);
                    }
                }
            }
        }

        public override void OnEncounterPokemon(BattlePokemonData self, Pokemon pokemon)
        {
            if(RandomService.game.Int(0,100)<50)
            {
                pokemon.PokeNature = self.nature;
                DebugHelper.LogFormat("因为队首宝可梦{0}同步特性激发,遇到的的野生宝可梦的性格变成与队首宝可梦{0}一样，是{1}", self.Ename,self.nature.NatureName);
            }
        }
    }
}
