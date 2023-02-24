using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChristmasItemOfferController : MonoBehaviour
{
    [SerializeField] string productId;
    [SerializeField] int rubyAmount, unlimitedEnergyTime;
    [SerializeField] private int[] itemIds;
    [SerializeField] private int[] itemCounts;
    // Start is called before the first frame update
    public void SetBlurActive(bool isBlur)
    {
        GetComponentInChildren<Button>().interactable = !isBlur;
    }
    public void OnClickBuy()
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        //AddRuby   
        DataController.Instance.Ruby += rubyAmount;
        for (int i = 0; i < itemIds.Length; i++)
        {
            if (itemCounts[i] > 0)
                DataController.Instance.AddItem(itemIds[i], itemCounts[i]);
        }
        mainMenu.IncreaseItem(transform.position);
        mainMenu.IncreaseGem(transform.position, rubyAmount);
        //Addtime
        FindObjectOfType<EnergyController>().StopAllCoroutines();
        DataController.Instance.AddUnlimitedEnergy(unlimitedEnergyTime);
        FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        string iap_log = IAPPackHelper.GetContentPack(productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        PlayerPrefs.SetInt("chris_" + productId, 1);
        int temp = PlayerPrefs.GetInt("chris_bought_count_2022", 0);
        temp++;
        PlayerPrefs.SetInt("chris_bought_count_2022", temp);
        DataController.Instance.SaveData();
        SetBlurActive(true);
        string userType = PlayerPrefs.GetString("paying_type", "f1");
        APIController.Instance.LogEventHalloween(userType, "popup_sale", productId);
        //FindObjectOfType<HalloweenOfferPanelController>().OnHide();
    }
}
