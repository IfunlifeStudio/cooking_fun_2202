using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class SpecialWeekendItemController : MonoBehaviour
{
    [SerializeField] int rubyAmount, goldAmount, unlimitedEnergyTime;
    [SerializeField] Items[] items;
    [SerializeField] GameObject blur, rubyIcon, itemEffectPrefabs;
    [SerializeField] Image saleTagImage;
    [SerializeField] Sprite[] saleTagSprites;
    int packIndex;
    public void Init(int packIndex, bool hasBought, bool isMaxSaleOff)
    {
        blur.SetActive(hasBought);
        if (isMaxSaleOff)
            saleTagImage.sprite = saleTagSprites[0];
        else
            saleTagImage.sprite = saleTagSprites[1];
        this.packIndex = packIndex;
    }
    public void OnClickBuy()
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        //AddRuby   
        DataController.Instance.Ruby += rubyAmount;
        mainMenu.IncreaseGem(transform.position/*+new Vector3( Random.Range(-100,100), Random.Range(-100, 100))*/, rubyAmount);
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        //AddGold
        if (goldAmount > 0)
        {
            mainMenu.IncreaseCoin(transform.position/* + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100))*/, goldAmount);
            DataController.Instance.Gold += goldAmount;
            APIController.Instance.LogEventEarnGold(goldAmount, "buy_IAP");
        }

        if (unlimitedEnergyTime > 0)
        {
            FindObjectOfType<EnergyController>().StopAllCoroutines();
            DataController.Instance.AddUnlimitedEnergy(unlimitedEnergyTime);
            FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        }
        if (items.Length > 0)
        {
            int ingameItemQuantity = 0;
            for (int i = 0; i < items.Length; i++)
            {
                DataController.Instance.AddItem(items[i].itemId, items[i].itemQuantity);
                ingameItemQuantity++;
            }
            //ingameItemQuantity = Mathf.Min(5, ingameItemQuantity);
            //for (int i = 0; i < ingameItemQuantity; i++)
            //{
            //    GameObject itemProp = Instantiate(itemEffectPrefabs, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform.parent);
            //    StartCoroutine(IMove(itemProp, mainMenu.shopIcon.transform.position, 1));

            //}
            mainMenu.IncreaseItem(transform.position);
        }
        PlayerPrefs.SetInt("has_bought_sw_" + packIndex, 1);
        DataController.Instance.SaveData();
        //Addtime
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        blur.SetActive(true);
        FindObjectOfType<SpecialWeekendPanelController>().OnHide();
    }
}
