using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class FreeCoinPanelController : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private string adsName = "";
    [SerializeField] private Button videoRewardBtn;
    private int rewardGoldAmount;
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
    }
    public void Init(int rewardAmount)
    {
        APIController.Instance.LogEventShowAds(adsName);
        AudioController.Instance.PlaySfx(popUpClip);
        rewardGoldAmount = rewardAmount;
    }
    public void OnClickClaim()
    {
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
        videoRewardBtn.interactable = false;
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void OnCloseAds()
    {
    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        APIController.Instance.LogEventViewAds(adsName);
        StartCoroutine(DelayShowReward());
    }
    private IEnumerator DelayShowReward()
    {
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        FindObjectOfType<RewardPanelController>().Init(rewardGoldAmount, 0, 0, new int[0], new int[0],false);
        Destroy(gameObject);
    }
}
