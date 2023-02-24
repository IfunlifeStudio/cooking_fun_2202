using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Purchasing;

public class ShopRubyBtnController : MonoBehaviour
{
    [SerializeField] private GameObject rubyPropPrefab;
    private float timeStamp;
    [SerializeField] private Transform rubyIcon;
    [SerializeField] private int rubyAmount;
    [SerializeField] private int limitedEnergy = 0;
    private void Update()
    {
        float deltaTime = Time.time - timeStamp;//update top bar coin progress
        if (deltaTime < 0.1f)
        {
            float curentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            rubyIcon.localScale = new Vector3(curentScale, curentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
            {
                rubyIcon.localScale = Vector3.one;
            }
        }
    }
    public void OnClickShowMore()
    {
        GetComponentInParent<ShopController>().OnShowMore();
    }
    public void OnClickBuyRuby()
    {
        //DataController.Instance.RemoveAds = 1;
        DataController.Instance.Ruby += rubyAmount;
        if (limitedEnergy > 0)
        {
            DataController.Instance.AddUnlimitedEnergy(limitedEnergy);
            FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
        }
        DataController.Instance.SaveData();
        for (int i = 0; i < 5; i++)
        {
            GameObject ruby = Instantiate(rubyPropPrefab, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, rubyIcon.parent.parent);
            Vector3 target = ruby.transform.position + new Vector3(Random.Range(-40.5f, 40.5f), Random.Range(-40.5f, 40.5f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updatePropPos = (t) =>
            {
                ruby.transform.position = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> completedPropMovement = (t) =>
            {
                Destroy(ruby);
                timeStamp = Time.time;
            };
            TweenFactory.Tween("ruby" + i + Time.time, ruby.transform.position, target, 0.5f + i * 0.06f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
            .ContinueWith(new Vector3Tween().Setup(target, rubyIcon.position, 0.25f + i * 0.06f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
        }
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
    }
}
