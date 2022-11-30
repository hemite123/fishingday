using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemanage : MonoBehaviour
{
    public GameObject camera;
    public int InventorySlot;
    public static Lazy<Gamemanage> instance;
    public CollisionManagement collisionDetect;
    public GameObject mark;
    public bool TouchingPond,onFishing,onBite,waitingTime,gambler,indicatormove,left,right,fishTrigger,itemGetTrigger,openShop,sell,openpopupbns,panelselectedbns,openpanelselectbns,triggered,fishcombo;
    public GameObject fishIndicator,panelFishPower;
    public List<DroppingPlace> dp = new List<DroppingPlace>();
    public List<GameObject> imageBar = new List<GameObject>();
    public float fishPower,timer,holdingbutton,timeradded;
    int combo = 0,attention = 0,currency = 0;
    CollisionManagement indicatorTrigger;
    public Sprite markimage;
    public Item itemget,itembuyselect;
    public inventory invent;
    public GameObject prefabShop,ContentShop,shopUi,shopBuyandSellPanel,panelselect,panelCurrency;
    ItemDatabase itemdb;
    public List<GameObject> shopbutton = new List<GameObject>();
    public List<Image> shopSelected = new List<Image>();
    public int selectshop = 0,itemeneeded = 1,openselectbns = 0;
    public TMPro.TextMeshProUGUI dynamicItemNeeded,dynamicCalc,notificaiton,goldText,interactText;
    public Image buyandsellitemimage, upbands, downbands,triggerfish;
    public AudioSource adsource;
    public Slider slider;
    public GameObject settingpanel,tutorial;

    private void Awake()
    {
        instance = new Lazy<Gamemanage>(() => this, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        itemdb = ItemDatabase.Instance.Value;
        indicatorTrigger = fishIndicator.GetComponent<CollisionManagement>();
        adsource = this.GetComponent<AudioSource>();
        invent = inventory.instance.Value;
        foreach(Item it in itemdb.itemDb)
        {
            GameObject go = Instantiate(prefabShop, ContentShop.transform);
            ShopBoxHandler sbh = go.GetComponent<ShopBoxHandler>();
            sbh.item = it;
            if(it.buyPrice > 0)
            {
                sbh.itembuy.text = it.buyPrice.ToString();
            }
            else
            {
                sbh.itembuy.gameObject.SetActive(false);
            }
            sbh.itemname.text = it.itemName.ToString();
            sbh.itemImage.sprite = it.itemImage;
            shopbutton.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = currency.ToString();
        adsource.volume = slider.value;
        if (holdingbutton > 2f)
        {
            timeradded = 0.1f;
        }
        else
        {
            timeradded = 0.3f;
        }
        if(onFishing && !onBite && !gambler)
        {
            interactText.text = "Press F For Pull";
            interactText.gameObject.SetActive(true);
            gambler = true;
            StartCoroutine(ChanceBite());
        }
        if (openpopupbns)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ProcessTransaction();
            }else if (Input.GetKey(KeyCode.UpArrow))
            {
                holdingbutton += Time.deltaTime;
                if (itemeneeded < 99)
                {
                    timer += Time.deltaTime;
                    if (timer > timeradded)
                    {
                        itemeneeded += 1;
                        int calc = 0;
                        if (sell)
                        {
                            calc += itembuyselect.sellPrice * itemeneeded;
                        }
                        else
                        {
                            calc += itembuyselect.buyPrice * itemeneeded;
                        }
                        dynamicCalc.text = calc.ToString();
                        StartCoroutine(animatedupdown(false));
                        timer = 0f;
                    }
                }
            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                holdingbutton += Time.deltaTime;
                if(itemeneeded > 1)
                {
                    timer += Time.deltaTime;
                    if(timer > timeradded)
                    {
                        int calc = 0;
                        if (sell)
                        {
                            calc += itembuyselect.sellPrice * itemeneeded;
                        }
                        else
                        {
                            calc += itembuyselect.buyPrice * itemeneeded;
                        }
                        dynamicCalc.text = calc.ToString();
                        itemeneeded -= 1;
                        StartCoroutine(animatedupdown(true));
                        timer = 0f;
                    }
                    
                }
            }else if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                holdingbutton = 0f;
            }
            dynamicItemNeeded.text = itemeneeded.ToString();

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (onBite && !triggered)
            {
                triggered = true;
                for(int i = 0; i < dp.Count; i++)
                {
                    if(dp[i].collider == collisionDetect.gameObject)
                    {
                        dp[i].blubactive.SetActive(false);
                        break;
                    }
                }
                interactText.gameObject.SetActive(false);
                StopCoroutine(waitForOutBite());
                StartCoroutine(markandshowfish());

            }
            else if (collisionDetect != null)
            {
                if(collisionDetect.collisionType == "fish" && TouchingPond && !onFishing)
                {
                    onFishing = true;
                    
                }
                else
                {
                    onFishing = false;
                    StopCoroutine(ChanceBite());
                }
            }
            
        }

        if (onBite && !waitingTime)
        {
            waitingTime = true;
            StartCoroutine(waitForOutBite());
        }

        if (indicatormove)
        {
            if (Input.GetKeyDown(KeyCode.Space) )
            {
                if (fishTrigger && !fishcombo)
                {
                    fishcombo = true;
                    triggerfish.color = new Color(247, 255, 0, 255);
                    StartCoroutine(waitforactiveagain());
                    AddSomeBlock();
                }
                else
                {
                    if(combo > 0)
                    {
                        combo -= 1;
                        imageBar[combo].SetActive(false);
                        return;
                    }
                    attention += 1;  
                }
            }
            if (combo == 4 && !itemGetTrigger)
            {
                itemGetTrigger = true;
                StartCoroutine(getItem());

            }else if(attention == 3)
            {
                ResetFishSystem();
            }
            if (attention == 0)
            {
                fishIndicator.GetComponent<Image>().color = Color.green;
            }
            else if (attention == 1)
            {
                fishIndicator.GetComponent<Image>().color = Color.white;
            }
            else if (attention == 2)
            {
                fishIndicator.GetComponent<Image>().color = Color.red;
            }
        }

        else if (openShop && !openpopupbns && !panelselectedbns)
        {
            if (!panelCurrency.activeSelf)
            {
                panelCurrency.SetActive(true);
            }
            for(int i = 0; i < shopbutton.Count; i++)
            {
                if(i == selectshop)
                {
                    shopbutton[i].GetComponent<Image>().color = new Color(255, 255, 255, 255f);
                }
                else
                {
                    shopbutton[i].GetComponent<Image>().color = new Color(255, 255, 255, 0f);
                }
            }
            if (!shopUi.activeSelf)
            {
                shopUi.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                itembuyselect = shopbutton[selectshop].GetComponent<ShopBoxHandler>().item;
                StartCoroutine(panelselectanim());
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectshop > 0)
                {
                    selectshop -= 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectshop < shopbutton.Count - 1)
                {
                    selectshop += 1;
                }
            }
        }

        else if (panelselectedbns)
        {
            for (int i = 0; i < shopSelected.Count; i++)
            {
                if (i == openselectbns)
                {
                    shopSelected[i].GetComponent<Image>().color = new Color(255, 255, 255, 255f);
                }
                else
                {
                    shopSelected[i].GetComponent<Image>().color = new Color(255, 255, 255, 0f);
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (openselectbns > 0)
                {
                    openselectbns -= 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (openselectbns < shopSelected.Count - 1)
                {
                    openselectbns += 1;
                }
            }else if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (openselectbns)
                {
                    case 0:
                        popupshowitem(false);
                        ClosePanel();
                        break;
                    case 1:
                        popupshowitem(true);
                        ClosePanel();
                        break;
                    case 2:
                        ClosePanel();
                        break;
                    default:
                        break;
                }
                openselectbns = 0;
            }
        }

        if (!openShop)
        {
            if (shopUi.activeSelf)
            {
                shopUi.SetActive(false);
                sell = false;
                openpanelselectbns = false;
                panelselectedbns = false;
                openpopupbns = false;
                itemeneeded = 1;
            }
            if (panelCurrency.activeSelf)
            {
                panelCurrency.SetActive(false);
            }
        }

        if (TouchingPond && !onFishing)
        {
            interactText.text = "Press F To Fish";
            interactText.gameObject.SetActive(true);
        }
        
    }

    private void ProcessTransaction()
    {
        if (sell)
        {
            ItemContainer cursel = null;
            for(int i =  0; i < invent.contentGO.Count; i++)
            {
                ItemBoxHandler ic = invent.contentGO[i].GetComponent<ItemBoxHandler>();
                if(ic.item != null)
                {
                    Item ininven = ic.item.item;
                    if (ininven == itembuyselect)
                    {
                        cursel = ic.item;
                        break;
                    }
                }
                
            }
            if(cursel != null)
            {
                if(cursel.itemHave >= itemeneeded)
                {
                    cursel.itemHave -= itemeneeded;
                    if(cursel.itemHave <= 0)
                    {
                        invent.removeItem(cursel.item);
                    }
                    currency += cursel.item.sellPrice * itemeneeded;
                    shopBuyandSellPanel.GetComponent<Animator>().SetBool("open", false);
                    notificaiton.gameObject.SetActive(true);
                    notificaiton.text = "Item Sell";
                    openpopupbns = false;
                    sell = false;
                    StartCoroutine(closeNotif());
                }
                else
                {
                    notificaiton.gameObject.SetActive(true);
                    notificaiton.text = "Not Enought Item To Sell";
                    StartCoroutine(closeNotif());
                }
            }
            else
            {
                notificaiton.gameObject.SetActive(true);
                notificaiton.text = "Not Enought Item To Sell";
                StartCoroutine(closeNotif());
            }
        }
        else
        {
            int itemcalculation = itembuyselect.buyPrice * itemeneeded;
            if(currency >= itemcalculation)
            {
                currency -= itemcalculation;
                invent.AddItem(itembuyselect, itemeneeded);
                shopBuyandSellPanel.GetComponent<Animator>().SetBool("open", false);
            }
            else
            {
                notificaiton.gameObject.SetActive(true);
                notificaiton.text = "Not Enought Gold";
                StartCoroutine(closeNotif());
            }
        }
    }

    IEnumerator waitforactiveagain()
    {
        yield return new WaitForSeconds(2f);
        fishcombo = false;
        triggerfish.color = new Color(255, 0, 0, 255);
    }

    public void ResetFishSystem()
    {
        RectTransform rt = fishIndicator.GetComponent<RectTransform>();
        indicatormove = false;
        fishIndicator.GetComponent<Image>().color = Color.green;
        onBite = false;
        waitingTime = false;
        left = false;
        right = false;
        gambler = false;
        fishIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);
        onFishing = false;
        panelFishPower.SetActive(false);
        attention = 0;
        combo = 0;
        itemGetTrigger = false;
        triggered = false;
        foreach(GameObject image in imageBar)
        {
            image.SetActive(false);
        }
    }

    public void AddSomeBlock()
    {
        float random = UnityEngine.Random.Range(0f, 5f);
        if (attention > 0)
        {
            attention = 0;
        }
        if (random > 2.0f)
        {
            imageBar[combo].SetActive(true);
            combo += 1;
        }
    }


    IEnumerator animatedupdown(bool down)
    {
        if (down)
        {
            downbands.color = new Color(255, 255, 255, 255);
            yield return new WaitForSeconds(2f);
            downbands.color = new Color(255, 255, 255, 150);
        }
        else
        {
            upbands.color = new Color(255, 255, 255, 255);
            yield return new WaitForSeconds(2f);
            upbands.color = new Color(255, 255, 255, 150);
        }
    }

    IEnumerator getItem()
    {
        ResetFishSystem();
        mark.SetActive(true);
        mark.GetComponent<SpriteRenderer>().sprite = itemget.itemImage;
        yield return new WaitForSeconds(2f);
        mark.SetActive(false);
        mark.GetComponent<SpriteRenderer>().sprite = markimage;
        invent.AddItem(itemget,1);
        itemget = null;
        
    }


    private void LateUpdate()
    {

        if (indicatormove)
        {
            if (left)
            {
                fishIndicator.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(-fishPower, 0f);
               
            }
            else if (right)
            {
                fishIndicator.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(fishPower, 0f);
            }

        }
    }

    IEnumerator ChanceBite()
    {
        float chance = UnityEngine.Random.RandomRange(0f, 1f);
        if(chance > 0.5f)
        {
            onBite = true;
            for (int i = 0; i < dp.Count; i++)
            {
                if (dp[i].collider == collisionDetect.gameObject)
                {
                    dp[i].blubactive.SetActive(true);
                    break;
                }
            }
            Debug.Log("Bait Bite");
            gambler = false;
            StopCoroutine(ChanceBite());
        }
        else
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(ChanceBite());
        }
    }

    IEnumerator markandshowfish()
    {
        mark.SetActive(true);
        waitingTime = false;
        DroppingPlace dropPlace = null;
        for(int i = 0; i < dp.Count; i++)
        {
            if(dp[i].collider == collisionDetect.gameObject)
            {
                dropPlace = dp[i];
                break;
            }
        }
        float proba = 0f;
        foreach (DropChance dc in dropPlace.dropChance)
        {
            proba += dc.drop;
        }
        float chancegets = UnityEngine.Random.Range(0f,proba);
        Debug.Log(chancegets);
        float top = 0f;
        for (int i = 0; i < dropPlace.dropChance.Count; i++)
        {
            top += dropPlace.dropChance[i].drop;    
            if (chancegets <= top)
            {
                itemget = dropPlace.dropChance[i].item;
                fishPower = dropPlace.dropChance[i].itemPower;
                break;
            }
        }
        fishIndicator.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(2f);
        float chanceget = UnityEngine.Random.Range(0f, 1f);
        if(chanceget > 0.5f)
        {
            right = true;
        }
        else
        {
            left = true;
        }
        mark.SetActive(false);
        panelFishPower.SetActive(true);
        indicatormove = true;
        yield return null;
    }

    IEnumerator waitForOutBite()
    {
        yield return new WaitForSeconds(4f);
        waitingTime = false;
        if (!indicatormove)
        {
            onBite = false;
        }
        

    }

    public void popupshowitem(bool issell)
    {
        shopBuyandSellPanel.SetActive(true);
        openpopupbns = true;
        buyandsellitemimage.sprite = itembuyselect.itemImage;
        sell = issell;
        int calc = 0;
        if (sell)
        {
            calc += itembuyselect.sellPrice * itemeneeded;
        }
        else
        {
            if(itembuyselect.buyPrice == 0)
            {
                notificaiton.gameObject.SetActive(true);
                notificaiton.text = "Item Not Buyable";
                openpopupbns = false;
                StartCoroutine(closeNotif());
                return;
            }
            calc += itembuyselect.buyPrice * itemeneeded;
        }
        dynamicCalc.text = calc.ToString();
        shopBuyandSellPanel.GetComponent<Animator>().SetBool("open", true);
    }

    IEnumerator panelselectanim()
    { 
        RectTransform rt = panelselect.GetComponent<RectTransform>();
        if (panelselect.activeSelf)
        {
            for(float i = 244.1982f; i < 652f; i += 10f)
            {
                rt.offsetMin = new Vector2(rt.offsetMin.x, i);
                yield return null;
            }
        }
        panelselectedbns = true;
        panelselect.SetActive(true);
        for (float i = 652f; i > 244.1982f; i -= 10f)
        {
            i -= 10f;
            rt.offsetMin = new Vector2(rt.offsetMin.x, i);
            yield return null;
        }
        yield return null;
    }

    IEnumerator closePanel()
    {
        RectTransform rt = panelselect.GetComponent<RectTransform>();
        for (float i = 244.1982f; i < 652f; i += 10f)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, i);
            yield return null;
        }
        panelselect.SetActive(false);
    }

    public void ClosePanel()
    {
        panelselectedbns = false;
        StartCoroutine(closePanel());
    }

    IEnumerator closeNotif()
    {
        yield return new WaitForSeconds(2f);
        notificaiton.gameObject.SetActive(false);
    }

    public void Setting(bool setup)
    {
        settingpanel.SetActive(setup);
    }

    public void Tutorial(bool setup)
    {
        tutorial.SetActive(setup);
    }
   
}

[System.Serializable]
public class DroppingPlace
{
    public GameObject collider;
    public List<DropChance> dropChance;
    public GameObject blubactive;
}

[System.Serializable]
public class DropChance
{
    public Item item;
    public float itemPower;
    public float drop;
}
