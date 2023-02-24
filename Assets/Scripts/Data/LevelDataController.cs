using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelDataController : MonoBehaviour
{
    private static LevelDataController instance = null;
    public static LevelDataController Instance
    {
        get { return instance; }
    }
    [System.NonSerialized] public PlayedLevelData playedLevelData = null;
    [System.NonSerialized] public LevelData currentLevel;
    [System.NonSerialized] public LevelData lastestPassedLevel = null;//if this level == null mean we lose else we can collect the reward
    [System.NonSerialized] public LevelData lastestLoseLevel = null;

    public int collectedGold;
    public int[] itemArray = { 100000, 110000, 120000, 130000, 100100, 100200, 100300, 101000, 102000 };
    public List<int> items = new List<int>();
    private List<LevelData> levelsData;
    public bool IsExtraJob { get; set; }
    public bool IsSockLevel { get; set; }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            IsExtraJob = false;
            currentLevel = null;
            lastestPassedLevel = null;
            levelsData = new List<LevelData>();
            DontDestroyOnLoad(gameObject);
        }
    }
    public int GetItemRandom()
    {
        int random = Random.Range(0, 9);
        return itemArray[random];
    }
    public void AddItem(int id)
    {
        items.Add(id);
    }
    public void RemoveItem(int id)
    {
        items.Remove(id);
    }
    public bool CheckItemActive(int id)
    {
        return items.Contains(id);
    }
    public void ConsumeItems()
    {
        items.Clear();
    }
    public int GetRecommendItemId()
    {
        if (currentLevel.ObjectiveType() == 1 && DataController.Instance.IsItemUnlocked((int)ItemType.Double_Coin))
            return (int)ItemType.Double_Coin;
        if (currentLevel.ConditionType() == 1 && DataController.Instance.IsItemUnlocked((int)ItemType.Anti_OverCook))
            return (int)ItemType.Anti_OverCook;
        return 0;
    }
    public void LoadLevel(int chapter, int levelId)
    {
        LevelData data = null;
        string levelData = FirebaseServiceController.Instance.GetLevelData("level_" + chapter + "_" + levelId);
        data = JsonUtility.FromJson<LevelData>(levelData);
        currentLevel = data;
    }
    public LevelData GetLevelData(int chapter, int levelId)
    {
        foreach (LevelData _data in levelsData)
        {
            if (_data.chapter == chapter && _data.id == levelId)
                return _data;//cache hit
        }
        LevelData data = null;
        string levelData = FirebaseServiceController.Instance.GetLevelData("level_" + chapter + "_" + levelId);
        data = JsonUtility.FromJson<LevelData>(levelData);
        levelsData.Add(data);
        return data;
    }
    public LevelData GetEventLevelData(int chapter, int levelId)
    {
        LevelData data = null;
        string levelData = FirebaseServiceController.Instance.GetEventLevelData("level_" + chapter + "_" + levelId);
        data = JsonUtility.FromJson<LevelData>(levelData);
        return data;
    }
    public LevelData[] LoadAllLevels()
    {
        TextAsset[] datas = Resources.LoadAll<TextAsset>("LevelsData");
        List<LevelData> lvlDatas = new List<LevelData>();
        LevelData lvlData;
        foreach (var data in datas)
        {
            lvlData = JsonUtility.FromJson<LevelData>(data.text);
            lvlDatas.Add(lvlData);
        }
        return lvlDatas.ToArray();
    }
    public LevelData[] LoadAllEventsLevels()
    {
        TextAsset[] datas = Resources.LoadAll<TextAsset>("Events/LevelsData");
        List<LevelData> lvlDatas = new List<LevelData>();
        LevelData lvlData;
        foreach (var data in datas)
        {
            lvlData = JsonUtility.FromJson<LevelData>(data.text);
            lvlDatas.Add(lvlData);
        }
        return lvlDatas.ToArray();
    }
}
[System.Serializable]
public class LevelData
{
    public int chapter = 1;
    public int id = 1;
    public int key = 1;
    public bool IsJustGrantKey()
    {
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {

                return DataController.Instance.GetGameData().levelStates[i].level == key;
            }
        }
        return false;
    }
    public bool IsKeyGranted()
    {
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
                return DataController.Instance.GetGameData().levelStates[i].level >= key;
        }
        return false;
        //return DataController.Instance.GetLevelState(chapter, id) >= key;
    }
    public int[] objectiveType = { 1, 1, 1 };
    public int ObjectiveType()
    {
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return objectiveType[level];
            }
        }
        return objectiveType[0];
    }
    public int[] objective = { 10, 10, 10 };
    public int Objective()
    {
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return objective[level];
            }
        }
        return objective[0];
    }
    public int[] limitType = { 1, 1, 1 };
    public int LimitType()
    {
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return limitType[level];
            }
        }
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        return limitType[0];
    }
    public int[] limit = { 10, 10, 10 };
    public int Limit()
    {

        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return limit[level];
            }
        }
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));

        return limit[0];
    }
    public int[] conditionType = { 1, 1, 1 };
    public int ConditionType()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return conditionType[level];
            }
        }
        return conditionType[0];
    }
    public int[] experiences = { 50, 50, 50 };
    public int GetRewardExperience()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return experiences[level];
            }
        }
        return experiences[0];
    }
    public string[] waves;
    public string GetCurrentWave()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return waves[level];
            }
        }
        return waves[0];
    }
    public string[] foodSpawnPercents;
    public string GetFoodSpawnPercent()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return foodSpawnPercents[level];
            }
        }
        return foodSpawnPercents[0];
    }
    public float[] customerSpeed = { 0, 0, 0 };
    public float GetCustomerSpeed()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return customerSpeed[level];
            }
        }
        return customerSpeed[0];
    }
    public int[] customerIntervals = { 5, 5, 4 };
    public int GetCustomerAppearInterval()
    {
        //int level = Mathf.Min(2, DataController.Instance.GetLevelState(chapter, id));
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                int level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
                return customerIntervals[level];
            }
        }
        return customerIntervals[0];
    }
    public int[] customerWaitTime = { 30, 30, 30 };
    public int GetCustomerWaitTime()
    {
        int level = 0;
        for (int i = 0; i < DataController.Instance.GetGameData().levelStates.Length; i++)
        {
            if (DataController.Instance.GetGameData().levelStates[i].chapterId == chapter && DataController.Instance.GetGameData().levelStates[i].id == id)
            {
                level = Mathf.Min(2, DataController.Instance.GetGameData().levelStates[i].level);
            }
        }
        if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
            return customerWaitTime[level] - 5;
        if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
            return customerWaitTime[level] + 5;
        return customerWaitTime[level];
    }
}
[System.Serializable]
public class ExtraJob
{
    public int chapter;
    public int id;
    public int reward;
}
[System.Serializable]
public class ExtraJobData
{
    public ExtraJob[] ExtraJobs;

    public int GetRewardExtraJob(int chapter, int id)
    {
        for (int i = 0; i < ExtraJobs.Length; i++)
        {
            if (ExtraJobs[i].chapter == chapter && ExtraJobs[i].id == id)
                return ExtraJobs[i].reward;
        }
        return 0;
    }
    public bool IsExtraJob(int chapter, int id)
    {
        for (int i = 0; i < ExtraJobs.Length; i++)
        {
            if (ExtraJobs[i].chapter == chapter && ExtraJobs[i].id == id)
                return true;
        }
        return false;
    }
    public ExtraJob GetExtraJob(int chapter, int id)
    {
        for (int i = 0; i < ExtraJobs.Length; i++)
        {
            if (ExtraJobs[i].chapter == chapter && ExtraJobs[i].id == id)
                return ExtraJobs[i];
        }
        return null;
    }
    public int GetExtraJobId(int chapter)
    {
        for (int i = 0; i < ExtraJobs.Length; i++)
        {
            if (ExtraJobs[i].chapter == chapter)
                return ExtraJobs[i].id;
        }
        return 100;
    }
}
[System.Serializable]
public class PlayedLevelData
{
    public int chapter = 1;
    public int id = 1;
    public int result;
    public int loseType;
    public List<int> UsedBoosterList;

    public PlayedLevelData(int chapter, int id, int result, int loseType, List<int> UsedBoosterList = null)
    {
        this.chapter = chapter;
        this.id = id;
        this.result = result;
        this.loseType = loseType;
        this.UsedBoosterList = UsedBoosterList;
    }
}

