using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
public class QuestFloatUpNotification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tittle, description, progressText;
    [SerializeField] private float originProgressWidth;
    [SerializeField] private Image progressBar, tickImage;
    public void HideFloatNotification()
    {
        gameObject.SetActive(false);
    }
    public void Init(Quest quest)
    {
        tittle.text = quest.Name;
        description.text = quest.Description;
        progressText.text = quest.tmpCount + "/" + quest.rewardThreshold;
        progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(originProgressWidth * quest.tmpCount * 1f / quest.rewardThreshold, progressBar.
        GetComponent<RectTransform>().sizeDelta.y);
        tickImage.gameObject.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine(DoQuestAnimation(quest));
    }
    private IEnumerator DoQuestAnimation(Quest quest)
    {
        GetComponent<Animator>().Play("Appear");
        yield return new WaitForSeconds(0.5f);
        var progressBarRect = progressBar.GetComponent<RectTransform>();
        System.Action<ITween<Vector3>> updateProgressBar = (t) =>
                            {
                                if (progressBarRect != null)
                                    progressBarRect.sizeDelta = t.CurrentValue;
                                if (progressText != null)
                                    progressText.text = (int)(t.CurrentValue.x * quest.rewardThreshold / originProgressWidth) + "/" + quest.rewardThreshold;
                            };
        TweenFactory.Tween("quest_progress" + Time.time, progressBarRect.sizeDelta, new Vector2(originProgressWidth * quest.count / quest.rewardThreshold, progressBarRect.sizeDelta.y), 0.75f, TweenScaleFunctions.QuinticEaseOut, updateProgressBar);
        yield return new WaitForSeconds(0.75f);
        tickImage.gameObject.SetActive(quest.count >= quest.rewardThreshold);
        progressText.text = quest.count + "/" + quest.rewardThreshold;
        yield return new WaitForSeconds(0.25f);
        GetComponent<Animator>().Play("Disappear");
    }
}
