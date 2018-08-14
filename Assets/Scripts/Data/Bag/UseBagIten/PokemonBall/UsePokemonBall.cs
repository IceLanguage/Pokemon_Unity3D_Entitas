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
        public UsePokemonBall(float CatachCorrection = 1,string BagItemName = "精灵球") :
            base(true,false,false,false,false, BagItemName)
        {
            this.CatachCorrection = CatachCorrection;
        }
        public float CatachCorrection { get; protected set; }

       
        protected virtual void ChangeCatachCorrection(BattlePokemonData pokemon) { }
        public override void Effect(BattlePokemonData pokemon)
        {
            
            DebugHelper.LogFormat("你选择使用了{0}", BagItemName);
            ChangeCatachCorrection(pokemon);

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
            
            DebugHelper.Log(
               new StringBuilder(40)
               .AppendFormat("正在捕捉精灵{0}，{1}振动了", pokemon.Ename, BagItemName)
               .Append(shadeNum)
               .AppendFormat("次，最后捕捉{0}了", issuccess ? "成功" : "失败")
               .ToString());

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
