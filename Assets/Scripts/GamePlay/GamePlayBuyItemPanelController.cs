using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using UnityEngine.Events;
public class GamePlayBuyItemPanelController : UIView
{
    private int itemId;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject endgameRubyPanelPrefab;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private TMPro.TextMeshProUGUI rubyText, itemName, title, costText;
    public UnityAction closeCallBack;
    private float timeStamp;
    private bool isTouch;
    void Start()
    {
        GetComponent<Animator>().Play("Appear");
        DataController.Instance.onDataChange.AddListener(UpdateRubyText);

    }
    public GamePlayBuyItemPanelController Spawn(int itemId, Transform parent)
    {
        var go = Instantiate(gameObject, parent).GetComponent<GamePlayBuyItemPanelController>();
        go.Init(itemId);
        return go;
    }
    public GamePlayBuyItemPanelController Spawn(int itemId, Transform parent, UnityAction callBack)
    {
        var go = Instantiate(gameObject, parent).GetComponent<GamePlayBuyItemPanelController>();
        go.Init(itemId);
        go.closeCallBack = callBack;
        return go;
    }
    public void Init(int itemId)
    {
        UIController.Instance.PushUitoStack(this);
        AudioController.Instance.PlaySfx(popUpClip);
        this.itemId = itemId;
        isTouch = false;
        rubyText.text = DataController.Instance.Ruby.ToString();
        itemName.text = DataController.Instance.GetItemName(itemId);
        int itemCost = DataController.Instance.GetItemDataById(itemId).unitPrice;

        switch (itemId)
        {
            case 100000:
                itemIcon.sprite = itemSprites[0];
                costText.text = itemCost.ToString();
                break;
            case 110000:
                itemIcon.sprite = itemSprites[1];
                costText.text = itemCost.ToString();
                break;
            case (int)ItemType.Instant_Cook:
                itemIcon.sprite = itemSprites[2];
                costText.text = itemCost.ToString();
                break;
            case (int)ItemType.Pudding:
                itemIcon.sprite = itemSprites[3];
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("not_enough_boost");
                costText.text = DataController.Instance.GetItemCost(itemId).ToString();
                break;
            case (int)ItemType.AutoServe:
                itemIcon.sprite = itemSprites[4];
                costText.text = itemCost.ToString();
                break;
            case (int)ItemType.Add_Customers:
                itemIcon.sprite = itemSprites[5];
                costText.text = itemCost.ToString();
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add3cus");
                break;
            case (int)ItemType.Add_Seconds:
                itemIcon.sprite = itemSprites[6];
                costText.text = itemCost.ToString();
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add20sec");
                break;
            case (int)ItemType.Second_Chance:
                itemIcon.sprite = itemSprites[7];
                costText.text = itemCost.ToString();
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                    title.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_burn");
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 2)
                    title.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_threw");
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 3)
                    title.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_cusleft");
                break;
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSecondsRealtime(0.2f);
        if (closeCallBack != null) closeCallBack.Invoke();
        Destroy(gameObject);
    }
    public void OnPlayerDontUseItem()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
            gameController.OnPlayerCancelItemPanel();
    }
    public void OnClickBuy()
    {
        int itemCost = DataController.Instance.GetItemDataById(itemId).unitPrice;
        if (DataController.Instance.Ruby >= itemCost)
        {
            DataController.Instance.Ruby -= itemCost;
            DataController.Instance.AddItem(itemId, 1);
            DataController.Instance.SaveData(false);
            APIController.Instance.LogEventSpentRuby(itemCost, "buy_item");
            OnHide();
        }
        else
        {
            Instantiate(endgameRubyPanelPrefab, transform.parent);
        }
    }
    public void OnClickBuyRuby()
    {
        Instantiate(endgameRubyPanelPrefab, transform.parent);
    }
    public void OnClickBuyAndUse()
    {
        if (!isTouch)
        {
            isTouch = true;
            GameController gameController = FindObjectOfType<GameController>();
            int itemCost = DataController.Instance.GetItemDataById(itemId).unitPrice;
            int itemQuantity = 1;
            if (itemId == (int)ItemType.Pudding)
            {
                itemCost = DataController.Instance.GetItemCost(itemId);
                itemQuantity = 3;
            }

            if (DataController.Instance.Ruby >= itemCost)
            {
                DataController.Instance.Ruby -= itemCost;
                DataController.Instance.AddItem(itemId, itemQuantity);
                switch (itemId)
                {
                    case 100100:
                        DataController.Instance.UseItem(itemId, itemQuantity);
                        gameController.OnClickAddCustomer(3);
                        break;
                    case 100200:
                        DataController.Instance.UseItem(itemId, itemQuantity);
                        gameController.OnClickAddSeconds(20);
                        break;
                    case 100300:
                        DataController.Instance.UseItem(itemId, itemQuantity);
                        gameController.OnClickSecondChance();
                        break;
                }
                DataController.Instance.SaveData(false);
                APIController.Instance.LogEventSpentRuby(itemCost, "buy_item");
                OnHide();
            }
            else
            {
                Instantiate(endgameRubyPanelPrefab, transform.parent);
            }
            isTouch = false;
        }
    }
    void OnDestroy()
    {
        DataController.Instance.onDataChange.RemoveListener(UpdateRubyText);
    }
    public void UpdateRubyText()
    {
        rubyText.text = DataController.Instance.Ruby.ToString();
    }
}
