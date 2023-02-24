using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoAdsPanelController : MonoBehaviour
{
    //    [SerializeField] private string adsName = "no_interstitial";
    //    public void Start()
    //    {
    //        bool isReadyAds= AdsController.Instance.CanShowAds(adsName);
    //        PlayerPrefs.SetFloat("noads_panel_timestamp", (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
    //        if (isReadyAds)
    //        {
    //            Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_show", new Firebase.Analytics.Parameter[] {
    //            new Firebase.Analytics.Parameter("source", adsName.ToLower()) });
    //        }
    //        GetComponent<Animator>().Play("Appear");
    //    }
    //    public void OnClickVideoAds()
    //    {
    //        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
    //    }
    //    public void OnCloseAds()
    //    {
    //    }
    //    public void OnEarnReward()
    //    {
    //        Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_viewed", new Firebase.Analytics.Parameter[] {
    //            new Firebase.Analytics.Parameter("source",adsName.ToLower()) });
    //        StartCoroutine(DelayShowReward());
    //    }
    //    private IEnumerator DelayShowReward()
    //    {
    //        AdsController.Instance.OnWatchAdsCompleted(adsName);
    //        yield return null;
    //        StartCoroutine(DelayDeactive());
    //    }
    //    public void OnClickBuyNoInterstitialAds()
    //    {
    //        DataController.Instance.RemoveAds = 1;
    //        DataController.Instance.SaveData();
    //        DatabaseController.Instance.SpawnRemoveAdsSuccessOverlay();
    //        OnClickClose();
    //    }
    //    public static bool CanDisplayNoAdsPanel()
    //    {
    //        if (DataController.Instance.RemoveAds == 1) return false;
    //        if (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat("noads_panel_timestamp", 0) < 6 * 3600)
    //            return false;//check panel cooldown and condition
    //        if (AdsController.Instance.interstitialDisplayedCount % 2 == 1 || AdsController.Instance.interstitialDisplayedCount == 0)
    //            return false;//only display no ads after 2 times display full ads
    //        return true;
    //    }
    //    public void OnClickClose()
    //    {
    //        StartCoroutine(DelayDeactive());
    //    }
    //    private IEnumerator DelayDeactive()
    //    {
    //        GetComponent<Animator>().Play("Disappear");
    //        yield return new WaitForSeconds(0.2f);
    //        Destroy(gameObject);
    //    }
}
