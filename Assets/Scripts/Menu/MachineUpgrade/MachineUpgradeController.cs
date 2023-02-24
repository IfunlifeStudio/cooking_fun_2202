using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
using System;
using Spine.Unity;

public class MachineUpgradeController : MonoBehaviour
{
    [SerializeField] private GameObject detailPanel, costPanel, processPanel, maxPanel;
    public MachineBtnController[] machinesBtn;
    [SerializeField] private TextMeshProUGUI machineName, currentCookDuration, upgradedCookDuration, currentMachineCount, upgradedMachineCount, upgradePriceText, upgradeTimeText, countdownTimeText, getNowRubyText;
    [SerializeField] private Button upgradeBtn, adsBtn;
    [SerializeField] private GameObject tapParticle, upgradeEffect, desUpgradeTime;
    [SerializeField] private SkeletonGraphic spineCar, spineProcessBg;
    [SerializeField] Slider processSlider;
    [SerializeField] private AudioClip rewardAudio;
    [SerializeField] private BreadMachineUpgradeTutorial breadMachineUpgradePrefab;
    [SerializeField] private PlateUpgradeTutorial plateMachineUpgradePrefab;
    public MachineBtnController currentMachine;
    int duration = 0, getnowTimebyRuby, upgradeTime, conversTimeValue;
    bool hasClaimReward = false;
    private void Start()
    {
        var dataController = DataController.Instance;
        int recommendmachine = dataController.GetRecommendMachineUpgradeId();
        bool hasSelectMachine = false;
        foreach (var machineBtn in machinesBtn)
        {
            machineBtn.Init();
            if (dataController.IsMachineUnlocked(machineBtn.machineId) && machineBtn.machineId == recommendmachine)
            {
                machineBtn.OnSelectMachine();
                hasSelectMachine = true;
            }
        }
        if (!hasSelectMachine)
        {
            foreach (var machineBtn in machinesBtn)
            {
                if (dataController.IsMachineUnlocked(machineBtn.machineId) && dataController.GetMachineUpgradeCost(machineBtn.machineId) <= dataController.Gold)
                {
                    machineBtn.OnSelectMachine();
                    hasSelectMachine = true;
                    break;
                }
            }
        }
        if (!hasSelectMachine)
            machinesBtn[0].OnSelectMachine();
        bool canSpawnPlateTut = plateMachineUpgradePrefab != null && dataController.GetMachineUpgradeCost(1105) < dataController.Gold && dataController.GetNextLevel(1) == 4 && dataController.GetMachineLevel(1105) < 2;
        if (breadMachineUpgradePrefab != null && dataController.GetMachineUpgradeCost(1100) < dataController.Gold && dataController.GetNextLevel(1) == 4 && dataController.GetMachineLevel(1100) < 2)//tutorial upgrade bread machine
        {
            UIController.Instance.CanBack = false;
            Transform cameraCanvas = FindObjectOfType<MainMenuController>().cameraCanvas;
            machinesBtn[0].OnSelectMachine();
            breadMachineUpgradePrefab.Spawn(upgradeBtn.transform.position, cameraCanvas, canSpawnPlateTut);
            FindObjectOfType<MainMenuController>().setIndexTab(5);
        }
        else if (canSpawnPlateTut)//tutorial upgrade plate
        {
            Transform cameraCanvas = FindObjectOfType<MainMenuController>().cameraCanvas;
            machinesBtn[1].OnSelectMachine();
            plateMachineUpgradePrefab.Spawn(upgradeBtn.transform.position, cameraCanvas);
            FindObjectOfType<MainMenuController>().setIndexTab(5);
        }
    }
    public bool IsMachineUpgradeAvailable()
    {
        bool result = false;
        var dataController = DataController.Instance;
        foreach (var machineBtn in machinesBtn)
        {
            if (dataController.IsMachineUnlocked(machineBtn.machineId)
            && dataController.GetMachineUpgradeCost(machineBtn.machineId) <= dataController.Gold
            && dataController.GetMachineLevel(machineBtn.machineId) < 3)
            {
                result = true;
                break;
            }
            else
            {
                if (dataController.HasJustCompletedIngredientUpgradeProcess(machineBtn.machineId))
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    public MachineUpgradeController Spawn(Transform parent)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.transform.SetSiblingIndex(parent.childCount - 7);
        return go.GetComponent<MachineUpgradeController>();
    }
    public void HidePanel()
    {
        Destroy(gameObject);
    }
    public void OnSelectMachine(MachineBtnController machineBtn)//load machine detail infomation
    {
        if (currentMachine != null) currentMachine.OnLostFocus();
        currentMachine = machineBtn;
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
        int upgradeCost = DataController.Instance.GetMachineUpgradeCost(currentMachine.machineId);
        string detailSource = "upgrade_machine";
        if (DataController.Instance.Gold < upgradeCost)
        {
            FindObjectOfType<MainMenuController>().ShowUpgradeWithRubyPanel(upgradeCost, UpgradeMachine, detailSource);
            isUpgrading = false;
        }
        else
        {
            DataController.Instance.Gold -= upgradeCost;
            UpgradeMachine();
            APIController.Instance.LogEventSpentGold(upgradeCost, detailSource);
        }
    }
    public void UpgradeMachine()
    {
        isUpgrading = true;
        currentMachine.SetUpgradeTagActive(true);
        var machine = DataController.Instance.GetMachine(currentMachine.machineId);
        int upgradeCost = machine.UpgradePrice();
        upgradeTime = machine.UpgradeTime;
        DataController.Instance.AddMachineUpgradeProcess(currentMachine.machineId);
        if (upgradeTime == 0)
            OnCompleteUpgrade();
        else
        {
            PushNotification.Instance.SendCompleteUpgradeNotification(upgradeTime);
            currentMachine.AutoWaitForUpgrade(upgradeTime);
            StartCoroutine(DelayUpgrade());
        }
        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnUpgradeMachine, new object[] { DataController.Instance.GetMachineLevel(currentMachine.machineId), upgradeCost }));
        DataController.Instance.SaveData();

    }
    private IEnumerator DelayUpgrade()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject go = Instantiate(upgradeEffect, currentMachine.transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
        StartCoroutine(UpdateDetailPanelView());
        isUpgrading = false;
    }
    private IEnumerator UpdateDetailPanelView()
    {
        int machineId = currentMachine.machineId;
        DataController dataController = DataController.Instance;
        detailPanel.SetActive(false);
        costPanel.SetActive(false);
        yield return null;
        var machine = dataController.GetMachine(machineId);

        machineName.text = machine.machineName;
        currentMachineCount.text = machine.MachineCount().ToString();
        if (dataController.GetMachineLevel(machineId) == 3)//disable upgrade button when machine at max level
        {
            costPanel.SetActive(false);
            processPanel.SetActive(false);
            maxPanel.SetActive(true);
            upgradedMachineCount.transform.parent.gameObject.SetActive(false);
            upgradedCookDuration.transform.parent.gameObject.SetActive(false);
            //rewardExp.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            maxPanel.SetActive(false);
            upgradeTime = machine.UpgradeTime;
            upgradeTimeText.gameObject.SetActive(upgradeTime != 0);
            desUpgradeTime.SetActive(upgradeTime != 0);
            var currentUpgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(currentMachine.machineId);

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
                if (duration > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowUpgradeProcess(currentUpgradeProcess, upgradeTime, multiple));
                }

            }
            else
            {
                upgradeTimeText.text = String.Format("{0:D2}:{1:D2}:{2:D2}", upgradeTime / 3600, (upgradeTime / 60) % 60, upgradeTime % 60);
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
            upgradedMachineCount.transform.parent.gameObject.SetActive(true);
            upgradedCookDuration.transform.parent.gameObject.SetActive(true);
            upgradedMachineCount.text = dataController.GetNextLevelMachineCount(machineId).ToString();
            upgradedCookDuration.text = dataController.GetMachineNextLevelWorkTime(machineId).ToString();
            upgradePriceText.text = dataController.GetMachineUpgradeCost(machineId).ToString();
            //rewardExp.text = dataController.GetMachineUpgradeExp(machineId).ToString();
        }
        if (dataController.GetMachineWorkTime(machineId) == 0)
        {
            upgradedCookDuration.transform.parent.gameObject.SetActive(false);
            currentCookDuration.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            currentCookDuration.transform.parent.gameObject.SetActive(true);
            currentCookDuration.text = dataController.GetMachineWorkTime(machineId, true).ToString();
        }
        detailPanel.SetActive(true);

    }
    public Vector2 GetMachineUpgradeBtn()
    {
        int totalMoney = DataController.Instance.Gold + DataController.Instance.Ruby * 20;
        List<int> machineIds = new List<int>();
        int chapterId = DataController.Instance.currentChapter;
        var machineDatas = DataController.Instance.GetRestaurantById(chapterId).machines;
        foreach (var machineData in machineDatas)
            machineIds.Add(machineData.id);
        for (int i = 0; i < machineIds.Count; i++)
            if (DataController.Instance.GetMachineUpgradeCost(machineIds[i]) < totalMoney && DataController.Instance.IsMachineUnlocked(machineIds[i]) && machineDatas[i].level < 3)
                return upgradeBtn.transform.position;
        return Vector2.zero;
    }
    IEnumerator ShowUpgradeProcess(UpgradeMachineProcess currentUpgradeProcess, float neededUpgradeTime, int multiple)
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
            processSlider.value = 1 - (float)(duration / neededUpgradeTime);
            if (duration <= getnowTimebyRuby)
            {
                getNowRubyText.text = "Free";
            }
            else
            {
                getNowRubyText.text = ((duration / conversTimeValue) + 1).ToString();
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
    public void OnCompleteUpgrade()
    {
        currentMachine.OnCompletedUpgrade();
        StopAllCoroutines();
        StartCoroutine(UpdateDetailPanelView());
        isUpgrading = false;
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
            var currentUpgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(currentMachine.machineId);
            currentUpgradeProcess.hasUsedX2Speed = true;
            currentMachine.Init();
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
