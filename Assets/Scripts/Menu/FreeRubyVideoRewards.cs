using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FreeRubyVideoRewards : MonoBehaviour
{
    [SerializeField] private AdsRewardController adsRewardPanel;
    [SerializeField] private SkeletonGraphic notification;
    private void Start()
    {
        gameObject.SetActive(DataController.Instance.GetLevelState(1, 3) > 0);
        RewardCheck();
    }
    public void RewardCheck()
    {
        DataController.Instance.VideoRewardDuration -= (int)(DataController.ConvertToUnixTime(DateTime.UtcNow) - DataController.Instance.VideoRewardTimeStamp);
        if (DataController.Instance.VideoRewardDuration < 0) DataController.Instance.VideoRewardDuration = 0;
        notification.gameObject.SetActive(PlayerPrefs.GetInt("received_ads_progress", 0) <= 5 && DataController.Instance.VideoRewardDuration <= 0);
    }
    public void OnClickShowAdsPanel()
    {
        Instantiate(adsRewardPanel, FindObjectOfType<MainMenuController>().cameraCanvas);
    }

}
