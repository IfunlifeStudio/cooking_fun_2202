using DigitalRuby.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalsTarget : MonoBehaviour
{
    [SerializeField] private Image objectiveIcon, conditionIcon;
    [SerializeField] private TextMeshProUGUI objectiveText, conditionText;
    [SerializeField] private Sprite[] objectiveSprites, limitSprites, conditionSprites;
    [SerializeField] private GameObject blankLayer;
    private LevelData levelData;
    private Animator anim;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        levelData = LevelDataController.Instance.currentLevel;
        Time.timeScale = 0;
        StartCoroutine(CanBackBasePanel());
        objectiveIcon.sprite = objectiveSprites[levelData.ObjectiveType() - 1];
        if (levelData.ObjectiveType() == 1)
            objectiveText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("object_target_gold"), levelData.Objective());
        else if (levelData.ObjectiveType() == 2)
            objectiveText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("object_target_heart"), levelData.Objective());
        else if (levelData.ObjectiveType() == 3)
            objectiveText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("object_target_dish"), levelData.Objective());
        if (levelData.ConditionType() > 0)
        {
            conditionIcon.gameObject.SetActive(true);
            conditionIcon.sprite = conditionSprites[levelData.ConditionType() - 1];
            if (levelData.ConditionType() == 1)
                conditionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tut_burned_condidtion");
            else if (levelData.ConditionType() == 2)
                conditionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tut_throw_condidtion");
            else if (levelData.ConditionType() == 3)
                conditionText.text = Lean.Localization.LeanLocalization.GetTranslationText("tut_cus_leave_condition");
        }
        else
        {
            conditionIcon.enabled = false;
            conditionText.enabled = false;
            objectiveIcon.transform.position = new Vector2(objectiveIcon.transform.position.x + 0.65f, objectiveIcon.transform.position.y);
            objectiveText.transform.position = new Vector2(objectiveText.transform.position.x + 0.65f, objectiveText.transform.position.y);
        }
        anim = gameObject.GetComponent<Animator>();
    }
    IEnumerator CanBackBasePanel()
    {
        UIController.Instance.CanBack = false;

        yield return new WaitForSecondsRealtime(2.5f);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1.5f);
        UIController.Instance.CanBack = true;
        MessageManager.Instance.SendMessage(
           new Message(CookingMessageType.OnGameStart,
           new object[] { levelData.ObjectiveType(), levelData.LimitType(), levelData.ConditionType(), levelData.chapter, levelData.id }));
        Destroy(gameObject);
    }
}
