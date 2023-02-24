using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyOfferPanelController : UIView
{
    [SerializeField] private string adsName = "daily_offer";
    [SerializeField] private DailyOfferItemController[] DailyOfferPrefabs;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject adsBlur, rewardTab;
    [SerializeField] Transform ItemPanel;
    void Start()
    {
        bool canShowAds = (PlayerPrefs.GetInt("watched_daily_ads",0)==0) && AdsController.Instance.IsRewardVideoAdsReady();
        adsBlur.SetActive(!canShowAds);
        animator.Play("Appear");
        SetupDailyOffer();
        ShowDailyOffer();      
    }
    public void ShowDailyOffer()
    {
        int Rank = DataController.Instance.Rank;
        int tmpRank;
        if (Rank == 5) tmpRank = 4;
        else if (Rank == 0) tmpRank = 1;
        else tmpRank = Rank;
        for (int index = 0; index < 2; index++)
        {
            var item = Instantiate(DailyOfferPrefabs[tmpRank - 1 + index], ItemPanel);
            item.Init(index);
        }
        UIController.Instance.PushUitoStack(this);
    }
    public void SetupDailyOffer()
    {
        MainMenuController.isFirstOpenDailyOffer = false;
        if (PlayerPrefs.GetInt("day_of_year", 0) != System.DateTime.Now.DayOfYear)
        {
            PlayerPrefs.SetInt("day_of_year", System.DateTime.Now.DayOfYear);
            PlayerPrefs.SetInt("DailyOffer0", 0);
            PlayerPrefs.SetInt("DailyOffer1", 0);
            PlayerPrefs.SetInt("watched_daily_ads", 0);
        }
    }
    public void OnClickAds()
    {
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnClosedAds);
    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        adsBlur.SetActive(true);
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();

        DataController.Instance.Ruby += 5;
        mainMenu.IncreaseGem(rewardTab.transform.position, 5);
        //AddGold
        mainMenu.IncreaseCoin(rewardTab.transform.position, 200);
        DataController.Instance.Gold += 200;
        //Addtime
        FindObjectOfType<EnergyController>().StopAllCoroutines();
        DataController.Instance.AddUnlimitedEnergy(10);
        FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect(rewardTab.transform);
        APIController.Instance.LogEventEarnRuby(5, "reward");
        APIController.Instance.LogEventEarnGold(200, "reward");
        int totalWatch = PlayerPrefs.GetInt("total_watched_ads", 0);
        PlayerPrefs.SetInt("watched_daily_ads", 1);
        PlayerPrefs.SetInt("total_watched_ads", totalWatch++);
        DataController.Instance.AddIapDaily();
    }
    public void OnClosedAds()
    {

    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        FindObjectOfType<DailyOfferController>().CanDisplayDailyOffer();
        animator.Play("Disappear");
        FindObjectOfType<MainMenuController>().DislayDailyReward();
        Destroy(gameObject, 0.3f);
    }
}
