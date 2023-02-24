using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.Purchasing;
using System.Globalization;

public class PackPanelController : UIView
{
    [SerializeField] private string packId = "com.cscmobi.cooking.love.free.saletest";
    [SerializeField] private string oldPackId = "com.cscmobi.cooking.love.free.packsale";
    [SerializeField] private int rubyAmount, unlimitedEnergyTime;
    [SerializeField] private int[] itemIds, itemAmounts;
    [SerializeField] private GameObject itemPropPrefab;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI NewPriceText;
    [SerializeField] private TextMeshProUGUI OldPriceText;
    private float packLifeTime, packTimeStamp, timeStamp = 0;
    private void Start()
    {
        if (CodelessIAPStoreListener.initializationComplete)
        {
            var product = CodelessIAPStoreListener.Instance.GetProduct(packId);
            if (oldPackId != "" && OldPriceText != null)
            {
                var oldProduct = CodelessIAPStoreListener.Instance.GetProduct(oldPackId);
                OldPriceText.text = oldProduct.metadata.localizedPriceString;
            }
            if (product != null)
                NewPriceText.text = product.metadata.localizedPriceString;
        }
        packLifeTime = IAPPackHelper.GetPackLifeTime(packId);
        packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + packId, 0);
        GetComponent<Animator>().Play("Appear");
        float deltaTime = (float)(packTimeStamp + packLifeTime - DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        countDownText.gameObject.SetActive(deltaTime > 0);
        UIController.Instance.PushUitoStack(this);
    }
    private void Update()
    {
        if (Time.time - timeStamp > 1 && countDownText.gameObject.activeInHierarchy)
        {
            timeStamp = Time.time;
            float deltaTime = (float)(packTimeStamp + packLifeTime - DataController.ConvertToUnixTime(System.DateTime.UtcNow));
            int hours = DataController.GetHours(deltaTime);
            if (hours > 0)
                countDownText.text = "<color=#005ed2>" + Lean.Localization.LeanLocalization.GetTranslationText("sale_description", "Sale time left:") + "</color> <color=#e42803>" + hours + "h " + DataController.GetMinutes(deltaTime) + "m " + DataController.GetSeconds(deltaTime) + "s</color>";
            else
                countDownText.text = "<color=#005ed2>" + Lean.Localization.LeanLocalization.GetTranslationText("sale_description", "Sale time left:") + "</color> <color=#e42803>" + DataController.GetMinutes(deltaTime) + "m " + DataController.GetSeconds(deltaTime) + "s</color>";
        }
    }
    public override void OnHide()
    {
        StartCoroutine(DelayClosePanel());
        UIController.Instance.PopUiOutStack();
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void OnClickBuy()
    {
        PlayerPrefs.SetInt(packId, 1);
        //DataController.Instance.RemoveAds = 1;
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
        Transform shopIcon = FindObjectOfType<PacksLogicController>().shopBtn;
        if (itemIds.Length > 0)
        {
            int itemAmount = 0;
            for (int i = 0; i < itemAmounts.Length; i++)
                itemAmount += itemAmounts[i];
            for (int i = 0; i < itemIds.Length; i++)
                DataController.Instance.AddItem(itemIds[i], itemAmounts[i]);
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
        DataController.Instance.SaveData();
        StartCoroutine(DelayClosePanel());
    }
}
