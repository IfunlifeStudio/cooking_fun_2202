using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_IOS

namespace AudienceNetwork
{
    public static class AdSettings
    {
        [DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }
    }
}

#endif
public class IronsourceAdsController : MonoBehaviour
{
    private static IronsourceAdsController instance = null;
    public static IronsourceAdsController Instance { get { return instance; } }
    private System.Action onUserEarnedReward = null, onAdClosed = null, onIntersAdsClose = null;
    float timeStamp = 0;
    bool isReadyAdsFull;

    private string appKey = "appkey";
#if UNITY_IOS
    private string appKey = "appkey";
#endif
    bool canGetReward;
    public bool hasInitIronsource = false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (instance == null)
        {
            instance = this;
#if UNITY_IOS && !UNITY_EDITOR
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif
            float timeStamp = Time.time;
            while (Time.time - timeStamp < 180)
                yield return new WaitForSeconds(1);
            if (!hasInitIronsource)
                SetAdsTrackingAndInitIronsource();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void Init()
    {
        StartCoroutine(IEInitIronSource());
    }
    private IEnumerator IEInitIronSource()
    {
        yield return new WaitForSeconds(0.1f);
        if (!hasInitIronsource)
            SetAdsTrackingAndInitIronsource();
    }
    private void SetAdsTrackingAndInitIronsource()
    {
        hasInitIronsource = true;
        bool isAdsConsented = false;
#if UNITY_ANDROID
        isAdsConsented = GDPRPopup.IsUserAllowAdsConsent();
#elif UNItY_IOS
        isAdsConsented=ATTPermissionRequest.IsUserAllowAdsConsent();
#endif
        IronSource.Agent.setConsent(isAdsConsented);
        IronSource.Agent.init(appKey);
        IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;
        #region video reward
        InitializeAndLoadRewardAds();
        #endregion
        #region interstitial
        RequestAndLoadInterstialAds();
        #endregion
    }
    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
    #region RewardVideoAds
    void InitializeAndLoadRewardAds()
    {
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
    }
    private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        if (impressionData != null)
        {
            double revenue = 0;
            if (impressionData.revenue != null) revenue = (double)impressionData.revenue;
            Firebase.Analytics.Parameter[] AdParameters = {
             new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
              new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
              new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
            new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
              new Firebase.Analytics.Parameter("currency","USD"),
                new Firebase.Analytics.Parameter("value", revenue)
        };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }
    public void OnShowRewardedVideoAd(System.Action onUserEarnedReward, System.Action onAdClosed)
    {
        this.onUserEarnedReward = onUserEarnedReward;
        this.onAdClosed = onAdClosed;
        IronSource.Agent.showRewardedVideo();
        AudioController.Instance.PauseMusic();

    }
    public bool IsRewardedVideoAdReady()
    {
        return IronSource.Agent.isRewardedVideoAvailable();
    }
    private void RewardedVideoAdClickedEvent(IronSourcePlacement obj)
    {
    }
    void RewardedVideoAdOpenedEvent()
    {
    }
    void RewardedVideoAdClosedEvent()
    {
        if (this.onAdClosed != null)
        {
            this.onAdClosed.Invoke();
        }
        AudioController.Instance.UnPauseMusic();
    }
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        bool rewardedVideoAvailability = available;
    }
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {

        if (this.onUserEarnedReward != null)
        {
            this.onUserEarnedReward.Invoke();
        }
        else
            Debug.LogError("no ads earned event fired");
    }
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log(" Reward video show fail : " + error.ToString());
    }
    void RewardedVideoAdStartedEvent()
    {

    }
    void RewardedVideoAdEndedEvent()
    {

    }
    #endregion
    #region InterstialVideo
    public void RequestAndLoadInterstialAds()
    {
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        IronSource.Agent.loadInterstitial();
    }
    public bool IsInterstialAdReady()
    {
        return IronSource.Agent.isInterstitialReady();
    }
    public void OnShowInterstialAd(Action onIntersAdsClosed)
    {
        this.onIntersAdsClose = onIntersAdsClosed;
        IronSource.Agent.showInterstitial();
    }
    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {

    }
    void InterstitialAdShowSucceededEvent()
    {
    }
    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        if (!IsInterstialAdReady())
        {
            IronSource.Agent.loadInterstitial();
        }
    }
    void InterstitialAdClickedEvent()
    {
    }
    void InterstitialAdClosedEvent()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (this.onIntersAdsClose != null)
                this.onIntersAdsClose.Invoke();
            if (!IsInterstialAdReady())
            {
                IronSource.Agent.loadInterstitial();
            }
        });
    }
    void InterstitialAdReadyEvent()
    {
    }
    void InterstitialAdOpenedEvent()
    {
    }
    //private void OnEnable()
    //{
    //    RequestAndLoadInterstialAds();
    //}
    #endregion
}