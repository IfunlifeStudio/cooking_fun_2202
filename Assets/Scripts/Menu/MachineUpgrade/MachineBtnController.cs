using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System;

public class MachineBtnController : MonoBehaviour
{
    public int machineId;
    [SerializeField] private Image machineImage;
    [SerializeField] private SkeletonGraphic mysteryBox;
    [SerializeField] private Sprite[] machineSprites;
    [SerializeField] private Image[] levels;
    [SerializeField] private GameObject focusFrame, recommendTag, upgradingTag;
    private bool isMachineUnlocked;
    [SerializeField] private GameObject openBoxEffectPrefab;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private AudioClip rewardAudio;
    public bool IsShowProcess;
    private int level;
    public void Init()
    {
        isMachineUnlocked = DataController.Instance.IsMachineUnlocked(machineId);
        bool isRecommend = DataController.Instance.GetRecommendMachineUpgradeId() == machineId;
        recommendTag.SetActive(isRecommend && isMachineUnlocked);
        if (isMachineUnlocked)
        {
            if (PlayerPrefs.GetInt("box_open" + machineId, 0) == 0 && mysteryBox != null)
                StartCoroutine(PlayOpenMysteryBox());
            else
            {
                level = DataController.Instance.GetMachineLevel(machineId);
                for (int i = 0; i < levels.Length; i++)
                    levels[i].gameObject.SetActive(i < level);
                machineImage.sprite = machineSprites[level - 1];
                IsUpgradeProcessing();
            }

        }
        else
        {
            mysteryBox.gameObject.SetActive(true);
            machineImage.gameObject.SetActive(false);
            levels[0].transform.parent.gameObject.SetActive(false);
        }
    }
    private IEnumerator PlayOpenMysteryBox()
    {
        mysteryBox.gameObject.SetActive(true);
        machineImage.gameObject.SetActive(false);
        levels[0].transform.parent.gameObject.SetActive(false);
        mysteryBox.AnimationState.SetAnimation(0, "click_open", false);
        GameObject openBoxEffect = Instantiate(openBoxEffectPrefab, transform.position - Vector3.forward, Quaternion.identity, transform.parent);
        Destroy(openBoxEffect, 2);
        yield return new WaitForSeconds(0.7f);
        machineImage.gameObject.SetActive(true);
        levels[0].transform.parent.gameObject.SetActive(true);
        level = DataController.Instance.GetMachineLevel(machineId);
        UpdateMachineSprite(level);
        PlayerPrefs.SetInt("box_open" + machineId, 1);
    }
    public void OnSelectMachine()
    {
        if (isMachineUnlocked)
        {
            GetComponent<Animator>().Play("Pressed");
            FindObjectOfType<MachineUpgradeController>().OnSelectMachine(this);
            focusFrame.SetActive(true);
            IsShowProcess = true;
        }
        else
        {
            mysteryBox.AnimationState.SetAnimation(0, "idle", false);
            FindObjectOfType<MainMenuController>().ShowMessagePanel(Lean.Localization.LeanLocalization.GetTranslationText("unlock_machin_panel_message", "Continue playing to unlock this machine"));
        }
    }
    public void OnLostFocus()
    {
        IsShowProcess = false;
        focusFrame.SetActive(false);
    }
    public void OnCompletedUpgrade()
    {
        if (rewardAudio != null)
            AudioController.Instance.PlaySfx(rewardAudio);
        DataController.Instance.CompletedMachineUpgrade(machineId, true, false);
        level = DataController.Instance.GetMachineLevel(machineId);
        SetUpgradeTagActive(false);
        DataController.Instance.SaveData(false);
        StopAllCoroutines();
        StartCoroutine(DelayUpgradeToLeve(level));
    }
    IEnumerator DelayUpgradeToLeve(int level)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponent<Animator>().Play("Pressed");
        UpdateMachineSprite(level);
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
    public void AutoWaitForUpgrade(float time)
    {
        StopAllCoroutines();
        StartCoroutine(DelayUpgradeWithTime(time));
    }
    IEnumerator DelayUpgradeWithTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        if (!IsShowProcess)
        {
            DataController.Instance.CompletedMachineUpgrade(machineId, true, false);
            SetUpgradeTagActive(false);
            DataController.Instance.SaveData(false);
            GetComponent<Animator>().Play("Pressed");
            level = DataController.Instance.GetMachineLevel(machineId);
            UpdateMachineSprite(level);
            GameObject go = Instantiate(upgradeEffect, transform.position, Quaternion.identity);
            Destroy(go, 1);
        }
    }
    public void UpdateMachineSprite(int level)
    {
        if (level > 3) level = 3;
        for (int i = 0; i < levels.Length; i++)
            levels[i].gameObject.SetActive(i < level);
        machineImage.sprite = machineSprites[level - 1];
    }
    public bool IsUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(machineId);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(machineId);
            if (upgradeProcess.hasCompleteUpgradeInGame)
            {
                DataController.Instance.RemoveMachineUpgradeProcess(machineId);
                UpdateMachineSprite(level - 1);
                StopAllCoroutines();
                StartCoroutine(DelayUpgradeToLeve(level));
                return true;
            }
            else if (upgradeProcess.timeStamp != 0)
            {
                int multiple = upgradeProcess.hasUsedX2Speed ? 2 : 1;
                float passedTime = (float)(multiple * (DataController.ConvertToUnixTime(DateTime.UtcNow) - upgradeProcess.timeStamp));
                if (passedTime > neededUpgradeTime)
                {
                    OnCompletedUpgrade();
                    SetUpgradeTagActive(false);
                    return true;
                }
                else if (passedTime < neededUpgradeTime)
                {
                    SetUpgradeTagActive(true);
                    IsShowProcess = false;
                    AutoWaitForUpgrade((neededUpgradeTime - passedTime) / multiple);
                }
            }

        }
        return false;
    }
    public void SetUpgradeTagActive(bool status)
    {
        recommendTag.SetActive(false);
        upgradingTag.SetActive(status);
    }
}
