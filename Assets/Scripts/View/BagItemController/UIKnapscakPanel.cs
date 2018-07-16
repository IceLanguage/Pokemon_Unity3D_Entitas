using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using MyUnityEventDispatcher;

public class UIKnapscakPanel:TTUIPage
{
    public UIKnapscakPanel() :base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIKnapscakPanel";
    }
    private List<Button> BagSolt_list = new List<Button>();
    private List<BagItem> bagItem_list = new List<BagItem>();
    private RectTransform rectTransform;
    private Button CloseButton;
    public override void Awake(GameObject go)
    {
        InitUnityComponents();
    }

    private void InitUnityComponents()
    {
        CloseButton = transform.Find("Close").GetComponent<Button>();
        CloseButton.onClick.AddListener(ClosePage<UIKnapscakPanel>);
        rectTransform = transform.GetComponent<RectTransform>();

        for (int i = 0; i < 24; i++)
        {
            var button = transform.Find("SlotPanel/Slot (" + i.ToString() + ")").GetComponent<Button>();
            BagSolt_list.Add(button);
            var item = button.transform.GetComponent<BagItem>();
            
            bagItem_list.Add(item);
        }
    }
    public override void Active()
    {
        //ClosePage<UI_PokemonBag>();
        rectTransform.DOAnchorPosX(-240, 1);
    }

    public override void Hide()
    {
        rectTransform.DOAnchorPosX(-900, 1);
        //Contexts.sharedInstance.game.ReplacePickUI(false, null);
    }

    public override void Refresh()
    {
        ShowBagItem();
        Resources.UnloadUnusedAssets();
    }

    private void ShowBagItem()
    {
        //var playerEntity=Contexts.sharedInstance.game.playerTransformEntity;
        //if (!playerEntity.hasBagItem) return;
        //var bagItems = playerEntity.bagItem.bagItems;
        //var enumerator = bagItems.GetEnumerator();
        //int count = 0;
        //try
        //{
        //    while (enumerator.MoveNext() && count <= 24)
        //    {

        //        var cur = enumerator.Current;

        //        var bagitem = bagItem_list[count];
        //        NotificationCenter<KeyValuePair<int, int>>.Get().DispatchEvent("StoreItem" + bagitem.GetHashCode(), cur);
        //        count++;
        //    }
        //}
        //finally
        //{
        //    enumerator.Dispose();
        //}
       
        //for (; count<24;count++)
        //{
        //    var bagitem = bagItem_list[count];
        //    NotificationCenter<KeyValuePair<int, int>>.Get().DispatchEvent("StoreItem" + bagitem.GetHashCode(), new KeyValuePair<int, int>(0,0));
        //}

        
    }


}
