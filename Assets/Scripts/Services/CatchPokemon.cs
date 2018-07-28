using System;

namespace PokemonBattele
{
    public static partial class CatchPokemon
    {


        /// <summary>
        /// 捕获概率
        /// B在255以上时，捕获必定成功；B小于255时，进入判定
        /// </summary>
        /// <returns></returns>
        private static float GetCatchRate(BattlePokemonData pokemon, float CatachCorrection,float StateCorrection)
        {
            //（3*最大HP-2*当前HP）*捕获率*捕获修正*状态修正/(3*最大HP)
            //普通精灵球的捕获修正为1
            return (3 * pokemon.Health - 2 * pokemon.curHealth)
                * ResourceController.Instance.allCtachRateDic[pokemon.race.raceid]
                * CatachCorrection * StateCorrection
                / (3 * pokemon.Health);
        }

        public static void CatchPokemonResult(BattlePokemonData pokemon, float CatachCorrection, float StateCorrection, out int shaderNum, out bool issuccess)
        {
            shaderNum = 0; issuccess =true;
            float catchRate = GetCatchRate(pokemon, CatachCorrection, StateCorrection);
            if (catchRate >= 255)
            {
                issuccess = true;
                return;
            }
            else
            {

                //普通捕捉
                int G = Convert.ToInt32(GetPokemonShakeG(catchRate));
                int _max = Convert.ToInt32(65536);

                //计算摇晃次数
                for (shaderNum = 0; shaderNum < 3; shaderNum++)
                {
                    int ran = RandomService.game.Int(0,_max);
                    DebugHelper.LogFormat("捕捉{2}判定中 Random: {0};G: {1}", ran, G, pokemon.Ename);
                    if (ran > G)
                    {
                        issuccess = false;

                        break;

                    }
                }

            }
        }

        // <summary>
        /// 计算精灵球摇动次数判定值
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        private static double GetPokemonShakeG(float B)
        {
            double t = (255 / B);
            t = Math.Pow(t, 0.1875);
            return 65536 / t;
        }
    }
}

