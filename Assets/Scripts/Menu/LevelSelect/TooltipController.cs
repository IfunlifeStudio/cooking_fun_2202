using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TooltipController : MonoBehaviour
{
    [SerializeField] private GameObject tipPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private bool isTipOpen;
    private void Update()
    {
        if (isTipOpen)
            if (Input.GetMouseButtonDown(0))
            {
                tipPanel.SetActive(false);
                isTipOpen = false;
            }
    }
    public void OnClickNormalTip()
    {
        tipPanel.SetActive(true);
        isTipOpen = true;
    }
    public void InitWithMessage(string message)
    {
        tipPanel.SetActive(true);
        isTipOpen = true;
        descriptionText.text = message;
    }
    public void OnClickObjectiveTips()//this function is made only for the objective icon
    {
        tipPanel.SetActive(true);
        LevelData levelData = LevelDataController.Instance.currentLevel;
        isTipOpen = true;
        switch (levelData.ObjectiveType())
        {
            case 1:
                descriptionText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("tooltip_earn_coin"), levelData.Objective());
                break;
            case 2:
                descriptionText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("tooltip_earn_like"), levelData.Objective());
                break;
            case 3:
                descriptionText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("tooltip_earn_dish"), levelData.Objective());
                break;
        }
    }
    public void OnClickLimitTips()//this function is made only for the limit icon
    {
        LevelData levelData = LevelDataController.Instance.currentLevel;
        isTipOpen = true;
        switch (levelData.LimitType())
        {
            case 1:
                descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tooltip_time_limit");
                break;
            case 2:
                descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tooltip_cus_limit");
                break;
        }
        tipPanel.SetActive(true);
    }
    public void OnClickConditionTips()//this function is made only for the condition icon
    {
        LevelData levelData = LevelDataController.Instance.currentLevel;
        isTipOpen = true;
        switch (levelData.ConditionType())
        {
            case 1:
                descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tooltip_burn_limit");
                break;
            case 2:
                descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tooltip_throw_limit");
                break;
            case 3:
                descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tooltip_leave_limit");
                break;
        }
        tipPanel.SetActive(true);
    }
    public void OnClickBorderTips()
    {
        tipPanel.SetActive(true);
        isTipOpen = true;
        descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText("unlock_border");
    }
}
