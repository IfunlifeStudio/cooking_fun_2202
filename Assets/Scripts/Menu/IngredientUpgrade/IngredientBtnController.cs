using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;
using System;

public class IngredientBtnController : MonoBehaviour
{
    public int ingredientId;
    [SerializeField] private Image igredientImage;
    [SerializeField] private SkeletonGraphic mysteryBox;
    [SerializeField] private Sprite[] ingredientSprites;
    [SerializeField] private Image[] levels;
    [SerializeField] private GameObject focusFrame, recommendTag, upgradingTag;
    [SerializeField] private GameObject openBoxEffectPrefab;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private bool isShowUpgradeInGame = true;
    [SerializeField] private AudioClip rewardAudio;
    public bool IsShowProcess { get; set; }
    private bool isIngredientUnlocked;
    private int level;
    public void Init()
    {
        isIngredientUnlocked = DataController.Instance.IsIngredientUnlocked(ingredientId);
        bool isRecommend = DataController.Instance.GetRecommendIngredientUpgradeId() == ingredientId;
        recommendTag.SetActive(isRecommend && isIngredientUnlocked);
        if (isIngredientUnlocked)
        {
            if (PlayerPrefs.GetInt("box_open" + ingredientId, 0) == 0 && mysteryBox != null)
            {
                StartCoroutine(PlayOpenMysteryBox());
            }
            else
            {
                level = DataController.Instance.GetIngredientLevel(ingredientId);
                for (int i = 0; i < levels.Length; i++)
                    levels[i].gameObject.SetActive(i < level);
                igredientImage.sprite = ingredientSprites[level - 1];
                IsUpgradeProcessing();
            }
        }
        else
        {
            mysteryBox.gameObject.SetActive(true);
            igredientImage.gameObject.SetActive(false);
            levels[0].transform.parent.gameObject.SetActive(false);
        }


    }
    public bool IsUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeIngredientProcessing(ingredientId);
        if (upgradeProcess != null)
        {

            float neededUpgradeTime = DataController.Instance.GetIngredientUpgradeTime(ingredientId);
            if (upgradeProcess.hasCompleteUpgradeInGame)
            {
                DataController.Instance.RemoveIngredientUpgradeProcess(ingredientId);
                UpdateIngredientSprite(level - 1);
                StopAllCoroutines();
                StartCoroutine(DelayUpgradeToLevel(level));
                SetUpgradeTagActive(false);
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
    public void UpdateIngredientSprite(int level)
    {
        if (level > 3) level = 3;
        for (int i = 0; i < levels.Length; i++)
            levels[i].gameObject.SetActive(i < level);
        igredientImage.sprite = ingredientSprites[level - 1];
    }
    private IEnumerator PlayOpenMysteryBox()
    {
        mysteryBox.gameObject.SetActive(true);
        igredientImage.gameObject.SetActive(false);
        levels[0].transform.parent.gameObject.SetActive(false);
        mysteryBox.AnimationState.SetAnimation(0, "click_open", false);
        GameObject openBoxEffect = Instantiate(openBoxEffectPrefab, transform.position - Vector3.forward, Quaternion.identity, transform.parent);
        Destroy(openBoxEffect, 2);
        yield return new WaitForSeconds(0.7f);
        igredientImage.gameObject.SetActive(true);
        levels[0].transform.parent.gameObject.SetActive(true);
        level = DataController.Instance.GetIngredientLevel(ingredientId);
        UpdateIngredientSprite(level);
        PlayerPrefs.SetInt("box_open" + ingredientId, 1);
    }
    public void OnSelectIngredient()
    {
        if (isIngredientUnlocked)
        {
            GetComponent<Animator>().Play("Pressed");
            FindObjectOfType<IngredientUpgradeController>().OnSelectIngredient(this);
            focusFrame.SetActive(true);
            IsShowProcess = true;
        }
        else
        {
            mysteryBox.AnimationState.SetAnimation(0, "idle", false);
            FindObjectOfType<MainMenuController>().ShowMessagePanel(Lean.Localization.LeanLocalization.GetTranslationText("unlock_ingredient_panel_message", "Continue playing to unlock this ingredient"));
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
        DataController.Instance.CompletedIngredientUpgrade(ingredientId);
        if (isShowUpgradeInGame)
            DataController.Instance.HasCompletedIngredientUpgradeInMenu(ingredientId);
        else
            DataController.Instance.RemoveIngredientUpgradeProcess(ingredientId);
        level = DataController.Instance.GetIngredientLevel(ingredientId);
        SetUpgradeTagActive(false);
        DataController.Instance.SaveData(false);
        StopAllCoroutines();
        StartCoroutine(DelayUpgradeToLevel(level));
    }
    IEnumerator DelayUpgradeToLevel(int level)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetComponent<Animator>().Play("Pressed");
        UpdateIngredientSprite(level);
        GameObject go = Instantiate(upgradeEffect, transform.position, Quaternion.identity);
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
            DataController.Instance.CompletedIngredientUpgrade(ingredientId);
            if (isShowUpgradeInGame)
                DataController.Instance.HasCompletedIngredientUpgradeInMenu(ingredientId);
            else
                DataController.Instance.RemoveIngredientUpgradeProcess(ingredientId);
            SetUpgradeTagActive(false);
            DataController.Instance.SaveData(false);
            level = DataController.Instance.GetIngredientLevel(ingredientId);
            GetComponent<Animator>().Play("Pressed");
            UpdateIngredientSprite(level);
            GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
            Destroy(go, 1);
        }
    }
    public void SetUpgradeTagActive(bool status)
    {
        recommendTag.SetActive(false);
        upgradingTag.SetActive(status);
    }
}
