using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.Purchasing;
using System.Globalization;
public class FlashSalePanelController : UIView
{
    [SerializeField] private string packId = "com.cscmobi.cooking.love.free.flashsale101";
    [SerializeField] private int rubyAmount, unlimitedEnergyTime;
    [SerializeField] private int[] itemIds, itemAmounts;
    [SerializeField] private GameObject itemPropPrefab, quitPanel;
    [SerializeField] float packValue;
    bool isBought = false;
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
        UIController.Instance.PushUitoStack(this);
    }
    public override void OnHide()
    {
        StartCoroutine(DelayClosePanel());
        UIController.Instance.PopUiOutStack();
    }
    private IEnumerator DelayClosePanel()
    {
        if (isBought)
            PlayerPrefs.SetFloat("fs_waittime", 86400);
        else
            PlayerPrefs.SetFloat("fs_waittime", 172800);
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void OnShowQuitPanel()
    {
        quitPanel.SetActive(true);
    }
    public void OnHideQuitPanel()
    {
        quitPanel.SetActive(false);
    }
    public void OnClickBuy()
    {
        PlayerPrefs.SetInt(packId, 1);
        if (rubyAmount > 0)
        {
            DataController.Instance.Ruby += rubyAmount;
            FindObjectOfType<MainMenuController>().IncreaseGem(transform.position, rubyAmount);
            APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        }
        if (unlimitedEnergyTime > 0)
        {
            DataController.Instance.AddUnlimitedEnergy(unlimitedEnergyTime);
            FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        }
        Transform shopIcon = FindObjectOfType<MainMenuController>().shopIcon;
        if (itemIds.Length > 0)
        {
            int itemAmount = 0;
            for (int i = 0; i < itemAmounts.Length; i++)
                itemAmount += itemAmounts[i];
            for (int i = 0; i < itemIds.Length; i++)
            {
                DataController.Instance.AddItem(itemIds[i], itemAmounts[i]);
            }
            for (int i = 0; i < itemAmount; i++)
            {
                GameObject itemProp = Instantiate(itemPropPrefab, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform.parent);
                Vector3 target = itemProp.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);//generate a random pos
                System.Action<ITween<Vector3>> updatePropPos = (t) =>
                {
                    itemProp.transform.position = t.CurrentValue;
                };
                System.Action<ITween<Vector3>> completedPropMovement = (t) =>
                {
                    Destroy(itemProp);
                };
                TweenFactory.Tween("item" + i + Time.time, itemProp.transform.position, target, 0.5f + i * 0.06f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
                .ContinueWith(new Vector3Tween().Setup(target, shopIcon.position, 0.5f + i * 0.06f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
            }
        }
        isBought = true;
        DataController.Instance.AddIapDaily(packValue);
        DataController.Instance.SaveData();
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        StartCoroutine(DelayClosePanel());
    }
}
