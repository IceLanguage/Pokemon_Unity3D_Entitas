using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using TinyTeam.UI;

public class ItemUI : MonoBehaviour
{
    private static Sprite defaultSprite;
    public BagItems Items { get; private set; }
    public Item Item { get; private set; } //UI上的物品
    //public int Amount { get; private set; }//物品数量

    private Image itemImage;//获取item的Image组件
    private Text amountText;//获取item下子物体用于显示数量的组件

    private readonly float targetScale = 1f;//目标缩放大小
    private readonly Vector3 animationScale = new Vector3(1.4f, 1.4f, 1.4f);
    private readonly float smothing = 4f;//动画平滑过渡时间
    private RectTransform rectTransform;
    
    private RectTransform parentRectTransform;
    public RectTransform ParentRectTransform
    {
        get
        {
            if(null == parentRectTransform)
            {
                parentRectTransform = transform.parent.GetComponent<RectTransform>();
            }
            return parentRectTransform;
        }
    }
    void Awake() //可用 属性get方式替代
    {
        rectTransform = GetComponent<RectTransform>();
        
        itemImage = this.GetComponent<Image>();
        amountText = this.GetComponentInChildren<Text>();
        if (null == defaultSprite )
        {
            defaultSprite =itemImage.sprite;
        }
    }

    void Update()
    {
        //if (this.transform.localScale.x != targetScale)//设置物品动画
        //{
        //    float scaleTemp = Mathf.Lerp(this.transform.localScale.x, targetScale, smothing*Time.deltaTime);
        //    this.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
        //    if (Mathf.Abs(transform.localScale.x-targetScale) < 0.02f)//插值运算达不到临界值，比较耗性能，加上临界值判断能更好的控制
        //    {
        //        this.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        //    }
        //}
    }

    /// <summary>
    ///更新item的UI显示，默认数量为1个
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(BagItems items)
    {
        rectTransform.anchoredPosition = new Vector2(30, -30); ;
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        this.Items = items;
        this.Item = ResourceController.Instance.ItemDic[items.ItemName];
        if (null != Item && null != Item.sprite)
            this.itemImage.sprite = Item.sprite;        //更新UI
        else
            this.itemImage.sprite = defaultSprite;
        if (Items.count > 1)
        {
            this.amountText.text = Items.count.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }
    ///// <summary>
    ///// 添加item数量
    ///// </summary>
    ///// <param name="num"></param>
    //public void AddItemAmount(int num = 1)
    //{
    //    this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
    //    this.Amount += num;
    //    if (this.Amount> 1)//更新UI
    //    {
    //        this.amountText.text = Amount.ToString();
    //        this.itemImage.sprite = Resources.Load<Sprite>(Item.Sprite);        //更新UI
    //    }        
    //    else
    //    {
    //        this.amountText.text = "";
    //    }
    //}
    //设置item的个数
    public void SetAmount(int amount) {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        Items.count = amount;
        if (Items.count > 1)//更新UI
        {
            this.amountText.text = Items.count.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }

    //减少物品数量
    public void RemoveItemAmount(int amount = 1) 
    {
        this.transform.localScale = this.animationScale;//物品更新时放大UI，用于动画
        Items.count -= amount;
        if (Items.count > 1)//更新UI
        {
            this.amountText.text = Items.count.ToString();
        }
        else
        {
            this.amountText.text = "";
        }
    }

    ////显示方法
    //public void Show() {
    //    gameObject.SetActive(true);
    //}

    //隐藏方法
    public void Hide() {
        itemImage.sprite = defaultSprite;
        this.amountText.text = "";
    }

    //设置位置方法
    public void SetLocalPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    //当前物品（UI）和 出入物品（UI）交换显示
    public void Exchange(ItemUI itemUI)
    {
        itemUI.SetItem(Items);
        this.SetItem(itemUI.Items);
    }

    ///// <summary>
    ///// 使用物品
    ///// </summary>
    //public void UseItem()
    //{
        
    //    if(Item.Type!=ItemType.CarryingProps)
    //    {
    //        //UnityAction action;
    //        //ResourceController.Instance.BagItemUseEffectDic.TryGetValue(Item.ID,out action);
    //        //if (null == action) return;
    //        //var playerEntity = Contexts.sharedInstance.game.playerTransformEntity;
    //        //var bagItems = playerEntity.bagItem.bagItems;

    //        //var dic = bagItems;
    //        //dic[Item.ID]--;

    //        //var e = Contexts.sharedInstance.game.CreateEntity();
    //        //e.AddUseItemEvent(dic, action);
    //        //e.AddName(Item.Name);

    //        //var playPokemon = playerEntity.onBattlingPokemon.entity;

    //        //Contexts.sharedInstance.game.CreateEntity().AddBattleCommand(playPokemon, e, true);



    //    }
    //    else
    //    {
    //        Debug.Log("携带物品无法在对战中使用");
    //    }
    //}

    
}
