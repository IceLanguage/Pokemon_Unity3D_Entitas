using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
using UnityEngine.UI;
using DG.Tweening;

public class UI_BagIcon : TTUIPage {

    public UI_BagIcon() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UI_BagIcon";
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
        ShowPage<UIKnapscakPanel>();
    }
}
