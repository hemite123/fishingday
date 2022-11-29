using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="Item",menuName ="Item/CreateItem",order =1)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public ItemType itemType;
    [TextArea]
    public string itemDescription;
    public bool Stackable;
    public int sellPrice;
    public int buyPrice;
}

public enum ItemType
{
    Rod,Shoes,Consumable,Misc
}
