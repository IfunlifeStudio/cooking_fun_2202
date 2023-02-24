using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestElementController : MonoBehaviour
{
    [SerializeField] private string adsName = "default";
    [SerializeField] private int questIndex = 0;
    [SerializeField] private TextMeshProUGUI tittle, description, progressText, rewardText;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject claimBtn, adsRefreshBtn, rubyRefreshBtn, tickIcon;
    private float originProgressWidth;
    private void Start()
    {
        UpdateElementDetail();
    }
    private void UpdateElementDetail()
    {
        var quest = QuestController.Instance.activeQuests[questIndex];
        tittle.text = quest.Name;
        description.text = quest.Description;
        rewardText.text = quest.coinReward.ToString();
        originProgressWidth = progressBar.GetComponent<RectTransform>().sizeDelta.x;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(originProgressWidth * quest.count * 1f / quest.rewardThreshold, progressBar.GetComponent<RectTransform>().sizeDelta.y);
        claimBtn.SetActive(false);
        adsRefreshBtn.SetActive(false);
        rubyRefreshBtn.SetActive(false);
        tickIcon.SetActive(false);
        progressText.text = quest.Progress;
        if (quest.isClaimed)
            tickIcon.SetActive(true);
        else
        {
            if (quest.IsCompleted() && !quest.isClaimed)
                claimBtn.SetActive(true);
            else
            {
                if (AdsController.Instance.IsRewardVideoAdsReady() && AdsController.Instance.CanShowAds(adsName))
                    adsRefreshBtn.SetActive(true);
                else
                    rubyRefreshBtn.SetActive(true);
            }
        }
    }
    public void OnClickClaim()
    {
        FindObjectOfType<QuestPanelController>().UpdatePanelDisplay();
        FindObjectOfType<MainMenuController>().IncreaseCoin(claimBtn.transform.position, QuestController.Instance.activeQuests[questIndex].coinReward);
        QuestController.Instance.ClaimQuest(questIndex);
        DataController.Instance.Gold += QuestController.Instance.activeQuests[questIndex].coinReward;
        DataController.Instance.SaveData();
        UpdateElementDetail();
        FindObjectOfType<QuestPanelController>().UpdatePanelDisplay();
    }
    public void OnClickAdsRefresh()
    {
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
    }
    public void OnClickRubyRefresh()
    {
        if (DataController.Instance.Ruby > 5)
        {
            DataController.Instance.Ruby -= 5;
            DataController.Instance.SaveData();
            QuestController.Instance.RenewQuest(questIndex);
            UpdateElementDetail();
        }
        else
            FindObjectOfType<ShopController>().OpenShopPanel(3);
    }
    public void OnCloseAds()
    {
    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        StartCoroutine(DelayShowReward());
    }
    private IEnumerator DelayShowReward()
    {
        yield return new WaitForSeconds(0.1f);
        QuestController.Instance.RenewQuest(questIndex);
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        UpdateElementDetail();
    }
}
