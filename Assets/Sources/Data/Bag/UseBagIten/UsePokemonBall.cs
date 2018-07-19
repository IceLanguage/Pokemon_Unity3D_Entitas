using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using MyUnityEventDispatcher;

namespace PokemonBattele
{
    class UsePokemonBall : UseBagItem
    {
        public float CatachCorrection = 1;
        public override void Effect()
        {
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
            Debug.Log("捕捉精灵" + pokemon.Ename +
                "振动" + shadeNum + "次" + issuccess);

            //精灵球振动
            //TODO

            //精灵变小
            pokemon.transform
                .DOScale(0.1f,BattleController.Instance.BattleInterval/2)
                .OnComplete(
                ()=> 
                {
                    
                    if(issuccess)
                    {
                        //对战结束
                        var context = Contexts.sharedInstance.game;
                        var trainer = context.playerData.scriptableObject;
                        trainer.pokemons.Add(pokemon.pokemon);
                        context.ReplacePlayerData(trainer);

                        //通知结束对战
                        NotificationCenter<int>.Get().DispatchEvent("CatchPokemon", 1);

                        //显示捕捉结果UI
                        NotificationCenter<Pokemon>.Get().DispatchEvent("CatchPokemonResult", pokemon.pokemon);
                    }
                    else
                    {
                        pokemon.transform.DOScale(1, BattleController.Instance.BattleInterval / 5);
                    }
                }
            );

        }
    }
}
