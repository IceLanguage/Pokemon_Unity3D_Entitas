using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyUnityEventDispatcher;
using System.Collections.Generic;
using TinyTeam.UI;

public class BagItemUI : MonoBehaviour,
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
            if (null == itemUI)
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
            if (null == rectTransform)
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
        NotificationCenter<BagItems>.Get().AddEventListener("StoreItem" + this.GetHashCode(), StoreItem);
    }
    private void StoreItem(Notification<BagItems> notific)
    {
        BagItems param = notific.param;
        if (null == param || 0 == param.ItemName.Length|| 0 == param.count)
        {
            if (ItemUI != null)
            {
                ItemUI.Hide();
            }
            return;
        }
        StoreItem(param);
    }
    private void StoreItem(BagItems bagItems)
    {
        ItemUI.SetItem(bagItems);//更新ItemUI
    }

    //根据索引器得到当前物品槽中的物品类型
    public ItemType GetItemType()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }
    
    //捡起UI
    public void OnPointerDown(PointerEventData eventData)
    {

        if (UIKnapscakPanel.isPick)
        {
            //正在拖拽
            ItemUI PickUI = UIKnapscakPanel.pickUI;
            PickUI.Exchange(itemUI);
            UIKnapscakPanel.isPick = false;
            UIKnapscakPanel.pickUI = null;
        }
        else
        {
            if (null != itemUI.Item)
            {
                UIKnapscakPanel.isPick = true;
                UIKnapscakPanel.pickUI = itemUI;

            }


        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.transform.childCount > 0)
        {
            if (null == ItemUI.Item ) return;
            string toolTipText = ItemUI.Item.ToString();
            //Contexts.sharedInstance.game.ReplaceToolTipString
            //    (toolTipText, RectTransform.anchoredPosition);
            NotificationCenter<ToolTipMessage>.Get()
                .DispatchEvent("UIToolTip"
                ,new ToolTipMessage(toolTipText, RectTransform.anchoredPosition));
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
        SetDraggedPosition(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    private void SetDraggedPosition(PointerEventData eventData)
    {
        var rt = gameObject.GetComponent<RectTransform>();

        // transform the screen point to world point int rectangle
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
        }
    }
}
