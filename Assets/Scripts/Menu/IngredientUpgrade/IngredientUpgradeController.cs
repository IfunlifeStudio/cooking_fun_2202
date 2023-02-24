using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
using System;
using Spine.Unity;

public class IngredientUpgradeController : MonoBehaviour
{
    [SerializeField] private GameObject detailPanel, costPanel, processPanel, maxPanel;
    [SerializeField] private IngredientBtnController[] ingredientBtns;
    [SerializeField] private Button upgradeBtn, adsBtn;
    [SerializeField] private TextMeshProUGUI ingredientName, currentGoldValue, upgradedGoldValue, rewardExpText, upgradeCostText, upgradeTimeText, countdownTimeText, getNowRubyText;
    [SerializeField] private GameObject tapParticle, upgradeEffect, desUpgradeTime;
    [SerializeField] private SkeletonGraphic spineCar, spineProcessBg;
    [SerializeField] Slider slProcess;
    //[SerializeField] private GameObject ingredientHelpingHand, upgradeHelpingHand;
    private bool canDisplayUpgradeHand = false, hasClaimReward = false;
    public IngredientBtnController currentIngredient;
    [SerializeField] private AudioClip rewardAudio;
    int duration = 0, getnowTimebyRuby, upgradeTime, conversTimeValue;
    public IngredientUpgradeController Spawn(Transform detailPanel)
    {
        GameObject go = Instantiate(gameObject, detailPanel);
        go.transform.SetSiblingIndex(detailPanel.childCount - 7);
        IngredientUpgradeController result = go.GetComponent<IngredientUpgradeController>();
        return result;
    }
    private void Start()
    {
        var dataController = DataController.Instance;
        int recommendIngredient = dataController.GetRecommendIngredientUpgradeId();
        bool hasSelectIngredient = false;
        foreach (var ingredientBtn in ingredientBtns)
        {
            ingredientBtn.Init();
            if (dataController.IsIngredientUnlocked(ingredientBtn.ingredientId) && ingredientBtn.ingredientId == recommendIngredient)
            {
                ingredientBtn.OnSelectIngredient();
                hasSelectIngredient = true;
            }
        }
        if (!hasSelectIngredient)
        {
            foreach (var ingredientBtn in ingredientBtns)
            {
                if (dataController.IsIngredientUnlocked(ingredientBtn.ingredientId) && dataController.GetIngredientUpgradeCost(ingredientBtn.ingredientId) <= dataController.Gold)
                {
                    ingredientBtn.OnSelectIngredient();
                    hasSelectIngredient = true;
                    break;
                }
            }
        }
        if (!hasSelectIngredient)
            ingredientBtns[0].OnSelectIngredient();
    }
    public bool IsIngredientUpgradeAvailable()
    {
        bool result = false;
        var dataController = DataController.Instance;
        foreach (var ingredientBtn in ingredientBtns)
        {
            if (dataController.IsIngredientUnlocked(ingredientBtn.ingredientId)
            && dataController.GetIngredientUpgradeCost(ingredientBtn.ingredientId) <= dataController.Gold
            && dataController.GetIngredientLevel(ingredientBtn.ingredientId) < 3)
            {
                result = true;
                break;
            }
            else
            {
                if (dataController.HasJustCompletedIngredientUpgradeProcess(ingredientBtn.ingredientId))
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    public void HidePanel()
    {
        Destroy(gameObject);
    }
    public void OnSelectIngredient(IngredientBtnController ingredientBtn)
    {
        if (currentIngredient != null) currentIngredient.OnLostFocus();
        currentIngredient = ingredientBtn;
        StopAllCoroutines();
        StartCoroutine(UpdateDetailPanelView());
    }
    private bool isUpgrading;
    public void OnClickUpgrade()
    {
        if (isUpgrading) return;
        isUpgrading = true;
        GameObject go = Instantiate(tapParticle, upgradeBtn.transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
        AudioController.Instance.PlaySfx(rewardAudio);
        int upgradeCost = DataController.Instance.GetIngredientUpgradeCost(currentIngredient.ingredientId);
        string detailSource = "upgrade_ingredient";
        if (DataController.Instance.Gold < upgradeCost)
        {
            FindObjectOfType<MainMenuController>().ShowUpgradeWithRubyPanel(upgradeCost, UpgradeIngredient, detailSource);
            isUpgrading = false;
        }
        else
        {
            DataController.Instance.Gold -= upgradeCost;
            UpgradeIngredient();
            APIController.Instance.LogEventSpentGold(upgradeCost, detailSource);
        }
    }
    public void UpgradeIngredient()
    {
        currentIngredient.SetUpgradeTagActive(true);
        var ingredient = DataController.Instance.GetIngredient(currentIngredient.ingredientId);
        int upgradeCost = ingredient.UpgradePrice();
        upgradeTime = ingredient.UpgradeTime;
        DataController.Instance.AddIngredientUpgradeProcess(currentIngredient.ingredientId);
        if (upgradeTime == 0)
            OnCompleteUpgrade();
        else
        {
            PushNotification.Instance.SendCompleteUpgradeNotification(upgradeTime);
            currentIngredient.AutoWaitForUpgrade(upgradeTime);
            StartCoroutine(DelayUpgrade());
        }
        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnUpgradeIngredient, new object[] { DataController.Instance.GetIngredientLevel(currentIngredient.ingredientId), upgradeCost }));
        DataController.Instance.SaveData();
    }
    private IEnumerator DelayUpgrade()
    {
        yield return null;
        GameObject go = Instantiate(upgradeEffect, currentIngredient.transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
        StartCoroutine(UpdateDetailPanelView());
        isUpgrading = false;
    }
    private IEnumerator UpdateDetailPanelView()
    {
        int ingredientId = currentIngredient.ingredientId;
        DataController dataController = DataController.Instance;
        detailPanel.SetActive(false);
        costPanel.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        var ingredient = dataController.GetIngredient(ingredientId);

        ingredientName.text = ingredient.ingredientName;
        currentGoldValue.text = ingredient.Gold().ToString();
        if (dataController.GetIngredientLevel(ingredientId) == 3)
        {
            costPanel.SetActive(false);
            processPanel.SetActive(false);
            maxPanel.SetActive(true);
        }
        else
        {
            maxPanel.SetActive(false);
            var currentUpgradeProcess = DataController.Instance.GetGameData().GetUpgradeIngredientProcessing(ingredientId);
            upgradeTime = ingredient.UpgradeTime;
            upgradeTimeText.gameObject.SetActive(upgradeTime != 0);
            desUpgradeTime.SetActive(upgradeTime != 0);
            if (currentUpgradeProcess != null && currentUpgradeProcess.timeStamp != 0)
            {
                bool canShowAds = AdsController.Instance.CanShowAds("x2speed_waiting_upgrade") && AdsController.Instance.IsRewardVideoAdsReady() && !currentUpgradeProcess.hasUsedX2Speed;
                adsBtn.interactable = canShowAds;
                if (canShowAds)
                    APIController.Instance.LogEventShowAds("x2speed_waiting_upgrade");
                processPanel.SetActive(true);
                int multiple = 1;
                if (currentUpgradeProcess.hasUsedX2Speed)
                {
                    spineCar.AnimationState.SetAnimation(0, "play-2", true);
                    spineProcessBg.timeScale = 1.5f;
                    multiple = 2;
                }
                else
                {
                    spineProcessBg.timeScale = 0.5f;
                    spineCar.AnimationState.SetAnimation(0, "play", true);
                    multiple = 1;
                }
                duration = (int)(upgradeTime - multiple * (DataController.ConvertToUnixTime(DateTime.UtcNow) - currentUpgradeProcess.timeStamp));
                costPanel.SetActive(false);
                StopAllCoroutines();
                if (duration > 0)
                    StartCoroutine(ShowUpgradeProcess(currentUpgradeProcess, upgradeTime, multiple));
            }
            else
            {
                upgradeTimeText.text = String.Format("{0:D2}:{1:D2}:{2:D2}", ingredient.UpgradeTime / 3600, (ingredient.UpgradeTime / 60) % 60, ingredient.UpgradeTime % 60);
                costPanel.SetActive(true);
                processPanel.SetActive(false);
            }
            System.Action<ITween<Vector3>> updateUpgradeScale = (t) =>
                   {
                       if (upgradeBtn != null)
                           upgradeBtn.transform.localScale = t.CurrentValue;
                   };
            TweenFactory.Tween("UpgradeBtn" + Time.time, upgradeBtn.transform.localScale, new Vector3(1.2f, 1.2f, 1), 0.1f, TweenScaleFunctions.QuinticEaseOut, updateUpgradeScale)
             .ContinueWith(new Vector3Tween().Setup(new Vector3(1.2f, 1.2f, 1), new Vector3(0.95f, 0.95f, 1), 0.1f, TweenScaleFunctions.QuinticEaseInOut, updateUpgradeScale))
             .ContinueWith(new Vector3Tween().Setup(new Vector3(0.95f, 0.95f, 1), new Vector3(1f, 1f, 1f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updateUpgradeScale))
             ;
            upgradedGoldValue.transform.parent.gameObject.SetActive(true);
            upgradedGoldValue.text = ingredient.GetUpgradedGoldValue().ToString();
            upgradeCostText.text = ingredient.UpgradePrice().ToString();
        }
        detailPanel.SetActive(true);
    }
    IEnumerator ShowUpgradeProcess(UpgradeIngredientProcess currentUpgradeProcess, float neededUpgradeTime, int multiple)
    {
        float timeDelay = 1 / multiple;
        var delay = new WaitForSecondsRealtime(timeDelay);
        var adsBtntxt = adsBtn.GetComponentInChildren<TextMeshProUGUI>();
        var resData = DataController.Instance.GetRestaurantById(DataController.Instance.currentChapter);
        int nowAdsTime = resData.nowAdsTime;
        string x2speedStr = Lean.Localization.LeanLocalization.GetTranslationText("x2_speed");
        string getNowStr = Lean.Localization.LeanLocalization.GetTranslationText("get_now");
        getnowTimebyRuby = resData.nowRubyTime;
        conversTimeValue = resData.conversTimeValue;
        while (duration > 0)
        {
            duration = (int)(upgradeTime - multiple * (DataController.ConvertToUnixTime(DateTime.UtcNow) - currentUpgradeProcess.timeStamp));
            countdownTimeText.text = String.Format("{0:D2}:{1:D2}:{2:D2}", duration / 3600, (duration / 60) % 60, duration % 60);
            slProcess.value = 1 - (float)(duration / neededUpgradeTime);
            if (duration <= getnowTimebyRuby)
            {
                getNowRubyText.text = "Free";
            }
            else
            {
                getNowRubyText.text = (duration / conversTimeValue + 1).ToString();
            }
            if (nowAdsTime < duration)
                adsBtntxt.text = x2speedStr;
            else
            {
                adsBtntxt.text = getNowStr;
                adsBtn.interactable = true;
            }

            yield return delay;
        }
        OnCompleteUpgrade();
    }
    public void OnClickGetNowbyruby()
    {
        if (duration <= getnowTimebyRuby)
        {
            OnCompleteUpgrade();
        }
        else
        {
            int ruby = (duration / conversTimeValue + 1);
            if (DataController.Instance.Ruby > ruby)
            {
                DataController.Instance.Ruby -= ruby;
                OnCompleteUpgrade();
                APIController.Instance.LogEventSpentRuby(ruby, "finished_upgrade");
            }
            else
            {
                FindObjectOfType<ShopController>().OpenShopPanel(3);
            }
        }
    }
    public void OnCompleteUpgrade()
    {
        currentIngredient.OnCompletedUpgrade();
        StopAllCoroutines();
        StartCoroutine(UpdateDetailPanelView());
        isUpgrading = false;
    }
    public void OnClickGetNowbyAds()
    {
        AdsController.Instance.ShowVideoReward(onUserEarnedReward, onAdClosed);
    }

    private void onAdClosed()
    {
        if (!hasClaimReward) return;
        var resData = DataController.Instance.GetRestaurantById(DataController.Instance.currentChapter);
        if (duration <= resData.nowAdsTime)
            OnCompleteUpgrade();
        else
        {
            adsBtn.interactable = false;
            var currentUpgradeProcess = DataController.Instance.GetGameData().GetUpgradeIngredientProcessing(currentIngredient.ingredientId);
            currentUpgradeProcess.hasUsedX2Speed = true;
            currentIngredient.Init();
            StopAllCoroutines();
            StartCoroutine(UpdateDetailPanelView());
        }

        hasClaimReward = false;
        APIController.Instance.LogEventViewAds("x2speed_waiting_upgrade");
    }

    private void onUserEarnedReward()
    {
        APIController.Instance.LogEventRewarded();
        hasClaimReward = true;
    }
}
