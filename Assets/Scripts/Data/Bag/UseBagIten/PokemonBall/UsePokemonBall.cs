using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using MyUnityEventDispatcher;
using System.Collections;

namespace PokemonBattele
{
    class UsePokemonBall : UseBagItem
    {
        public UsePokemonBall
            (bool canUseInBattle,
            bool canUseOutBattle,
            bool NotNeedUseButEffect,
            bool UseWhilePokemonCarry) :
            base(canUseInBattle,canUseOutBattle,NotNeedUseButEffect, UseWhilePokemonCarry)
        {

        }
        public float CatachCorrection = 1;
        public string BagItemName = "精灵球";
        public override void Effect()
        {
            DebugHelper.LogFormat("你选择使用了{0}", BagItemName);

            var pokemon = BattleController.Instance.EnemyCurPokemonData;

            //精灵球和精灵球特效
            GameObject pokemonBallInPool = PokemonFactory.GetPokemonBall();
            pokemonBallInPool.transform.position =
                pokemon.transform.position +
                Vector3.up*3;
            PokemonFactory.PokemonBallEffect(pokemon.transform.position);

            //精灵捕捉率计算
            int shadeNum = 0;
            bool issuccess = false;
            CatchPokemon.CatchPokemonResult(
                pokemon,
                this.CatachCorrection,
                1
                ,out shadeNum,out issuccess);
            DebugHelper.LogFormat("正在捕捉精灵{0}，{3}振动了{1}次，最后捕捉{2}了",
                pokemon.Ename, shadeNum,issuccess?"成功":"失败", BagItemName);


            pokemon.transform.GetComponent<Rigidbody>().useGravity = false;
            //精灵变小
            pokemon.transform
                .DOScale(0.01f,0.5f)
                .OnComplete(
                ()=> 
                {
                    Sequence mySequence = DOTween.Sequence();

                    //精灵球振动
                    for (int i =0;i<shadeNum;++i)
                    {
                        mySequence.Append(pokemonBallInPool.transform.DOShakePosition(0.3f, new Vector3(-0.03f, 0.03f, 0.03f), 30));
                        mySequence.AppendInterval(0.2f);
                    }

                    mySequence.AppendCallback(() =>
                    {
                        if (issuccess)
                        {
                            //捕捉精灵
                            LHCoroutine.CoroutineManager.DoCoroutine(CatachPokeomon(pokemon.pokemon));

                            //通知结束对战
                            NotificationCenter<int>.Get().DispatchEvent("CatchPokemon", 1);


                            //显示捕捉结果UI
                            NotificationCenter<Pokemon>.Get().DispatchEvent("CatchPokemonResult", pokemon.pokemon);

                            NotificationCenter<bool>.Get().DispatchEvent("BattlePause", false);
                        }
                        else
                        {
                            pokemon.transform.DOScale(1, 0.5f).
                            OnComplete(() =>
                            {
                                pokemon.transform.GetComponent<Rigidbody>().useGravity = true;
                                PokemonFactory.StorePokemonBallInPool(pokemonBallInPool);
                                NotificationCenter<bool>.Get().DispatchEvent("BattlePause", false);
                            });
                            
                        }
                    });

                    
                }
            );

        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="pokemon"></param>
        /// <returns></returns>
        IEnumerator CatachPokeomon(Pokemon pokemon)
        {
            var context = Contexts.sharedInstance.game;
            yield return new WaitWhile
            (
                () =>
                {
                    return context.isBattleFlag;
                }
            );

            var trainer = context.playerData.scriptableObject;
            trainer.pokemons.Add(pokemon);           
            context.ReplacePlayerData(trainer);

        }


    }
}
