using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdsRewardController : UIView
{
    public string adsName = "ads_reward_daily";
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Animator animator;
    [SerializeField] private AdsRewardBtnController[] AdsRewardPanel;
    [SerializeField] private Image progress;
    [SerializeField] private Button watchBtn;
    [SerializeField] private TextMeshProUGUI rewardTimeText;
    [SerializeField] private GameObject timeLeftGo;
    AdsSetting adsSetting;
    List<CookingReward> cookingRewards = new List<CookingReward>();
    int adsRewardProgress = 0;
    int receivedAdsProgress = 0;
    bool isWatchedAds = false;
    bool adsReady = false;
    // Start is called before the first frame update
    void Start()
    {
        adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        adsReady = AdsController.Instance.IsRewardVideoAdsReady() && DataController.Instance.VideoRewardDuration <= 0;
        watchBtn.interactable = adsReady;

        int adsRewardDay = PlayerPrefs.GetInt("ads_reward_day", 0);
        if (adsRewardDay != System.DateTime.Now.DayOfYear)
        {
            PlayerPrefs.SetInt("ads_reward_day", System.DateTime.Now.DayOfYear);
            PlayerPrefs.SetInt("ads_reward_progress", 0);
            PlayerPrefs.SetInt("received_ads_progress", 0);
        }
        adsRewardProgress = PlayerPrefs.GetInt("ads_reward_progress", 0);
        if (adsRewardProgress == 4) adsRewardProgress = 0;
        GetAdsDataByProgress(adsRewardProgress);
        UIController.Instance.PushUitoStack(this);
        StartCoroutine(CountDownAdsReward());
        animator.Play("Appear");
        SetupData();
    }
    public void SetupData()
    {

        receivedAdsProgress = PlayerPrefs.GetInt("received_ads_progress", 0);
        for (int i = 0; i < AdsRewardPanel.Length; i++)
        {
            progress.rectTransform.sizeDelta = new Vector2(1080 / 5 * receivedAdsProgress, 25);
            bool canClaim = i < receivedAdsProgress;
            int itemId = cookingRewards[i].ItemID;
            int quatity = cookingRewards[i].Quantity;
            AdsRewardPanel[i].Init(itemId, quatity, canClaim);
        }
    }
    IEnumerator CountDownAdsReward()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (DataController.Instance.VideoRewardDuration > 0)
        {
            timeLeftGo.SetActive(true);
            FindObjectOfType<FreeRubyVideoRewards>().RewardCheck();
            DataController.Instance.VideoRewardDuration -= (int)(DataController.ConvertToUnixTime(DateTime.UtcNow) - DataController.Instance.VideoRewardTimeStamp);
            if (DataController.Instance.VideoRewardDuration < 0) DataController.Instance.VideoRewardDuration = 0;
            DataController.Instance.VideoRewardTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
            rewardTimeText.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", DataController.Instance.VideoRewardDuration / 3600, (DataController.Instance.VideoRewardDuration / 60) % 60, DataController.Instance.VideoRewardDuration % 60);
            yield return delay;
        }
        FindObjectOfType<FreeRubyVideoRewards>().RewardCheck();
        timeLeftGo.SetActive(false);
        watchBtn.interactable = true;
    }

    public void GetAdsDataByProgress(int progress)
    {
        var adsRewardData = DataController.Instance.adsRewardData;
        int amount = 5;

        for (int i = progress * 5; i < adsRewardData.AdsReward.Count; i++)
        {
            if (amount > 0)
            {
                cookingRewards.Add(adsRewardData.AdsReward[i]);
                amount--;
            }
            else
                break;

        }
    }
    public void OnClickClaim()
    {
        if (DataController.Instance.VideoRewardDuration <= 0)
        {
            AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
        }
    }
    private void OnEarnReward()
    {
        isWatchedAds = true;
        SetupData();
    }
    private void OnCloseAds()
    {
        if (isWatchedAds)
        {
            DataController.Instance.VideoRewardTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
            DataController.Instance.VideoRewardDuration = adsSetting.adsCooldown;
            DataController.Instance.SaveData(false);
            StopAllCoroutines();
            StartCoroutine(CountDownAdsReward());
            OnHide();
            StartCoroutine(DelayShowReward());
            FindObjectOfType<ShopVideoController>()?.ResetTime();
        }

    }
    private IEnumerator DelayShowReward()
    {
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        yield return new WaitForSeconds(0.2f);
        bool canShowVideoReward = AdsController.Instance.CanShowAds(adsName);
        AdsRewardPanel[receivedAdsProgress].GetReward();
        receivedAdsProgress++;
        PlayerPrefs.SetInt("received_ads_progress", receivedAdsProgress);
        if (receivedAdsProgress % 5 == 0)
        {
            adsRewardProgress++;
            PlayerPrefs.SetInt("received_ads_progress", 0);
            PlayerPrefs.SetInt("ads_reward_progress", adsRewardProgress);
        }

        FindObjectOfType<MainMenuController>().setIndexTab(5);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayOnClose());
    }
    IEnumerator DelayOnClose()
    {
        animator.Play("Disappear");
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
[System.Serializable]
public class AdsRewards
{
    [SerializeField]
    public List<CookingReward> AdsReward;
}
