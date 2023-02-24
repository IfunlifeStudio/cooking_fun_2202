using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class BonusSundayItemControler : MonoBehaviour
{
    [SerializeField] string packageId = "com.cscmobi.cooking.love.weekendoffer1";
    [SerializeField] int rubyAmount;
    [SerializeField] Items items;
    [SerializeField] GameObject blur, rubyIcon, itemEffectPrefabs;
    string hasboughtItem;
    private void Start()
    {
        blur.SetActive(HasItemBought());
    }
    public bool HasItemBought()
    {
        hasboughtItem = PlayerPrefs.GetString("has_bought_sunday", "");
        if (hasboughtItem != "")
        {
            string[] itemIds = hasboughtItem.Split('_');
            for (int i = 0; i < itemIds.Length; i++)
            {
                int id = int.Parse(itemIds[i]);
                if (id == items.itemId)
                    return true;
            }
        }
        return false;
    }
    public void OnClickBuy()
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        //AddRuby   
        if (rubyAmount > 0)
        {
            DataController.Instance.Ruby += rubyAmount;
            mainMenu.IncreaseGem(rubyIcon.transform.position/*+new Vector3( Random.Range(-100,100), Random.Range(-100, 100))*/, rubyAmount);
            APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        }
        //AddGold
        if (items.itemQuantity > 0)
        {
            DataController.Instance.AddItem(items.itemId, items.itemQuantity);
            //for (int i = 0; i < 5; i++)
            //{
            //    GameObject itemProp = Instantiate(itemEffectPrefabs, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, mainMenu.cameraCanvas);
            //    StartCoroutine(IMove(itemProp, mainMenu.shopIcon.transform.position, 1));
            //}
            mainMenu.IncreaseItem(Vector3.zero);
        }
        string hasboughtItem = PlayerPrefs.GetString("has_bought_sunday", "");
        if (hasboughtItem != "")
            PlayerPrefs.SetString("has_bought_sunday", hasboughtItem + "_" + items.itemId);
        else
            PlayerPrefs.SetString("has_bought_sunday", items.itemId.ToString());
        DataController.Instance.SaveData();
        blur.SetActive(true);
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        FindObjectOfType<BonusSundayPanelController>().OnHide();
    }
}
