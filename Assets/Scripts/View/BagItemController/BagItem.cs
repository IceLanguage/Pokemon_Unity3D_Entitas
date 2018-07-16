using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyUnityEventDispatcher;
using System.Collections.Generic;
using TinyTeam.UI;

public class BagItem : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IDragHandler,
    IEndDragHandler
{
    private ItemUI itemUI;
    private RectTransform rectTransform;
    private Vector2 offset = Vector2.zero;
    public ItemUI ItemUI
    {
        get
        {
            if(null == itemUI )
            {
                ItemUI = transform.Find("Item(Clone)").GetComponent<ItemUI>();
            }
            
            return itemUI;
        }

        set
        {
            itemUI = value;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            if(null == rectTransform )
            {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }

        set
        {
            rectTransform = value;
        }
    }

    private void Awake()
    {
        NotificationCenter<KeyValuePair<int, int>>.Get().AddEventListener("StoreItem" + this.GetHashCode(), StoreItem);
    }
    private void StoreItem(Notification<KeyValuePair<int, int>> notific)
    {
        KeyValuePair<int, int> param = notific.param;
        if(0 == notific.param.Key|0 == notific.param.Value)
        {
            if(ItemUI!=null)
            {
                ItemUI.Hide();
            }
            return;
        }
        var newitem = ResourceController.Instance.allItemDic[param.Key];
        StoreItem(newitem,param.Value);
    }
    public void StoreItem(Item item,int count)
    {
        
        
        
        ItemUI.SetItem(item, count);//更新ItemUI
    }

    //根据索引器得到当前物品槽中的物品类型
    public ItemType GetItemType()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }

    //根据索引器得到当前物品槽中的物品ID
    public int GetItemID()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }

    
    //捡起UI
    public void OnPointerDown(PointerEventData eventData)
    {

        //if (Contexts.sharedInstance.game.pickUI.isPick)
        //{
        //    //正在拖拽
        //    ItemUI PickUI = Contexts.sharedInstance.game.pickUI.clickUI;
        //    PickUI.Exchange(itemUI);
        //    Contexts.sharedInstance.game.ReplacePickUI(false, null);
        //}
        //else
        //{
        //    if (null != itemUI.Item)
        //    {
        //        Contexts.sharedInstance.game.ReplacePickUI(true, itemUI);
                
        //    }
               

        //}

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.transform.childCount > 0)
        {
            if (null == ItemUI.Item ) return;
            string toolTipText = ItemUI.Item.GetToolTipText();
            //Contexts.sharedInstance.game.ReplaceToolTipString
            //    (toolTipText, RectTransform.anchoredPosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.childCount > 0) //自身不是空
        {
            //TTUIPage.ClosePage<UIToolTip>();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //SetDraggedPosition(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //SetDraggedPosition(eventData);
    }
    //private void SetDraggedPosition(PointerEventData eventData)
    //{
    //    var rt = gameObject.GetComponent<RectTransform>();

    //    // transform the screen point to world point int rectangle
    //    Vector3 globalMousePos;
    //    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
    //    {
    //        rt.position = globalMousePos;
    //    }
    //}
}
