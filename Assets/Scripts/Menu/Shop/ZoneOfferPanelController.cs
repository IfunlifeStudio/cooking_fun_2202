using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ZoneOfferPanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] int rubyAmount;
    [SerializeField] GameObject japanSymbol1, japanSymbol2;
    [SerializeField] Image leftZoneSymbol, rightZoneSymbol;
    [SerializeField] Sprite[] leftZoneIconSprites, rightZoneIconSprites;
    [SerializeField] TextMeshProUGUI packageNameTxt;
    [SerializeField] Items[] items;
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        SetupData();
        animator.Play("Appear");
    }
    public void OnClickBuy()
    {

        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();

        PlayerPrefs.SetInt("has_bought_zone_offer", DataController.Instance.HighestRestaurant);
        if (rubyAmount > 0)
        {
            mainMenu.IncreaseGem(transform.position, rubyAmount);
            DataController.Instance.Ruby += rubyAmount;
            APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        }
        for (int i = 0; i < items.Length; i++)
        {
            DataController.Instance.AddItem(items[i].itemId, items[i].itemQuantity);
        }
        mainMenu.IncreaseItem(transform.position);
        DataController.Instance.SaveData();
        FindObjectOfType<ZoneOfferController>().ShowZoneOfferBtn();
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        OnHide();
    }
    public void SetupData()
    {
        string welcome = Lean.Localization.LeanLocalization.GetTranslationText("welcome", "Welcome");
        int highestRes = DataController.Instance.HighestRestaurant;
        string resName = Lean.Localization.LeanLocalization.GetTranslationText("res_name_" + highestRes);
        packageNameTxt.text = welcome + " " + resName;
        if (highestRes > PlayerPrefs.GetInt("has_show_zone_offer", 1))
            PlayerPrefs.SetInt("has_show_zone_offer", highestRes);
        DisplayZoneSymbol(highestRes);
    }
    public void DisplayZoneSymbol(int highestRes)
    {
        int zoneIndex = 0;
        if (highestRes >= 5 && highestRes <= 8) zoneIndex = 1;
        else if (highestRes >= 9 && highestRes <= 13) zoneIndex = 2;
        else if (highestRes >= 14) zoneIndex = 3;
        leftZoneSymbol.sprite = leftZoneIconSprites[zoneIndex];
        rightZoneSymbol.sprite = rightZoneIconSprites[zoneIndex];
        japanSymbol1.SetActive(zoneIndex == 0);
        japanSymbol2.SetActive(zoneIndex == 0);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        animator.Play("Disappear");
        Destroy(gameObject, 0.3f);
    }
}
