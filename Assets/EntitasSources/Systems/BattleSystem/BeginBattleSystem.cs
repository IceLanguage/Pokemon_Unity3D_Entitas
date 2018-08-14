using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using PokemonBattele;
using UnityEngine;

public class BeginBattleSystem : ReactiveSystem<GameEntity>, IEndBattleEvent
{
    readonly GameContext context;
    public static Action BeginBattleEvent;
    private readonly PlayerPrefs playerPrefs = new PlayerPrefs();
    public BeginBattleSystem(Contexts contexts):base (contexts.game)
    {
        context = contexts.game;
        EndBattleSystem.EndBattleEvent += EndBattleEvent;
    }

    public void EndBattleEvent()
    {
        TOUCH_Controller.Instance.EnablewalkStick();

        PlayerController.Instance.transform.position = new PlayerPrefs().GetVector3
            ("playPrePos");
    }

    protected override void Execute(List<GameEntity> entities)
    {
        //屏幕扭曲特效
        SpecialEffect.ScreenDistorted(context, BeginBattle);

        //禁止玩家行动
        TOUCH_Controller.Instance.DisablewalkStick();
        TOUCH_Controller.Instance.EnableroatateStick();

        //存储玩家当前位置
        playerPrefs.SetVector3("playPrePos",
            PlayerController.Instance.transform.position);
        
    }

    private void BeginBattle()
    {        
        //初始化战斗场景精灵信息
        List<Pokemon> playPokemons = context.playerData.scriptableObject.pokemons;

        List<Pokemon> PreBattlePokemon = playPokemons.Count >= 6 ?
            playPokemons.GetRange(0, 6) : playPokemons;
        BattleController.Instance.InitPlayerPokemons(PreBattlePokemon);

       

        if (BattleController.Instance.CanBattle)
        {
            BeginBattleEvent();

            //玩家进入战斗场景
            PlayerController.Instance.transform.position =
                BattleController.Instance.PlayerTransform.position;
            //开始战斗
            BattleController.Instance.BeginBattle();
        }
        else
        {
            context.isBattleFlag = false;

        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.BattleFlag);
    }

}
