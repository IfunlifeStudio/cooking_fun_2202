using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DailyRewardController : UIView
{
    public string adsName = "x2Daily";
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject fireworkPrefab;
    [SerializeField] private Transform[] spawnsPos;
    private int rewardProgress, lastRewardDay;
    [SerializeField] private GameObject dailyRewardBtnPrefab;
    private DailyRewardObjects dailyRewards;
    private int weeklyRewardProgress, lastReceivedDate;

    void Start()
    {
        LoadDailyRewardData();
        weeklyRewardProgress = PlayerPrefs.GetInt("weekly_reward_progress", 0);
        dailyRewards = GetDailyRewardObjectsByWeek(weeklyRewardProgress);
        int receivedProgress = PlayerPrefs.GetInt("received_progress", 0);
        if (receivedProgress == 0)
        {
            PlayerPrefs.SetInt("last_received_date", System.DateTime.Now.DayOfYear-1);
        }
        bool canDouble = AdsController.Instance.CanShowAds(adsName);
        for (int i = 0; i < dailyRewards.DailyReward.Count; i++)
        {
            GameObject dailyRewardBtn= Instantiate(dailyRewardBtnPrefab, spawnsPos[i].position, Quaternion.identity, spawnsPos[i]);
            dailyRewardBtn.GetComponent<DailyBtnController>().Init(dailyRewards.DailyReward[i].ItemID, dailyRewards.DailyReward[i].Quantity, i+1, receivedProgress, canDouble,OnHide);
        }
        AudioController.Instance.PlaySfx(popUpClip);
        UIController.Instance.PushUitoStack(this);
    }
    public void LoadDailyRewardData()
    {
        dailyRewards = JsonUtility.FromJson<DailyRewardObjects>(FirebaseServiceController.Instance.GetDailyRewardData());
    }
    public static bool CanClaimReward()
    {
        int _lastRewardDay = PlayerPrefs.GetInt("last_received_date", -1);
        bool isClaimed = (_lastRewardDay == System.DateTime.Now.DayOfYear);
        return !isClaimed;
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
    public DailyRewardObjects GetDailyRewardObjectsByWeek(int weekIndex)
    {
        DailyRewardObjects drObjects = new DailyRewardObjects();
        int count = 7;
        weekIndex %= 4;
        for (int i = weekIndex * 7; i < dailyRewards.DailyReward.Count; i++)
        {
            if (count > 0)
            {
                drObjects.DailyReward.Add(dailyRewards.DailyReward[i]);
                count--;
            }
            else
                break;
        }
        return drObjects;
    }
}
[System.Serializable]
public class CookingReward
{
    public int ItemID;
    public int Quantity;
}
[System.Serializable]
public class DailyRewardObjects
{
    [SerializeField]
    public List<CookingReward> DailyReward;
    public DailyRewardObjects()
    {
        DailyReward = new List<CookingReward>();
    }
}
