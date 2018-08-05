using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TinyTeam.UI;
using MyUnityEventDispatcher;
using DG.Tweening;

public struct ToolTipMessage
{
    public string str;
    public Vector2 position;
    public ToolTipMessage(string message,Vector2 position)
    {
        str = message;
        this.position = position;
    }
}
public class UIToolTip :TTUIPage
{
    public UIToolTip() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIToolTip";
    }
    private float smothing = 0.5f;//用于显示和隐藏的插值运输
    private Text toolTipText;//提示框的父Text，主要用来控制提示框的大小
    private Text contentText;//提示框的子Text，主要用来显示提示
    private CanvasGroup toolTipCanvasGroup;//提示框的CanvasGroup组件，用来制作显示和隐藏功能
    //private float targetAlpha = 0.0f;//设置提示框的Alpha值，0代表隐藏，1代表显示
    public float targetAlpha
    {
        get
        {
            return toolTipCanvasGroup.alpha;
        }
        set
        {
            toolTipCanvasGroup.alpha = value;
        }
    }
    private RectTransform rectTransform;

    public override void Awake(GameObject go)
    {
        rectTransform = transform.GetComponent<RectTransform>();
        
        toolTipText = transform.GetComponent<Text>();
        contentText = transform.Find("Content").GetComponent<Text>();
        toolTipCanvasGroup =transform.GetComponent<CanvasGroup>();
        this.toolTipText.text = "";
        this.contentText.text = "";
        NotificationCenter<ToolTipMessage>.Get().AddEventListener("UIToolTip", Show);
        LHCoroutine.CoroutineManager.DoCoroutine(waitHide());
    }
    
    



    public override void Active()
    {
        
        if(contentText.text!=null&& contentText.text.Length>0)
            this.targetAlpha = 1;
    }

    //提示框的隐藏方法
    public override void Hide()
    {
        LHCoroutine.CoroutineManager.DoCoroutine(waitHide());
    }

    //设置提示框自身的位置
    public void SetLocalPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void Show(Notification<ToolTipMessage> notific)
    {
        var param = notific.param;
        
        Show(param);
    }

    //提示框的显示方法
    public void Show(ToolTipMessage message)
    {
       
        string text = message.str;
        this.toolTipText.text = text;
        this.contentText.text = text;
        if (text != null&& 0 == text.Length)
        {
            targetAlpha = 0;
            return;
        }
        SetLocalPosition(message.position);

        targetAlpha = 1.0f;

        Hide();
       
    }

    IEnumerator waitHide()
    {
        while (0 != targetAlpha)
        {
            targetAlpha = Mathf.Lerp(targetAlpha, 0, smothing * Time.deltaTime);
            if (Mathf.Abs(targetAlpha - 0) < 0.01f)//如果当前提示框的Alpha值与目标Alpha值相差很小，那就设置为目表值
            {
                targetAlpha = 0;
                
            }
            else
            {
                yield return  new WaitForSeconds(0.05f);
            }
        }
        
    }

}
