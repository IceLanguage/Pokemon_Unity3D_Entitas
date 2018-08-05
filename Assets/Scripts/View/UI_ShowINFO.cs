using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;
using MyUnityEventDispatcher;
using System.Collections;

class UI_ShowINFO : TTUIPage
{
    public UI_ShowINFO() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UI_ShowINFO";
    }
    private Text text;
    private Stack<string> Messages = new Stack<string>();
    private int count = 0;
    public override void Awake(GameObject go)
    {
        text = transform.Find("Text").GetComponent<Text>();
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        NotificationCenter<string>.Get().AddEventListener("ShowINFO", ShowINFO);
        LHCoroutine.CoroutineManager.DoCoroutine(showMessage());
        Application.logMessageReceived += OnLogMessage;
    }
    private void ShowINFO(Notification<string> notific)
    {
        Messages.Push(notific.param);
    }
    IEnumerator showMessage()
    {
        yield return new WaitForSeconds(0.3f);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        
        if (Messages.Count > 0)
        {
            if(count%2==0)
                text.text = Messages.Pop();
            else
                text.text+= Messages.Pop();
            count++;
        }
            
        LHCoroutine.CoroutineManager.DoCoroutine(showMessage());
    }

    private static void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
            NotificationCenter<string>.Get().DispatchEvent("ShowINFO", condition);
    }
}
