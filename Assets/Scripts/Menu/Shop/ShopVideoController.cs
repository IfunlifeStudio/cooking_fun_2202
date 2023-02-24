using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopVideoController : MonoBehaviour
{
    [SerializeField] private string adsName = "menuRewardRuby";
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button watchBtn;
    private int rewardRubyAmount;
    AdsSetting adsSetting;
    int duration = 0;
    // Start is called before the first frame update
    void Start()
    {
        adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        double deltaTime = (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - DataController.Instance.VideoRewardTimeStamp);
        DataController.Instance.VideoRewardDuration -= (int)deltaTime;
        duration = DataController.Instance.VideoRewardDuration;
        bool canWatchAds = AdsController.Instance.IsRewardVideoAdsReady() &&
            AdsController.Instance.CanShowAds(adsName);
        watchBtn.interactable = canWatchAds;
        if (duration > 0)
        {
            StopAllCoroutines();
            StartCoroutine(CountdownMenuRewardRuby());
        }
        else if (canWatchAds)
        {
            APIController.Instance.LogEventShowAds(adsName);
            statusText.text = Lean.Localization.LeanLocalization.GetTranslationText("shop_watch_video", "Watch");
            StopAllCoroutines();
            StartCoroutine(CountdownMenuRewardRuby());
        }
    }
    IEnumerator CountdownMenuRewardRuby()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (true)
        {
            if (duration > 0)
            {
                duration -= 1;
                if (duration < 0) duration = 0;
                watchBtn.interactable = false;
                DataController.Instance.VideoRewardDuration = duration;
                DataController.Instance.VideoRewardTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
                statusText.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", duration / 3600 , (duration / 60) % 60, duration % 60);
            }
            else
            {
                watchBtn.interactable = true;
                statusText.text = Lean.Localization.LeanLocalization.GetTranslationText("shop_watch_video", "Watch");
            }
            yield return delay;
        }

    }
    public void OnCloseAds()
    {
    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        ResetTime();
        StartCoroutine(DelayShowReward());
        APIController.Instance.LogEventViewAds(adsName);
        //FindObjectOfType<FreeRubyVideoRewards>()?.ResetTime();
    }
    public void ResetTime()
    {

        DataController.Instance.VideoRewardTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        DataController.Instance.VideoRewardDuration = adsSetting.adsCooldown;
        duration = DataController.Instance.VideoRewardDuration;
        DataController.Instance.SaveData();
        StopAllCoroutines();
        StartCoroutine(CountdownMenuRewardRuby());
    }
    public void OnClickClaim()
    {
        if (duration <= 0)
            AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
    }
    private IEnumerator DelayShowReward()
    {
        rewardRubyAmount = Random.Range(2, 5);
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        yield return null;
        bool canShowVideoReward = AdsController.Instance.CanShowAds(adsName);
        FindObjectOfType<RewardPanelController>().Init(0, rewardRubyAmount, 0, new int[0], new int[0], true, "x2Ruby");
        FindObjectOfType<ShopController>().OnHide();
    }
}
