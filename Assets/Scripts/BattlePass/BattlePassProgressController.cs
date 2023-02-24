using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassProgressController : MonoBehaviour
{
    [SerializeField] private Image progressImg;
    [SerializeField] GameObject maxLevelGo;
    [SerializeField] private TextMeshProUGUI progressTxt, nextLevelTxt;
    [SerializeField] private Animator sockAnimator;
    public void HandleData(BattlePassElementData nextElementData, int currentLevel, int currentExp)
    {
        //progressSlider.value = currentExp / nextElementData.Exp;
        float progressValue = currentExp * 1f / nextElementData.Exp;
        string progressStr = currentExp + "/" + nextElementData.Exp;
        string nextLevelStr = nextElementData.Level.ToString();
        if (currentLevel == 30 && nextElementData.Level == 30)
        {
            progressValue = 1;
            progressStr = "Max";
            maxLevelGo.SetActive(true);
            nextLevelTxt.gameObject.SetActive(false);
        }
        FillProgress(progressValue);
        progressTxt.text = progressStr;
        nextLevelTxt.text = nextLevelStr;
        if (currentExp >= nextElementData.Exp)
            sockAnimator.Play("shake");
        else
            sockAnimator.Play("idle");
    }
    public void FillProgress(float value)
    {
        float currentValue = progressImg.fillAmount;
        System.Action<ITween<float>> updateFoodProp = (t) =>
        {
            progressImg.fillAmount = t.CurrentValue;
        };

        System.Action<ITween<float>> completedPropMovement = (t) =>
        {
            progressImg.fillAmount = value;
        };
        TweenFactory.Tween("FillProgress" + Random.Range(0, 100f) + Time.time, currentValue, value, 1f, TweenScaleFunctions.Linear, updateFoodProp, completedPropMovement);
    }
}
