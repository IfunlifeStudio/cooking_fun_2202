using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;
public class QuestPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questTittle, countDownText, questProgressText;
    [SerializeField] private GameObject claimBtn, tick, guideText;
    [SerializeField] private Transform progressMask;
    [SerializeField] private SkeletonGraphic giftBox;
    private float timeStamp, questTimeStamp, maskOriginWidth;
    void Start()
    {
        questTimeStamp = (float)PlayerPrefs.GetFloat(QuestController.QUEST_TIMESTAMP, 0);
        GetComponent<Animator>().Play("Appear");
        maskOriginWidth = progressMask.GetComponent<RectTransform>().sizeDelta.x;
        questTittle.text = Lean.Localization.LeanLocalization.GetTranslationText("daily_tittle_mission_end", "MISSION END IN");
        UpdatePanelDisplay();
    }
    void Update()
    {
        if (Time.time - timeStamp > 1)
        {
            timeStamp = Time.time;
            float deltaTime = (float)(questTimeStamp + 43200 - DataController.ConvertToUnixTime(System.DateTime.UtcNow));
            countDownText.text = DataController.GetHours(deltaTime) + "h " + DataController.GetMinutes(deltaTime) + "m " + DataController.GetSeconds(deltaTime) + "s";
        }
    }
    public void UpdatePanelDisplay()
    {
        claimBtn.SetActive(false);
        tick.SetActive(false);
        guideText.SetActive(false);
        if (QuestController.Instance.HasUncompletedQuest() || QuestController.Instance.HasClaimableQuest())
        {
            guideText.SetActive(true);
            giftBox.AnimationState.SetAnimation(0, "idle_click", true);
        }
        else
        {
            if (!QuestController.Instance.ClaimState)
                claimBtn.SetActive(true);
            else
            {
                questTittle.text = Lean.Localization.LeanLocalization.GetTranslationText("daily_tittle_mission_next", "NEXT MISSION IN");
                tick.SetActive(true);
                giftBox.AnimationState.SetAnimation(0, "open", true);
            }
        }
        int totalQuestCompleted = 0;
        foreach (var quest in QuestController.Instance.activeQuests)
            if (quest.IsCompleted() && quest.isClaimed)
                totalQuestCompleted++;
        questProgressText.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("achi_complete", "Completed") + " {0}/{1}", totalQuestCompleted, 3);
        progressMask.GetComponent<RectTransform>().sizeDelta = new Vector2(maskOriginWidth * totalQuestCompleted / 3f, progressMask.GetComponent<RectTransform>().sizeDelta.y);
    }
    public void OnClickClaim()
    {
        QuestController.Instance.ClaimState = true;
        FindObjectOfType<RewardPanelController>().Init(0, 0, 30, new int[1] { DataController.Instance.GetRandomItemId() }, new int[1] { 1 });
        StartCoroutine(DelayClose());
    }
    public void OnClickClose()
    {
        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
