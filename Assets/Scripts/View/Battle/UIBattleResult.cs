using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using LHCoroutine;

public class UIBattleResult : TTUIPage
{
    public UIBattleResult() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattleResult";
    }

    private RectTransform rectTransform;
    private Image image;
    public override void Awake(GameObject go)
    {
        rectTransform = transform.GetComponent<RectTransform>();
        image = transform.Find("Image").GetComponent<Image>();
    }

    public override void Active()
    {
        rectTransform.DOAnchorPosY(0, 1);
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosY(-1000, 1);
    }

    public override void Refresh()
    {
        if ( null != BattleController.Instance.PlayerCurPokemonData)
            image.sprite = Resources.Load<Sprite>("UIPrefab/BattleResult/win");
        else
            image.sprite = Resources.Load<Sprite>("UIPrefab/BattleResult/faill");

        CoroutineManager.DoCoroutine(PreHide());
    }

    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(4);
        Hide();
    }
}
