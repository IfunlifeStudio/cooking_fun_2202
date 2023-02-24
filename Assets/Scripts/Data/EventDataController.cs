using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;
public class EventDataController : MonoBehaviour
{
    public const string EVENT_TIME_STAMP = "event_time_stamp", EVENT_TIME_STAMP_2 = "event_time_stamp_2";
    private static EventDataController instance;
    public static EventDataController Instance { get { return instance; } }
    [HideInInspector] public int currentChapter = -1;
    [HideInInspector] public UnityEvent onDataChange;
    [SerializeField] private GameData gameData;
    [SerializeField] private RestaurantData[] eventRestaurantDatas;
    private ItemsData itemsData = new ItemsData();
    private string dataPath = "";
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            dataPath = Path.Combine(Application.persistentDataPath, "event_data.dat");//check and reset event data after 7 days
            float eventTimeStamp = PlayerPrefs.GetFloat(EventDataController.EVENT_TIME_STAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
            float deltaTime = (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow) - eventTimeStamp;
            if (deltaTime > 7 * 24 * 60 * 60 || deltaTime < 0)
            {
                int cycle = (int)(deltaTime / 604800);
                eventTimeStamp += cycle * 604800;
                PlayerPrefs.SetFloat(EventDataController.EVENT_TIME_STAMP, eventTimeStamp);
                ResetData();
            }
            eventTimeStamp = PlayerPrefs.GetFloat(EventDataController.EVENT_TIME_STAMP_2, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
            deltaTime = (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow) - eventTimeStamp;
            if (deltaTime > 7 * 24 * 60 * 60 || deltaTime < 0)
            {
                int cycle = (int)(deltaTime / 604800);
                eventTimeStamp += cycle * 604800;
                PlayerPrefs.SetFloat(EventDataController.EVENT_TIME_STAMP_2, eventTimeStamp);
                ResetData();
            }
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void LoadData()
    {
        string resData = null;//read all config from Resources
        List<RestaurantData> resDatas = new List<RestaurantData>();
        int index = 50;
        do
        {
            index++;
            resData = FirebaseServiceController.Instance.GetEventRestaurantData("res_config_" + index);
            if (resData != null && resData.Length > 10)
            {
                var tmp = JsonUtility.FromJson<RestaurantData>(resData);
                resDatas.Add(tmp);
            }
        }
        while (resData != null && resData.Length > 10);
        eventRestaurantDatas = resDatas.ToArray();
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string data;
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                try
                {
                    data = (string)binaryFormatter.Deserialize(fileStream);
                    gameData = JsonUtility.FromJson<GameData>(data);
                    //LevelData[] eventsLevelsData = LevelDataController.Instance.LoadAllEventsLevels();
                    gameData.UpdateGameData(itemsData);//dont use the item data in here
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    ResetData();
                }
            }
        }
        else
            ResetData();
    }
    private void ResetData()
    {
        PlayerPrefs.SetInt("event_res_reward_progress_" + 51, -1);
        PlayerPrefs.SetInt("event_res_reward_progress_" + 52, -1);
        //LevelData[] eventsLevelsData = LevelDataController.Instance.LoadAllEventsLevels();//dont use the item data in here
        gameData = new GameData(itemsData);
        SaveData();
    }
    public void SaveData()
    {
        string origin = JsonUtility.ToJson(gameData);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, origin);
        }
    }
    public int GetFoodValue(Food food)
    {
        int foodValue = 0;
        foreach (int ingredient in food.IngredientIds)
            foodValue += GetIngredientValue(ingredient);
        return foodValue;
    }
    public int[] GetLevelProgressUnlock(int chapter)
    {
        return GetRestaurantById(chapter).levelProgressLock;
    }
    public int GetTotalLevelsPerRestaurant(int chapter)
    {
        var currentRestaurant = GetRestaurantById(chapter);
        return currentRestaurant.levelProgressLock[currentRestaurant.levelProgressLock.Length - 1];
    }
    #region Ingredient Helper Function
    public bool IsIngredientUnlocked(int ingredientId, int currentLevel = -1)
    {
        IngredientData ingredientData = GetRestaurantById(currentChapter).GetIngredient(ingredientId);
        int nextLevel = GetNextLevel(currentChapter);
        int highestLevel = GetHighestLevel(currentChapter);
        if (highestLevel >= nextLevel && highestLevel > 1) highestLevel = GetTotalLevelsPerRestaurant(currentChapter) + 1;
        if (highestLevel < currentLevel) highestLevel = currentLevel;
        return ingredientData.IsUnlocked(highestLevel);
    }
    public string GetIngredientName(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetIngredientName(ingredientId);
    }
    public int GetIngredientValue(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetCurrentIngredientValue(ingredientId);
    }
    public int GetIngredientLevel(int ingredientId)
    {
        return gameData.GetIngredientState(ingredientId);
    }
    #endregion
    #region Machine Helper Function
    public bool IsMachineUnlocked(int machineId, int currentLevel = -1)
    {
        MachineData machineData = GetRestaurantById(currentChapter).GetMachine(machineId);
        int nextLevel = GetNextLevel(currentChapter);
        int highestLevel = GetHighestLevel(currentChapter);
        if (highestLevel >= nextLevel && highestLevel > 1) highestLevel = GetTotalLevelsPerRestaurant(currentChapter) + 1;
        if (highestLevel < currentLevel) highestLevel = currentLevel;
        return machineData.IsUnlocked(highestLevel);
    }
    public string GetMachineName(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineName(machineId);
    }
    public float GetMachineWorkTime(int machineId, bool ignoreNoobMode = false)
    {
        return GetRestaurantById(currentChapter).GetWorkTime(machineId);
    }
    public int GetMachineLevel(int machineId)
    {
        return gameData.GetMachineState(machineId);
    }
    public void SetMachineLevel(int machineId, int level)
    {
        gameData.SetMachineState(machineId, level);
        SaveData();
    }
    public int GetMachineCount(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineCount(machineId);
    }
    #endregion
    #region Res Helper Function
    public RestaurantData GetRestaurantById(int id)
    {
        foreach (var res in eventRestaurantDatas)
            if (res.id == id)
                return res;
        return null;
    }
    public int GetTotalKeyGranted()
    {
        int result = 0;
        int totalRestaurant = eventRestaurantDatas.Length;
        for (int i = 0; i < totalRestaurant; i++)
            result += GetRestaurantKeyGranted(eventRestaurantDatas[i].id);
        return result;
    }
    public int GetRestaurantKeyGranted(int chapterId)
    {
        int result = 0;
        int totalLevel = GetTotalLevelsPerRestaurant(chapterId);
        LevelData levelData = null;
        for (int i = 1; i < totalLevel + 1; i++)
        {
            levelData = LevelDataController.Instance.GetEventLevelData(chapterId, i);
            if (GetLevelState(chapterId, i) >= levelData.key)
                result++;
        }
        return result;
    }
    public bool HasRestaurantRewarded(int chapterId, int _progress)
    {
        //PlayerPrefs.SetInt("event_res_reward_progress_" + chapterId, -1);
        int progress = PlayerPrefs.GetInt("event_res_reward_progress_" + chapterId, -1);
        return progress == _progress;
    }
    public void SetRestaurantRewarded(int chapterId, int progress)
    {
        PlayerPrefs.SetInt("event_res_reward_progress_" + chapterId, progress);
    }
    #endregion
    #region Level Helper Function
    public string GetGamePlayScene(int chapter)
    {
        var res = GetRestaurantById(chapter);
        if (res != null) return res.sceneName;
        return null;
    }
    public int GetLevelState(int chapter, int levelId)
    {
        return gameData.GetLevelState(chapter, levelId);
    }
    public void SetLevelState(int chapter, int levelId, int value)
    {
        //gameData.SetLevelState(chapter, levelId, value);
        SaveData();
    }
    public int GetHighestLevel(int chapter)//get the highest level played
    {
        int totalLevel = GetTotalLevelsPerRestaurant(chapter);
        int result = 1;
        for (int i = totalLevel; i > 0; i--)
            if (GetLevelState(chapter, i) > 0)
            {
                result = i;
                break;
            }
        return result;
    }
    public int GetNextLevel(int chapter)//get the next level which have not been unlocked
    {
        int totalLevel = GetTotalLevelsPerRestaurant(chapter);
        int level = GetLevelState(chapter, totalLevel);
        int index = 1;
        for (index = 1; index < totalLevel + 1; index++)
            if (GetLevelState(chapter, index) <= level)
                break;
        return index;
    }
    public int GetFocusKeyLevel(int chapter)//get the focus level which key have not been granted
    {
        int totalLevel = GetTotalLevelsPerRestaurant(chapter);
        int level = GetLevelState(chapter, totalLevel);
        int index = 1;
        LevelData levelData = null;
        for (index = 1; index < totalLevel + 1; index++)
        {
            levelData = LevelDataController.Instance.GetEventLevelData(chapter, index);
            if (GetLevelState(chapter, index) < levelData.key)
                return index;
        }
        for (index = 1; index < totalLevel + 1; index++)
            if (GetLevelState(chapter, index) <= level)
                return index;
        return index;
    }
    #endregion
}
