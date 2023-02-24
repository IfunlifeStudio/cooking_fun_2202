using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestMenuController : MonoBehaviour
{
    [SerializeField] private GameObject newQuestNoti, questCompletedNoti, questBtn, questPanelPrefab, questIconTutorialPrefab;
    [SerializeField] private Transform cameraCanvas;
    [SerializeField] private QuestFloatUpNotification floatUpNotification;
    private float clickTimeStamp;
    void Start()
    {
        DisplayQuestIcon();
        QuestController.Instance.InitiateQuest();//handle player dont close detail panel after finish level 2.1
        StartCoroutine(DisplayFloatNotification());
    }
    private IEnumerator DisplayFloatNotification()
    {
        yield return null;
        foreach (Quest quest in QuestController.Instance.activeQuests)
        {
            if (quest.isClaimed || quest.tmpCount == quest.count) continue;
            floatUpNotification.Init(quest);
            yield return new WaitForSeconds(2.25f);
            quest.tmpCount = quest.count;
        }
        floatUpNotification.HideFloatNotification();
    }
    public void DisplayQuestIcon()
    {
        questBtn.SetActive(QuestController.IsQuestUnlocked());
        newQuestNoti.SetActive(QuestController.Instance.newQuestActived);
        questCompletedNoti.SetActive(QuestController.Instance.HasClaimableQuest());
    }
    public void DisplayQuestTutorial()
    {
        if (QuestController.IsQuestUnlocked() && !DataController.Instance.GetTutorialData().Contains(7))
        {
            DataController.Instance.GetTutorialData().Add(7);
            DataController.Instance.SaveData();
            StartCoroutine(DelaySpawnQuest());
        }
    }
    private IEnumerator DelaySpawnQuest()
    {
        yield return new WaitForSeconds(1f);
        GameObject go = Instantiate(questIconTutorialPrefab);
        Button btn = go.GetComponentInChildren<Button>();
        btn.onClick.AddListener(OnClickQuestBtn);
        btn.onClick.AddListener(() => { Destroy(go); });
    }
    public void OnClickQuestBtn()
    {
        QuestController.Instance.newQuestActived = false;
        newQuestNoti.SetActive(false);
        questCompletedNoti.SetActive(false);
        if (Time.time - clickTimeStamp < 0.5f) return;
        else
            clickTimeStamp = Time.time;
        Instantiate(questPanelPrefab, cameraCanvas);
    }
}
