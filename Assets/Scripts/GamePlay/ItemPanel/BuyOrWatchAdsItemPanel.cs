using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BuyOrWatchAdsItemPanel : UIView
{
    public string adsName = "";
    private int itemId;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject endgameRubyPanelPrefab;
    [SerializeField] private Image itemIcon, itemIconAds;
    [SerializeField] private Sprite[] itemSprites, itemAdsSprites;
    [SerializeField] private TextMeshProUGUI rubyText, descriptionText, descriptionAdsText, conditionTitleText, itemQuantityText, reasonText;
    [SerializeField] private Button videoRewardBtn, buyItemBtn, useItemBtn;
    private bool isWatchedAds;
    private EndGameItemPanel endGameItemPanel;
    void Start()
    {
        isWatchedAds = false;
        GetComponent<Animator>().Play("Appear");
        DataController.Instance.onDataChange.AddListener(UpdateRubyText);
        endGameItemPanel = FindObjectOfType<EndGameItemPanel>();
        bool canShowAds = AdsController.Instance.CanShowAds(adsName);
        //videoRewardBtn.interactable = canShowAds;
        if (canShowAds)
        {
            APIController.Instance.LogEventShowAds(adsName);
        }
        int itemQuantity = DataController.Instance.GetItemQuantity(itemId);
        itemQuantityText.text = itemQuantity.ToString();
        buyItemBtn.gameObject.SetActive(itemQuantity <= 0);
        useItemBtn.gameObject.SetActive(itemQuantity > 0);
    }
    void OnDestroy()
    {
        DataController.Instance.onDataChange.RemoveListener(UpdateRubyText);
    }
    public void UpdateRubyText()
    {
        rubyText.text = DataController.Instance.Ruby.ToString();
    }
    public void Init(int itemId)
    {
        UIController.Instance.PushUitoStack(this);
        Time.timeScale = 0;
        AudioController.Instance.PlaySfx(popUpClip);
        this.itemId = itemId;
        rubyText.text = DataController.Instance.Ruby.ToString();
        string message = null;
        string messageAds = null;
        switch (itemId)
        {
            case 100100:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add3cus");
                messageAds = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add1cus");
                reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add3cus");
                break;
            case 100200:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add20sec");
                messageAds = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add10sec");
                reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add20sec");
                break;
            case 100300:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_second_chance");
                messageAds = message;
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_burn");
                }
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 2)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_threw");
                }
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 3)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_cusleft");
                }
                break;
        }
        descriptionText.text = message;
        descriptionAdsText.text = messageAds;
        switch (itemId)
        {
            case 100100:
                itemIcon.sprite = itemSprites[0];
                itemIconAds.sprite = itemAdsSprites[0];
                break;
            case 100200:
                itemIcon.sprite = itemSprites[1];
                itemIconAds.sprite = itemAdsSprites[1];
                break;
            case 100300:
                itemIcon.sprite = itemSprites[2];
                itemIconAds.sprite = itemAdsSprites[2];
                break;
        }
    }
    public void OnClickBuyAndUseItem()
    {
        int itemCost = 40;//check if can buy item
        int ruby = DataController.Instance.Ruby;
        if (ruby >= itemCost)
        {
            UIController.Instance.PopUiOutStack();
            APIController.Instance.LogEventSpentRuby(itemCost, "buy_item");
            DataController.Instance.Ruby -= itemCost;
            DataController.Instance.AddItem(itemId, 1);
            UseItem();
        }
        else
        {
            OnClickBuyRuby();
        }
    }
    public void OnCloseAds()
    {
        if (isWatchedAds)
        {
            isWatchedAds = false;
            UseAdsItem();
        }
    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        APIController.Instance.LogEventViewAds(adsName);
        isWatchedAds = true;
        //UseAdsItem();
    }
    public void UseItem()
    {
        endGameItemPanel.hasWatchAds = true;
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        GameController gameController = FindObjectOfType<GameController>();
        DataController.Instance.UseItem(itemId, 1);
        DataController.Instance.SaveData();
        switch (itemId)
        {
            case 100100:
                gameController.OnClickAddCustomer(3);
                break;
            case 100200:
                gameController.OnClickAddSeconds(20);
                break;
            case 100300:
                gameController.OnClickSecondChance();
                break;
        }
        string itemName = DataController.Instance.GetItemName(itemId);
        string para_leveldata = gameController.levelData.chapter + "_" + gameController.levelData.id + "_" + gameController.levelState;
        APIController.Instance.LogEventUseItem(itemName, para_leveldata);
        HidePanel();
    }
    public void UseAdsItem()
    {
        endGameItemPanel.hasWatchAds = true;
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        GameController gameController = FindObjectOfType<GameController>();
        switch (itemId)
        {
            case 100100:
                gameController.OnClickAdd1Customer(1);
                break;
            case 100200:
                gameController.OnClickAddSeconds(10);
                break;
            case 100300:
                gameController.OnClickSecondChance();
                break;
        }
        string itemName = DataController.Instance.GetItemName(itemId);
        string para_leveldata = gameController.levelData.chapter + "_" + gameController.levelData.id + "_" + gameController.levelState;
        APIController.Instance.LogEventUseItem(itemName, para_leveldata);
        HidePanel();
    }
    public void OnClickWatchAds()
    {
        ShowRewardedVideo();
    }
    public void OnClickBuyRuby()
    {
        if (PlayerPrefs.GetString("shop_ingame_abtest", "1") == "1")
            PlayerPrefs.SetString("shop_loaction", "shop_full_ingame_lose_level");
        else
            PlayerPrefs.SetString("shop_loaction", "shop_mini_ingame_lose_level");
        Instantiate(endgameRubyPanelPrefab, transform.parent);
    }
    public void OnClickClose()
    {
        UIController.Instance.PopUiOutStack();
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
            gameController.OnPlayerCancelItemPanel();
        HidePanel();
    }
    private void HidePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
            gameController.IsShowAds = true;
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
    }
    public override void OnHide()
    {
        OnClickClose();
    }
}
