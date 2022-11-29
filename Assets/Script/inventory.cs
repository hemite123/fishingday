using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventory : MonoBehaviour
{
    Gamemanage gamemanage;
    public static Lazy<inventory> instance;
    public GameObject inventoryBox;
    public GameObject Content;
    public List<ItemContainer> itemData = new List<ItemContainer>();
    public List<GameObject> contentGO = new List<GameObject>();
    public bool openInventory;
    public ItemBoxHandler boxSelected;
    public ItemBoxHandler lastSelected;
    public List<GameObject> actionButton;
    public int actionbuttonselected = 0;
    public bool isOpenActionButton,openAction;
    public GameObject menuaction,detailItem,inventoryPanel;
    public TMPro.TextMeshProUGUI itemNameDesc, itemDescDe;
    public Image itemSpriteDesc;
    public Animator panelanim;
    public GameObject itemTrigger;
    int inventorySlot;

    private void Awake()
    {
        instance = new Lazy<inventory>(() => this,true);
        panelanim = menuaction.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gamemanage = Gamemanage.instance.Value;
        inventorySlot = gamemanage.InventorySlot;
        for(int i = 0; i < inventorySlot; i++)
        {
            GameObject go = Instantiate(inventoryBox, Content.transform);
            contentGO.Add(go);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (openInventory)
            {
                openInventory = false;
                inventoryPanel.SetActive(false);
                gamemanage.panelCurrency.SetActive(false);
                StartCoroutine(closeInnventoryPanel());
            }
            else
            {
                openInventory = true;
                gamemanage.panelCurrency.SetActive(true);
                inventoryPanel.SetActive(true);
                StartCoroutine(openInnventoryPanel());
            }
           
        }

        if(itemTrigger != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddItem(itemTrigger.GetComponent<CollisionManagement>().item,1);
                DestroyImmediate(itemTrigger);
                itemTrigger = null;
            }
        }

        if (isOpenActionButton)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) && openAction)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && actionbuttonselected > 0)
                {
                    actionbuttonselected -= 1;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && actionbuttonselected < 1)
                {
                    actionbuttonselected += 1;
                }
                SettingupMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Return) && openAction)
            {
                openAction = false;
                isOpenActionButton = false;
                switch (actionbuttonselected)
                {
                    case 0:
                        StartCoroutine(ClosePanel());
                        boxSelected.isOpen = false;
                        detailItem.SetActive(true);
                        Item item = boxSelected.item.item;
                        itemNameDesc.text = item.itemName;
                        itemDescDe.text = item.itemDescription;
                        itemSpriteDesc.sprite = item.itemImage;
                        detailItem.GetComponent<Animator>().SetBool("open", true);
                        break;
                    case 1:
                        Debug.Log("Droping Item");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void SettingupMenu()
    {
        for (int i = 0; i < actionButton.Count; i++)
        {
            if (i == actionbuttonselected)
            {
                actionButton[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                actionButton[i].GetComponent<Image>().enabled = false;
            }
        }
        openAction = true;
    }

    IEnumerator openInnventoryPanel()
    {
        RectTransform rect = Content.GetComponent<RectTransform>();
        rect.position = new Vector3(rect.position.x,1,rect.position.z);
        for(float i = 0f; i <= 1.1f; i += 0.1f)
        {
            inventoryPanel.transform.localScale = new Vector3(i, i, i);
            yield return null;
        }
        yield return null;
    }

    IEnumerator closeInnventoryPanel()
    {
        for (float i =1f; i >= 0f; i -= 0.1f)
        {
            inventoryPanel.transform.localScale = new Vector3(i, i, i);
            yield return null;
        }
        yield return null;
    }
    public void AddItem(Item items,int manyitem)
    {
        if (items.Stackable)
        {
            int indexofitem = -1;
            if(itemData.Count > 0)
            {
                for (int i = 0; i < itemData.Count; i++)
                {
                    if (itemData[i].item == items)
                    {
                        indexofitem = i;
                        break;
                    }
                }
            }
            
            
            if(indexofitem == -1)
            {
                ItemContainer iC = new ItemContainer() { item = items,itemHave = 1};
                itemData.Add(iC);
                ItemBoxHandler getContent = contentGO[itemData.Count - 1].GetComponent<ItemBoxHandler>();
                getContent.item = iC;
            }
            else
            {
                ItemBoxHandler getContent = contentGO[indexofitem].GetComponent<ItemBoxHandler>();
                getContent.item.itemHave += manyitem;
            }
        }
        else
        {
            ItemContainer iC = new ItemContainer() { item = items, itemHave = 1 };
            itemData.Add(iC);
            ItemBoxHandler getContent = contentGO[itemData.Count - 1].GetComponent<ItemBoxHandler>();
            getContent.item = iC;
        }

    }

    public void removeItem(Item items)
    {
        for(int i = 0; i < itemData.Count; i++)
        {
            ItemContainer itc = itemData[i];
            if (itc.item == items)
            {
                ItemBoxHandler iBH = contentGO[i].GetComponent<ItemBoxHandler>();
                iBH.item = null;
                iBH.itemImage.sprite = null;
                itemData.RemoveAt(i);
                break;
            }
        }
    }

    public IEnumerator OpenPanel()
    {
        menuaction.SetActive(true);
        if (panelanim.GetBool("open"))
        {
            panelanim.SetBool("open", false);
            yield return new WaitForSeconds(panelanim.GetCurrentAnimatorStateInfo(0).length);
        }
        panelanim.SetBool("open", true);
        yield return null;
    }

    public IEnumerator ClosePanel()
    {
        panelanim.SetBool("close", false);       
        yield return new WaitForSeconds(panelanim.GetCurrentAnimatorStateInfo(0).length);
        menuaction.SetActive(false);
    }

    public IEnumerator CloseNotif()
    {
        Animator anim = detailItem.GetComponent<Animator>();
        anim.SetBool("open", false);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        detailItem.SetActive(false);
    }
}
