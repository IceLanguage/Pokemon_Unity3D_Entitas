using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;
using DG.Tweening;
using MyUnityEventDispatcher;

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

    

    private void InitUnityComponents()
    {
        if (null == thisrect )
            thisrect = gameObject.GetComponent<RectTransform>();

        if (null == BagButton )
        {
            BattleButton = transform.Find("Battle").GetComponent<Button>();
            if (BattleButton.onClick.GetPersistentEventCount() > 0)
                BattleButton.onClick.RemoveAllListeners();
            BattleButton.onClick.AddListener(Battle);
        }

        if (null == RunButton )
        {
            RunButton = this.transform.Find("Run").GetComponent<Button>();
            if (RunButton.onClick.GetPersistentEventCount() > 0)
                RunButton.onClick.RemoveAllListeners();
            RunButton.onClick.AddListener(Run);
        }

        if (null == BagButton )
        {
            BagButton = this.transform.Find("Bag").GetComponent<Button>();
            if (BagButton.onClick.GetPersistentEventCount() > 0)
                BagButton.onClick.RemoveAllListeners();
            BagButton.onClick.AddListener(OpenBag);
        }

        if (null == BallButton)
        {
            BallButton = this.transform.Find("Ball").GetComponent<Button>();
            if (BallButton.onClick.GetPersistentEventCount() > 0)
                BallButton.onClick.RemoveAllListeners();
            BallButton.onClick.AddListener(CallPokemon);
        }
    }

   

    public void Battle()
    {
        TTUIPage.ClosePage<UIKnapscakPanel>();
        TTUIPage.ClosePage<UIBattle_Pokemons>();
        TTUIPage.ShowPage<UIBattle_Skills>();
    }

    public void Run()
    {
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
