using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_PokemonBagIcon:TTUIPage
{
    public UI_PokemonBagIcon() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UI_PokemonBagIcon";
    }

    private Button button;
    private RectTransform rectTransform;
    public override void Awake(GameObject go)
    {
        button = gameObject.GetComponent<Button>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        button.onClick.AddListener(OpenBag);
    }
    public override void Active()
    {
        rectTransform.DOAnchorPosY(-240, 0.1f);
    }
    public override void Hide()
    {
        rectTransform.DOAnchorPosY(-750, 0.1f);
    }
    private void OpenBag()
    {
        var e = ResourceController.Instance;
        if (!e.LOADITEM || !e.LOADSKILL || !e.LOADSKILLPOOL) return;
        ShowPage<UI_PokemonBag>();
    }
}
