using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CollisionManagement : MonoBehaviour,IPointerDownHandler
{
    public Item item;
    public string panelType;
    public string collisionType;
    inventory inventorys;
    Gamemanage gamemanager;

    private void Start()
    {
        inventorys = inventory.instance.Value;
        gamemanager = Gamemanage.instance.Value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (item != null)
        {
            inventorys.itemTrigger = this.gameObject;
        } else if (collisionType == "fishTrigger")
        {
            if (collision.tag == "fishindi")
            {
                string taggameobject = this.tag;
                if (taggameobject == "point")
                {
                    gamemanager.fishTrigger = true;
                } 
                if (taggameobject == "lastpointleft")
                {
                    gamemanager.left = false;
                    gamemanager.right = true;
                } else if (tag == "lastpointright")
                {
                    gamemanager.left = true;
                    gamemanager.right = false;
                }
            }
        } else if (collision != null)
        {
            gamemanager.collisionDetect = this;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collisionType == "fishTrigger")
        {
            if (collision.tag == "fishindi")
            {
                string taggameobject = this.tag;
                if (taggameobject == "point")
                {
                    gamemanager.fishTrigger = true;
                   
                }
                if (taggameobject == "lastpointleft")
                {
                    gamemanager.left = false;
                    gamemanager.right = true;
                }
                else if (tag == "lastpointright")
                {
                    gamemanager.left = true;
                    gamemanager.right = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collisionType == "fish")
        {
            gamemanager.collisionDetect = null;
        }
        if (collisionType == "fishTrigger")
        {
            if (collision.tag == "fishindi")
            {
                gamemanager.fishTrigger = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(this.tag == "fishing")
        {
            if(collision.collider.tag == "chara")
            {
                gamemanager.TouchingPond = true;
            }
           
        }else if(collisionType == "shop")
        {
            gamemanager.openShop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (this.tag == "fishing")
        {
            if (collision.collider.tag == "chara")
            {
                gamemanager.TouchingPond = false;
                gamemanager.interactText.gameObject.SetActive(false);
            }

        }
        else if (collisionType == "shop")
        {
            gamemanager.openShop = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(panelType == "notif")
        {
            StartCoroutine(inventorys.CloseNotif());
        }
        else if(panelType == "shop")
        {
            this.GetComponent<Animator>().SetBool("open", false);
            gamemanager.itemeneeded = 1;
            gamemanager.openpopupbns = false;
        }
    }
}
