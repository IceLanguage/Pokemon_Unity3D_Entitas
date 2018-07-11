using UnityEngine;
using System.Collections;
/// <summary>
/// 物品类型
/// </summary>
public enum ItemType
{
    PokemonBall=0,
    TreeFruit=1,
    CarryingProps=2,
    Consumables=3

}
/// <summary>
/// 物品基类
/// </summary>
public class Item {
    public int ID { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public string Description { get; set; }
    //public int Capacity { get; set; }//容量
    //public int BuyPrice { get; set; }
    //public int SellPrice { get; set; }
    public string Sprite { get; set; }//用于后期查找UI精灵路径

    public Item() 
    {
        this.ID = -1;//表示这是一个空的物品类
    }

    public Item(int id,string name,ItemType type,string description,string sprite)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Description = description;
        //this.BuyPrice = buyPrice;
        //this.SellPrice = sellPrice;
        this.Sprite = sprite;
    }
    public Item(int id, string name, ItemType type, string description)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Description = description;
        //this.BuyPrice = buyPrice;
        //this.SellPrice = sellPrice;
    }
   


    //得到提示框应该显示的内容
    public virtual string GetToolTipText() 
    {
        string type="";
        switch(Type)
        {
            case ItemType.CarryingProps:
                type = "携带物品";
                break;
            case ItemType.Consumables:
                type = "消耗品";
                break;
            case ItemType.PokemonBall:
                type = "精灵球";
                break;
            case ItemType.TreeFruit:
                type = "树果";
                break;
        }
        string text= string.Format(
            "<color=magenta >{0}</color>\n" +
            "<color=yellow><size=10>介绍：{1}</size></color>\n" +
            "<color=green><size=12>物品类型：{2}</size></color>\n",
            Name,
            Description,
            type);
        return text;
    }
}
