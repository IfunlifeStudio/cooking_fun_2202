using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class QuestController : MonoBehaviour
{
    public const string SAVE_FOLDER = "/GameEvent/Quests/";
    public const string QUEST_TIMESTAMP = "quest_timestamp", REWARD_STATE = "quest_reward_state";
    private static QuestController instance = null;
    public static QuestController Instance
    {
        get { return instance; }
    }
    public List<Quest> registerQuests = new List<Quest>(), activeQuests = new List<Quest>();
    public bool newQuestActived = false;
    private bool hasInit = false;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    void OnDestroy()
    {
        SaveToJson();
    }
    public void InitiateQuest()
    {
        if (IsQuestUnlocked())
        {
            if (hasInit) return;
            hasInit = true;
            StartCoroutine(_InitiateQuest());
        }
    }
    private IEnumerator _InitiateQuest()
    {
        int activeCount = 0;
        foreach (Quest quest in registerQuests)
        {
            if (quest.isActivated)
                activeCount++;
        }
        if (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(QUEST_TIMESTAMP, 0) > 43200 || activeCount < 3)//refresh quest
        {
            newQuestActived = true;
            ClaimState = false;
            AdsController.Instance.ResetAdsCount("quest_ads_1");
            AdsController.Instance.ResetAdsCount("quest_ads_2");
            AdsController.Instance.ResetAdsCount("quest_ads_3");
            PlayerPrefs.SetFloat(QUEST_TIMESTAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
            List<Quest> newQuests = new List<Quest>();//load new quest
            Quest quest = null;
            int questTier = 1;
            do
            {
                quest = registerQuests[UnityEngine.Random.Range(0, registerQuests.Count)];
                if (!quest.MeetUnlockRequirement() || quest.tier != questTier) continue;
                bool isExist = false;
                for (int i = 0; i < newQuests.Count; i++)
                {
                    if (newQuests[i].id == quest.id)
                        isExist = true;
                }
                if (!isExist)
                {
                    newQuests.Add(quest);
                    questTier++;
                }
            }
            while (newQuests.Count < 3);
            foreach (Quest _quest in registerQuests)
            {
                _quest.Deactive();
                yield return null;
            }
            foreach (Quest _quest in newQuests)
            {
                _quest.Active();
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.25f);
        LoadFromJson();
        activeQuests.Clear();
        foreach (Quest quest in registerQuests)
        {
            if (quest.isActivated)
                activeQuests.Add(quest);
        }
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isClaimed)
                quest.Register();
        }
    }
    public void SaveToJson()
    {
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER;
        if (!System.IO.File.Exists(savePath))
            System.IO.Directory.CreateDirectory(savePath);
        foreach (Quest ge in registerQuests)
            ge.SaveToJson();
    }
    private void LoadFromJson()
    {
        foreach (Quest ge in registerQuests)
            ge.LoadFromJson();
    }
    public bool HasClaimableQuest()
    {
        bool hasClaimable = false;
        foreach (Quest quest in activeQuests)
        {
            if (quest.IsCompleted() && !quest.isClaimed)
                hasClaimable = true;
        }
        return hasClaimable;
    }
    public bool HasUncompletedQuest()
    {
        bool hasUncompleted = false;
        foreach (Quest quest in activeQuests)
        {
            if (!quest.IsCompleted() && !quest.isClaimed)
                hasUncompleted = true;
        }
        return hasUncompleted;
    }
    public void ClaimQuest(int index)
    {
        activeQuests[index].isClaimed = true;
        activeQuests[index].SaveToJson();
    }
    public void RenewQuest(int index)
    {
        Quest activeQuest = activeQuests[index];
        int questTier = activeQuest.tier;
        Quest quest = null;
        do
        {
            quest = registerQuests[UnityEngine.Random.Range(0, registerQuests.Count)];
        }
        while (!quest.MeetUnlockRequirement() || quest.tier != activeQuest.tier || quest.id == activeQuest.id);
        activeQuest.Deactive();
        quest.Active();
        quest.Register();
        activeQuests[index] = quest;
    }
    public static bool IsQuestUnlocked()
    {
        return DataController.Instance.GetLevelState(2, 1) > 0;
    }
    public bool ClaimState
    {
        get { return PlayerPrefs.GetInt(REWARD_STATE, 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(REWARD_STATE, value ? 1 : 0);
        }
    }
}
