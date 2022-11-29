using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBoxHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshProUGUI itemname;
    public TMPro.TextMeshProUGUI itembuy;
    public Image itemImage;
    public Item item;   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (item)
        {
            if(itemImage.sprite == null)
            {
                itemImage.sprite = item.itemImage;
                itemname.text = item.itemName;
                itembuy.text = item.buyPrice.ToString();
            }
        }
    }
}
