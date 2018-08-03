using System.Collections.Generic;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MyUnityEventDispatcher;
using System.Collections;

public class UIKnapscakPanel:TTUIPage
{
    public UIKnapscakPanel() :base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIKnapscakPanel";
    }
    private List<Button> BagSolt_list = new List<Button>();
    private static List<BagItemUI> bagItem_list = new List<BagItemUI>();
    private RectTransform rectTransform;
    private Button CloseButton;

    private static GameContext context;

    public static bool isPick = false;
    public static ItemUI pickUI;

    private static bool ishide = true;
    public override void Awake(GameObject go)
    {
        
        context = Contexts.sharedInstance.game;
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
            var item = button.transform.GetComponent<BagItemUI>();
            
            bagItem_list.Add(item);
        }
    }
    public override void Active()
    {
        ClosePage<UI_PokemonBag>();
        rectTransform.DOAnchorPosX(-240, 1);
    }

    public override void Hide()
    {
        isPick = false;
        pickUI = null;
        ishide = true;
        rectTransform.DOAnchorPosX(-900, 1);
       
    }

    public override void Refresh()
    {
        ishide = false;
        LHCoroutine.CoroutineManager.DoCoroutine(MovePickUI());
        ShowBagItem();
        Resources.UnloadUnusedAssets();
    }

    private static void ShowBagItem()
    {
        List<BagItems> bagItems = context.playerData.scriptableObject.bagItems;
        int count = 0;
        foreach (BagItems items in bagItems)
        {
            var bagitemUI = bagItem_list[count];
            NotificationCenter<BagItems>.Get().DispatchEvent("StoreItem" + bagitemUI.GetHashCode(), items);
            count++;
        }

        for (; count < 24; count++)
        {
            var bagitemUI = bagItem_list[count];
            NotificationCenter<BagItems>.Get().DispatchEvent("StoreItem" + bagitemUI.GetHashCode(), BagItems.NullItems);
        }


    }

    static IEnumerator MovePickUI()
    {
        yield return new WaitForSeconds(0.01f);
        if(!ishide)
        {
            UpdatePickUI();
            LHCoroutine.CoroutineManager.DoCoroutine(MovePickUI());
        }
        
    }

    private static void UpdatePickUI()
    {
        //让物品UI随着鼠标/手指移动
        if (isPick && pickUI != null)
        {

            Vector2 postionPickeItem;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (pickUI.ParentRectTransform,
                Input.mousePosition,
                UIController.Instance.UICamera,
                out postionPickeItem);
            pickUI.SetLocalPosition(postionPickeItem + new Vector2(25, -25));

        }

        //鼠标进入屏幕右侧且处于拾取UI的状态  触发背包物品使用功能
        if (Input.mousePosition.x > Screen.width *0.7f &&
           isPick && pickUI != null)
        {
            //取消拾取UI的状态
            isPick = false;

            //非战斗时的操作
            if (!context.isBattleFlag)
            {
                pickUI.SetLocalPosition(new Vector2(25, -25));
                return;
            }


           
            pickUI.SetLocalPosition(new Vector2(30, -30));
           
            

            

            //使用物品
            UseBagItem(pickUI);

            //关闭背包UI
            TTUIPage.ClosePage<UIKnapscakPanel>();
        }
    }

    private static void UseBagItem(ItemUI itemUI)
    {
        BagItems bagItems = itemUI.Items;
        if (0 >= bagItems.count)
            Debug.LogError("数量为0的道具不该存在");
        bagItems.count -= 1;
        ShowBagItem();
        if (context.isBattleFlag)
        {
            NotificationCenter<string>.Get().DispatchEvent("UseBagItem", bagItems.ItemName);
        }

    }

   

}
