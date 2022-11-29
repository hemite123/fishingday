using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBoxHandler : MonoBehaviour,IPointerClickHandler
{
    public ItemContainer item;
    inventory invent;
    public Image itemImage;
    public TMPro.TextMeshProUGUI Counting;
    Sprite sprite;
    public bool isOpen;

    private void Start()
    {
        invent = inventory.instance.Value;
    }


    private void Update()
    {
        if (Counting.isActiveAndEnabled && item != null)
        {
            string itemtotal = item.itemHave.ToString();
            if(Counting.text != itemtotal)
            {
                Counting.text = itemtotal;
            }
           
        }
        if(item == null)
        {
            if (Counting.isActiveAndEnabled)
            {
                Counting.enabled = false;
            }
            if(sprite != null)
            {
                sprite = null;
                itemImage.sprite = null;
            }
        }
        else
        {
            if(sprite == null)
            {
                sprite = item.item.itemImage;
                itemImage.sprite = sprite;
            }
            if (item.item.Stackable && !Counting.isActiveAndEnabled)
            {
                Counting.enabled = true;
            }
        }
    }

    
    

  
    public void OnClickBox()
    {
        if(item != null)
        {
            if (!isOpen)
            {
                invent.boxSelected = this;
                if (invent.lastSelected != null)
                {
                    invent.lastSelected.isOpen = false;
                }
                StartCoroutine(invent.OpenPanel());
                invent.lastSelected = this;
                invent.SettingupMenu();
                invent.isOpenActionButton = true;
                isOpen = true;
            }
            else
            {
                invent.isOpenActionButton = false;
                StartCoroutine(invent.ClosePanel());
                isOpen = false;
            }
        }
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickBox();
    }
}
