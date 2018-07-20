using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;
using DG.Tweening;
using MyUnityEventDispatcher;
using PokemonBattele;

public class UIBattle_Buttons : TTUIPage {

    public UIBattle_Buttons() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_Buttons";
    }
    private RectTransform  thisrect;
    private Button BattleButton, RunButton, BagButton, BallButton;
   
    public override void Awake(GameObject go)
    {

        InitUnityComponents();
        BattleStateForPlayer.InitEvent += PlayerRound;
        BattleStateForBattle.InitEvent += BattleRound;
        Refresh();
    }
    public override void Active()
    {
        thisrect.DOAnchorPosX(-15, 1);
    }

    public override void Hide()
    {

        thisrect.DOAnchorPosX(350, 1);
    }

    private void PlayerRound()
    {
        BallButton.onClick.AddListener(CallPokemon);
        RunButton.onClick.AddListener(Run);
        BagButton.onClick.AddListener(OpenBag);
    }
    private void BattleRound()
    {
        BallButton.onClick.RemoveAllListeners();
        RunButton.onClick.RemoveAllListeners();
        BagButton.onClick.RemoveAllListeners();
    }

    private void InitUnityComponents()
    {
        thisrect = gameObject.GetComponent<RectTransform>();

        BattleButton = transform.Find("Battle").GetComponent<Button>();
        BattleButton.onClick.AddListener(Battle);

        RunButton = this.transform.Find("Run").GetComponent<Button>();


        BagButton = this.transform.Find("Bag").GetComponent<Button>();

        BallButton = this.transform.Find("Ball").GetComponent<Button>();
    
    }



    public void Battle()
    {
        TTUIPage.ClosePage<UIKnapscakPanel>();
        TTUIPage.ClosePage<UIBattle_Pokemons>();
        TTUIPage.ShowPage<UIBattle_Skills>();
    }

    public void Run()
    {
        DebugHelper.Log("你选择了逃跑，对方取得了战斗的胜利");
        Contexts.sharedInstance.game.isBattleFlag = false;
    }

    public void OpenBag()
    {
        
        TTUIPage.ClosePage<UIBattle_Skills>();
        TTUIPage.ClosePage<UIBattle_Pokemons>();
        TTUIPage.ShowPage<UIKnapscakPanel>();

    }

    public void CallPokemon()
    {
        TTUIPage.ClosePage<UIKnapscakPanel>();
        TTUIPage.ClosePage<UIBattle_Skills>();
        TTUIPage.ShowPage<UIBattle_Pokemons>();

    }

    
}
