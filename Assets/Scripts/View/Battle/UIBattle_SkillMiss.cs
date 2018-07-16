using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using LHCoroutine;

public class UIBattle_PlaySkillMiss : TTUIPage
{
    public UIBattle_PlaySkillMiss() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_PlaySkillMiss";
    }
    private RectTransform rectTransform;
    public override void Awake(GameObject go)
    {
        rectTransform=gameObject.GetComponent<RectTransform>();
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Active()
    {
        rectTransform.DOAnchorPosY(210, 0.1f);
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosY(2210, 1);
    }
    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(1);
        Hide();
    }
}

public class UIBattle_EnemySkillMiss : TTUIPage
{
    public UIBattle_EnemySkillMiss() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_EnemySkillMiss";
    }
    private RectTransform rectTransform;
    public override void Awake(GameObject go)
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Active()
    {
        rectTransform.DOAnchorPosY(210, 0.1f);
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosY(2210, 1);
    }
    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(1);
        Hide();
    }
}
