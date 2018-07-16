using DG.Tweening;
using LHCoroutine;
using System.Collections;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle_ShowEnemySkill : TTUIPage
{
    public UIBattle_ShowEnemySkill() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_ShowEnemySkill";
    }
    private RectTransform rectTransform;
    private Text text;
    public override void Awake(GameObject go)
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        text = gameObject.GetComponent<Text>();
        text.text = "";
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Active()
    {
        rectTransform.DOAnchorPosY(80, 0.1f);
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosY(2210, 1);
        text.text = "";
    }
    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(2);
        Hide();
    }

    public void Show(string skillname)
    {
        text.text = skillname;
        Show();
    }

}

public class UIBattle_ShowPlaySkill : TTUIPage
{
    public UIBattle_ShowPlaySkill() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_ShowPlaySkill";
    }
    private RectTransform rectTransform;
    private Text text;
    public override void Awake(GameObject go)
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        text = gameObject.GetComponent<Text>();
        text.text = "";
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Active()
    {
        rectTransform.DOAnchorPosY(80, 0.1f);
        CoroutineManager.DoCoroutine(PreHide());
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosY(2210, 1);
        text.text = "";
    }
    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(2);
        Hide();
    }

    public void Show(string skillname)
    {
        text.text = skillname;
        Show();
    }

}