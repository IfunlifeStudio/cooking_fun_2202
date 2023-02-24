using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using UnityEngine.Purchasing;
using DigitalRuby.Tween;

public class StarterPackPanelController : UIView
{
    [SerializeField] private string packId = "com.cscmobi.cooking.love.free.saletest";
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private GameObject itemEffectPrefab;
    [SerializeField] private int rubyAmount;
    private float packLifeTime, packTimeStamp, timeStamp = 0;
    string leftTime = "";
    int starterPackLifeTime = 10800;
    private void Start()
    {
        leftTime = Lean.Localization.LeanLocalization.GetTranslationText("sale_description", "Sale time left: ");
        packTimeStamp = PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0);
        if (packId == "com.cscmobi.cooking.love.free.starterpack90")
        {
            System.DateTime now = System.DateTime.Now;
            System.DateTime tomorrow = now.AddDays(1).Date;
            packLifeTime = (float)(tomorrow - now).TotalSeconds;
        }
        else if (packId == "com.cscmobi.cooking.love.free.starterpack")
        {
            packLifeTime = (float)(packTimeStamp + starterPackLifeTime - (DataController.ConvertToUnixTime(System.DateTime.UtcNow))); ;
        }

        GetComponent<Animator>().Play("Appear");
        countDownText.gameObject.SetActive(packLifeTime > 0);
        UIController.Instance.PushUitoStack(this);
    }
    private void Update()
    {
        if (Time.time - timeStamp > 1)
        {
            packLifeTime -= 1;
            if (packLifeTime < 0) packLifeTime = 0;
            if (packLifeTime > 0)
            {
                int hours = DataController.GetHours(packLifeTime);
                if (hours > 0)
                    countDownText.text = leftTime + System.String.Format(" {0:D2}:{1:D2}:{2:D2}", (int)packLifeTime / 3600, (int)(packLifeTime / 60) % 60, (int)packLifeTime % 60);
                else
                    countDownText.text = leftTime + System.String.Format(" {0:D2}:{1:D2}", (int)(packLifeTime / 60) % 60, (int)packLifeTime % 60);
            }
            timeStamp = Time.time;
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClosePanel());
        //if (LevelDataController.Instance.lastestPassedLevel != null)
        //    FindObjectOfType<MainMenuController>().DisplayMenuPanel();
        FindObjectOfType<StarterPackController>().ShowIcon();
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void OnClickBuy()
    {
        PlayerPrefs.SetInt("has_buy_starter_pack", 1);
        FindObjectOfType<EnergyController>().StopAllCoroutines();
        DataController.Instance.AddUnlimitedEnergy(30);
        FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        DataController.Instance.Ruby += rubyAmount;
        int[] itemIds = DataController.Instance.GetAllItemIds();
        int itemAmount = itemIds.Length;
        for (int i = 0; i < itemIds.Length; i++)
        {
            DataController.Instance.AddItem(itemIds[i], 1);
        }
        DataController.Instance.SaveData();
        FindObjectOfType<MainMenuController>().IncreaseGem(Vector3.zero, rubyAmount);
        for (int i = 0; i < itemAmount; i++)
        {
            GameObject itemProp = Instantiate(itemEffectPrefab, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform.parent);
            Vector3 target = itemProp.transform.position + new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updatePropPos = (t) =>
            {
                itemProp.transform.position = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> completedPropMovement = (t) =>
            {
                Destroy(itemProp);
            };
            TweenFactory.Tween("item" + i + Time.time, itemProp.transform.position, target, 0.3f + i * 0.01f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
            .ContinueWith(new Vector3Tween().Setup(target, new Vector3(950, -180, 0), 0.4f + i * 0.01f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
        }
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        OnHide();
    }
}
