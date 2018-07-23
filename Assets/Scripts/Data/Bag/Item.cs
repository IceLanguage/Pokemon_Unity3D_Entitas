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
/// 背包物品
/// </summary>
[SerializeField]
public class BagItems : ScriptableObject
{
    [SerializeField]
    public string ItemName;
    [SerializeField]
    public int count;
    public static BagItems Build(string ItemName,int count)
    {
        BagItems items = ScriptableObject.CreateInstance<BagItems>();
        items.count = count;
        items.ItemName = ItemName;
        return items;
    }
    public static BagItems NullItems = Build("", 0);

}
/// <summary>
/// 物品基类
/// </summary>
#if UNITY_EDITOR
[CreateAssetMenu(fileName = "BagItem", menuName = "ScriptableObjec/BagItem")]
#endif
[SerializeField]
public class Item :ScriptableObject
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public ItemType Type;
    [SerializeField]
    public string Description;
    [SerializeField]
    public Sprite sprite;//用于后期查找UI精灵路径

    public Item Clone()
    {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.Description = Description;
        item.Name = name;
        item.Type = Type;
        item.sprite = sprite;
        return item;
    }
    public override int GetHashCode()
    {
        return Description.GetHashCode() + Name.GetHashCode() + Type.GetHashCode() + sprite.GetHashCode();
    }
    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (this.GetType() != other.GetType()) return false;
        return Equals((Item)other);

    }
    public bool Equals(Item obj)
    {
        if (obj == null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (this.GetType() != obj.GetType()) return false;
        if (!base.Equals(obj)) return false;
        return (Description.Equals(obj.Description) &&
            (Name.Equals(obj.Name))) &&
            (Type.Equals(obj.Type))&&
            (sprite == obj.sprite);

    }
    public override string ToString()
    {
        string type = "";
        switch (Type)
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
        string text = string.Format(
            "<color=magenta >{0}</color>\n" +
            "<color=yellow><size=10>介绍：{1}</size></color>\n" +
            "<color=green><size=12>物品类型：{2}</size></color>\n",
            Name,
            Description,
            type);
        return text;
    }

}
