using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class DailyOfferItemController : MonoBehaviour
{
    [SerializeField] private string packageId = "com.cscmobi.cooking.love.free.dailyoffer01";
    [SerializeField] int rubyAmount, goldAmount, unlimitedEnergyTime;
    [SerializeField] private Image iconPack;
    [SerializeField] private Sprite[] iconPackSprites;
    [SerializeField] GameObject ribbon, blur;
    [SerializeField] float packValue;
    int rankValue = 0;
    public void Init(int indexPack)
    {
        iconPack.sprite = iconPackSprites[indexPack];
        ribbon.SetActive(indexPack == 1);
        this.rankValue = indexPack;
        blur.SetActive(PlayerPrefs.GetInt("DailyOffer" + rankValue, 0) == 1);
    }
    public void OnClickBuyDailyOffer()
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        //AddRuby   
        DataController.Instance.Ruby += rubyAmount;
        mainMenu.IncreaseGem(iconPack.transform.position/*+new Vector3( Random.Range(-100,100), Random.Range(-100, 100))*/, rubyAmount);
        //AddGold
        mainMenu.IncreaseCoin(iconPack.transform.position/* + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100))*/, goldAmount);
        DataController.Instance.Gold += goldAmount;
        //Addtime
        FindObjectOfType<EnergyController>().StopAllCoroutines();
        DataController.Instance.AddUnlimitedEnergy(unlimitedEnergyTime);
        PlayerPrefs.SetInt("DailyOffer" + rankValue, 1);
        FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        DataController.Instance.AddIapDaily(packValue);
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        APIController.Instance.LogEventEarnGold(goldAmount, "buy_IAP");
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        blur.SetActive(true);
        FindObjectOfType<DailyOfferPanelController>().OnHide();
    }
}
