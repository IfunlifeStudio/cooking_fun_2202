using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Events;
using DG.Tweening;

public class DataController : MonoBehaviour
{
    private static DataController instance;
    public static DataController Instance { get { return instance; } }
    public const int MAX_RESTAURANT = 17;
    private float[] RANK_BASE = { 0.99f, 4.99f, 9.99f, 19.99f, 49.99f };
    public int currentChapter = -1;
    [HideInInspector] public UnityEvent onDataChange;
    [SerializeField] private GameData gameData;
    [SerializeField] private RestaurantData[] restaurantDatas;
    [SerializeField] private ItemsData itemsData;
    private List<int> resUnlocked = new List<int>();
    public VersionApp version;
    public ExtraJobData ExtraJobData;
    public DeviceIdData deviceIdData;
    public AdsRewards adsRewardData;
    public List<string> frNeedHelpList = new List<string>();
    public List<string> frWaveList = new List<string>();
    public List<string> frGivenEnergyList = new List<string>();
    public List<string> frGivenRubyList = new List<string>();
    public bool isFirstOpenPB = true;
    public double isShowNoel = 0;
    
    [SerializeField] private bool isTestUser;
    public string panelAdsShow = "";
    public int RemoveAds
    {
        get
        {
            return gameData.RemoveAds;
        }
        set
        {
            gameData.RemoveAds = value;
        }
    }
    public int Subcribed
    {
        get
        {
            return gameData.Subscribed;
        }
        set
        {
            gameData.Subscribed = value;
        }
    }
    public int CanShowZoneOffer
    {
        get { return gameData.canShowZoneOffer; }
        set { gameData.canShowZoneOffer = value; }
    }
    public double IsShowNoel
    {
        get { return isShowNoel; }
    }
    public int IceCreamTimeLevel
    {
        get
        {
            return gameData.IceCreamLevel;
        }
        set
        {
            gameData.IceCreamLevel = value;
        }
    }
    public double IceCreamTimeStamp
    {
        get
        {
            if (gameData.IceCreamTimeStamp == 0)
            {
                gameData.IceCreamTimeStamp = ConvertToUnixTime(DateTime.UtcNow);
                IceCreamDuration = 0;
            }
            return gameData.IceCreamTimeStamp;
        }
        set
        {
            gameData.IceCreamTimeStamp = value;
        }
    }
    public int IceCreamDuration
    {
        get
        {
            if (gameData.IceCreamTimeStamp == 0)
            {
                gameData.IceCreamTimeStamp = ConvertToUnixTime(DateTime.UtcNow);
                gameData.IceCreamTimeDuration = 0;
            }
            return gameData.IceCreamTimeDuration;
        }
        set { gameData.IceCreamTimeDuration = Mathf.Max(0, value); }
    }
    public double EnergyTimeStamp
    {
        get { return gameData.energyTimeStamp; }
        set { gameData.energyTimeStamp = value; }
    }
    public double UnlimitedEnergyTimeStamp
    {
        set { gameData.unlimitedEnergyTimeStamp = value; }
        get { return gameData.unlimitedEnergyTimeStamp; }
    }
    public double VideoRewardTimeStamp
    {
        set { gameData.videoRewardTimeStamp = value; }
        get { return gameData.videoRewardTimeStamp; }
    }
    public int VideoRewardDuration
    {
        get { return gameData.videoRewardDuration; }
        set { gameData.videoRewardDuration = Mathf.Max(0, value); }
    }
    public int UnlimitedEnergyDuration
    {
        get { return gameData.unlimitedEnergyTime; }
        set { gameData.unlimitedEnergyTime = Mathf.Max(0, value); }
    }
    public double SpecialWeekendTimeStamp
    {
        set { gameData.specialWeekendTimeStamp = value; }
        get { return gameData.specialWeekendTimeStamp; }
    }
    public double BonusSundayTimeStamp
    {
        set { gameData.bonusSundayTimeStamp = value; }
        get { return gameData.bonusSundayTimeStamp; }
    }
    public int GetUnlimitedEnergyTime()
    {
        return gameData.unlimitedEnergyTime;
    }
    public void AddUnlimitedEnergy(int minutes)
    {
        if (gameData.unlimitedEnergyTime == 0)
            gameData.unlimitedEnergyTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
        gameData.unlimitedEnergyTime += minutes * 60;
    }
    public bool IsUnlimitedEnergy()
    {
        double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - gameData.unlimitedEnergyTimeStamp; ;
        int remainTime = gameData.unlimitedEnergyTime - Mathf.Max(0, (int)deltaTime);
        return remainTime > 0;
    }
    public int Energy
    {
        get { return gameData.energy; }
        set
        {
            if (value > gameData.energy || (gameData.energy == 5 && value == 4))
                gameData.energyTimeStamp = ConvertToUnixTime(DateTime.UtcNow);
            gameData.energy = Mathf.Clamp(value, 0, 5);
            SaveData(false);
        }
    }
    public int Rank
    {
        get { return gameData.rank; }
        set { gameData.rank = value; }
    }
    public void AddIapDaily(float value = 0)
    {
        gameData.IapDaily += value;
        if (PlayerPrefs.GetInt("total_watched_ads", 0) > 20)
        {
            PlayerPrefs.SetInt("total_watched_ads", 0);
            DeCreaseRank();
        }
        int index = 0, tmpRank = 0;
        if (Rank == 0) index = 0;
        else index = Rank - 1;
        for (int i = 0; i < RANK_BASE.Length; i++)
        {
            if (gameData.IapDaily >= RANK_BASE[i])
                tmpRank++;
        }
        if (tmpRank > Rank)
        {
            Rank = tmpRank;
            gameData.IapDaily = 0;
        }
        else if (gameData.IapDaily > RANK_BASE[index])
            InCreaseRank();
        SaveData(false);
        //float timeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow)
    }
    public void InCreaseRank()
    {
        gameData.IapDaily = 0;
        Rank = Mathf.Clamp(Rank + 1, 0, 5);
    }
    public void DeCreaseRank()
    {
        Rank = Mathf.Clamp(Rank - 1, 0, 5);
    }
    public void IncreaseOneEnergy()//this func is made for increase energy without changing the timestamp
    {
        gameData.energy = Mathf.Clamp(gameData.energy + 1, 0, 5);
    }
    //public int Experience
    //{
    //    get { return gameData.xp; }
    //    set
    //    {
    //        gameData.xp = value;
    //        onDataChange.Invoke();
    //        SaveData(false);
    //    }
    //}
    //public int GetRemainExperience()
    //{
    //    return gameData.GetLevelRemainExperience();
    //}
    //public int GetLevelUpExperience()
    //{
    //    return gameData.GetLevelUpExperience();
    //}
    public void AddCustomerSkin(int customerId)
    {
        if (gameData.customerCollection.Length == 0) gameData.customerCollection = new int[] { customerId };
        int[] newCol = new int[gameData.customerCollection.Length + 1];
        for (int i = 0; i < gameData.customerCollection.Length; i++)
        {
            if (gameData.customerCollection[i] == customerId) return;
            newCol[i] = gameData.customerCollection[i];
        }
        newCol[gameData.customerCollection.Length] = customerId;
        gameData.customerCollection = newCol;
    }
    public bool HasUnlockCustomerSkin(int customerId)
    {
        if (gameData.customerCollection == null)
        {
            gameData.UpdateBattePassData();
        }
        for (int i = 0; i < gameData.customerCollection.Length; i++)
        {
            if (gameData.customerCollection[i] == customerId)
                return true;
        }
        return false;
    }
    public void AddAvatarToCollection(int avatarId)
    {
        int[] newCol = new int[gameData.avatarCollection.Length + 1];
        for (int i = 0; i < gameData.avatarCollection.Length; i++)
        {
            if (gameData.avatarCollection[i] == avatarId) return;
            newCol[i] = gameData.avatarCollection[i];
        }
        newCol[gameData.avatarCollection.Length] = avatarId;
        gameData.avatarCollection = newCol;
    }
    public bool HasUnlockAvatar(int avatarId)
    {
        for (int i = 0; i < gameData.avatarCollection.Length; i++)
        {
            if (gameData.avatarCollection[i] == avatarId)
                return true;
        }
        return false;
    }
    public int Level
    {
        get { return gameData.Level; }
    }
    public int Gold
    {
        get { return gameData.gold; }
        set
        {
            MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnCoinChange, new object[] { value - gameData.gold }));
            gameData.gold = value;
            //AchivementController.Instance.OnEarnCoin(value);
            onDataChange.Invoke();
        }
    }
    public int Ruby
    {
        get { return gameData.ruby; }
        set
        {
            int pre_ruby = gameData.ruby;
            gameData.ruby = value;
            if (gameData.ruby < pre_ruby)
            {
                int rubySpent = PlayerPrefs.GetInt("ruby_spent", 0);
                float rb_spent_time = PlayerPrefs.GetFloat("ruby_spent_time", (float)ConvertToUnixTime(System.DateTime.UtcNow));
                if (ConvertToUnixTime(System.DateTime.UtcNow) - rb_spent_time < 604800)
                {
                    PlayerPrefs.SetInt("ruby_spent", Mathf.Abs(value));
                    PlayerPrefs.SetFloat("ruby_spent_time", (float)ConvertToUnixTime(System.DateTime.UtcNow));
                }
                else
                {
                    rubySpent += Mathf.Abs(value);
                    PlayerPrefs.SetInt("ruby_spent", rubySpent);
                }
            }
            onDataChange.Invoke();
        }
    }
    //public AchivementDatas ResAchievementData(int AchivementID)
    //{
    //    for (int i = 0; i < gameData.achivementDatas.Count; i++)
    //    {
    //        if (gameData.achivementDatas[i].achievementID == AchivementID)
    //            return gameData.achivementDatas[i];
    //    }
    //    gameData.achivementDatas.Add(new AchivementDatas(AchivementID));
    //    return gameData.achivementDatas[gameData.achivementDatas.Count - 1];
    //}
    //public List<AchivementDatas> AchievementList
    //{
    //    get { return gameData.achivementDatas; }
    //}
    public int[] pbStorageMilestone
    {
        get { return new int[] { 60, 180, 300, 600, 1200, 3000, 6000 }; }
    }
    public bool IsFullPB()
    {
        return gameData.pbRuby >= pbStorageMilestone[PbLevel - 1];
    }
    public int PbLevel
    {
        get
        {
            if (gameData.pbLevel == 0) gameData.pbLevel = 1;
            return Mathf.Clamp(gameData.pbLevel, 1, 8);
        }
        set { gameData.pbLevel = Mathf.Clamp(value, 1, 8); }
    }
    public int PbRuby
    {
        get { return gameData.pbRuby; }
        set { gameData.pbRuby = Mathf.Clamp(value, 0, pbStorageMilestone[PbLevel - 1]); }
    }
    public int CurrentPbStorage
    {
        get { return pbStorageMilestone[PbLevel - 1]; }
    }
    public int PbTimeDuration
    {
        get { return gameData.pbTimeDuration; }
        set { gameData.pbTimeDuration = value; }
    }
    public double PbTimeStamp
    {
        get { return gameData.pbTimeStamp; }
        set { gameData.pbTimeStamp = value; }
    }
    private string dataPath = "";
    private void Start()
    {

        if (instance == null)
        {
            instance = this;
            currentChapter = -1;
            dataPath = Path.Combine(Application.persistentDataPath, "data.dat");
#if UNITY_IOS
            Application.targetFrameRate=60;
#endif

            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        gameObject.GetComponent<PushNotification>().SetUpNotification();
    }
    public void LoadData()
    {
        LoadLocalData();
        PrepareGameData();
    }
    public void PrepareGameData()
    {
        //EventDataController.Instance.LoadData();
        PlayerClassifyController.Instance.LoadData();
        FindObjectOfType<EventManager>().InitiateGameEvent();
        // QuestController.Instance.InitiateQuest();
        if (FindObjectOfType<IAPSilentProcesser>() != null)
            FindObjectOfType<IAPSilentProcesser>().canProcessIAP = true;
        LevelDataController.Instance.LoadLevel(1, 1);
        if (PlayerPrefs.GetInt("day_of_year", 0) != System.DateTime.Now.DayOfYear)
            gameData.IapDaily = 0;
        if (GetLevelState(1, 1) == 0 && DataController.Instance.HasRestaurantTutorial(111111))
        {
            currentChapter = 1;
            SceneController.Instance.LoadScene("Restaurant1_1");
        }
        else
            SceneController.Instance.LoadScene("MainMenu", false);
        CheckUser();
        gameObject.GetComponent<PushNotification>().SetUpNotification();
    }
    public void LoadLocalData()
    {
        LoadDeviceIdData();
        LoadVersiondata();
        LoadExtraJobData();
        LoadABtesting();
        LoadRubyValueInWaveFriend();
        LoadAdsRewardData();
        AchivementController.Instance.LoadDefaultAchievement();
        string _itemsData = FirebaseServiceController.Instance.GetItemsData("item_config");
        itemsData = JsonUtility.FromJson<ItemsData>(_itemsData);
        string resData = null;//read all config from Resources
        int index = 0;

        double noelShow = FirebaseServiceController.Instance.GetNoelSetting();
        List<RestaurantData> resDatas = new List<RestaurantData>();
        do
        {
            index++;
            resData = FirebaseServiceController.Instance.GetRestaurantData("res_config_" + index);
            if (resData != null && resData.Length > MAX_RESTAURANT)
            {
                var tmp = JsonUtility.FromJson<RestaurantData>(resData);
                resDatas.Add(tmp);
            }
        }
        while (resData != null && resData.Length > MAX_RESTAURANT);
        restaurantDatas = resDatas.ToArray();
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string data;
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                try
                {
                    data = (string)binaryFormatter.Deserialize(fileStream);
                    isShowNoel = noelShow;
                    gameData = JsonUtility.FromJson<GameData>(data);
                    gameData.UpdateGameData(itemsData);
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
        SaveData(false);
    }
    public GameData GetGameData()
    {
        return gameData;
    }
    public void ResetData()
    {
        //LevelData[] levelsData = LevelDataController.Instance.LoadAllLevels();
        gameData = new GameData(itemsData);
        SaveData(false);
    }
    public void SaveData(bool postData = true)
    {
        DOVirtual.DelayedCall(0.1f, () =>
         {
             gameData.SaveTime = DateTime.Now.Ticks;
             string origin = JsonUtility.ToJson(gameData);
             BinaryFormatter binaryFormatter = new BinaryFormatter();
             using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
             {
                 binaryFormatter.Serialize(fileStream, origin);
             }
             if (postData)
                 DatabaseController.Instance.PostData();
         }
            );
    }
    public void SaveUserID(string UserID)
    {
        gameData.userID = UserID;
    }
    public void SaveData(GameData gameData2)
    {
        gameData = gameData2;
        string origin = JsonUtility.ToJson(gameData2);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, origin);
        }
    }
    #region Item Helpers Function
    public bool HasClaimFree(int itemId)
    {
        return gameData.HasClaimFree(itemId);
    }
    public void ClaimFree(int itemId)
    {
        gameData.ClaimFree(itemId);
    }
    public bool IsItemUnlocked(int itemId)
    {
        return itemsData.IsItemUnlock(itemId);
    }
    public string GetItemName(int itemId)
    {
        return itemsData.GetItemName(itemId);
    }
    public string GetItemUnlockMessage(int itemId)
    {
        return itemsData.GetItemUnlockMessage(itemId);
    }
    public ItemData GetItemDataById(int itemId)
    {
        return itemsData.GetItemDataById(itemId);
    }
    public void UseItem(int itemId, int amount)
    {
        gameData.UseItem(itemId, amount);
        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnUseItem, new object[] { itemId }));
    }
    public void AddItem(int itemId, int amount)
    {
        gameData.AddItem(itemId, amount);
    }
    public int GetItemQuantity(int itemId)
    {
        return gameData.GetItemQuantity(itemId);
    }
    public int GetItemCost(int itemId)
    {
        return itemsData.GetItemCost(itemId);
    }
    public int GetRandomItemId()
    {
        return itemsData.GetRandomItemId();
    }
    public int[] GetAllItemIds()
    {
        List<int> itemIds = new List<int>();
        for (int i = 0; i < itemsData.items.Length; i++)
        {
            if (!itemIds.Contains(itemsData.items[i].id))
                itemIds.Add(itemsData.items[i].id);
        }
        return itemIds.ToArray();
    }
    #endregion
    public int GetFoodValue(Food food)
    {
        int foodValue = 0;
        foreach (int ingredient in food.IngredientIds)
            foodValue += GetIngredientValue(ingredient);
        if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
            return (int)(foodValue * 1.5f);
        return foodValue;
    }
    public int[] GetLevelProgressUnlock(int chapter)
    {
        return GetRestaurantById(chapter).levelProgressLock;
    }
    public int GetTotalLevelsPerRestaurant(int chapter)
    {
        var currentRestaurant = GetRestaurantById(chapter);
        if (currentRestaurant == null) return 10;
        return currentRestaurant.levelProgressLock[currentRestaurant.levelProgressLock.Length - 1];
    }
    public int GetRecommendUpgradeId()
    {
        return GetRestaurantById(currentChapter).GetRecommendUpgradeId();
    }
    public int GetRecommendIngredientUpgradeId()
    {
        return GetRestaurantById(currentChapter).GetRecommendIngredientUpgradeId();
    }
    public int GetRecommendMachineUpgradeId()
    {
        return GetRestaurantById(currentChapter).GetRecommendMachineUpgradeId();
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
    public int GetIngredientUpgradeValue(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetUpgradeIngredientValue(ingredientId);
    }
    public int GetIngredientRewardExp(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetIngredientUpgradeExp(ingredientId);
    }
    public int GetIngredientUpgradeCost(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetIngredientUpgradeCost(ingredientId);
    }
    public float GetIngredientUpgradeTime(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetIngredientUpgradeTime(ingredientId);
    }
    public IngredientData GetIngredient(int ingredientId)
    {
        return GetRestaurantById(currentChapter).GetIngredient(ingredientId);
    }
    public int GetIngredientLevel(int ingredientId)
    {
        return gameData.GetIngredientState(ingredientId);
    }
    public void AddIngredientUpgradeProcess(int ingredientId)
    {
        gameData.AddIngredientUpgradeProcess(ingredientId);
    }
    public void RemoveIngredientUpgradeProcess(int ingredientId)
    {
        gameData.RemoveIngredientUpgradeProcess(ingredientId);
    }
    public void CompletedIngredientUpgrade(int ingredientId)
    {
        gameData.GetIngredientStateData(ingredientId).Level++;
    }
    public void HasCompletedIngredientUpgradeInGame(int ingredientId)
    {
        var process = gameData.GetUpgradeIngredientProcessing(ingredientId);
        if (process != null)
        {
            process.hasCompleteUpgradeInGame = true;
            process.timeStamp = 0;
        }
    }
    public void HasCompletedIngredientUpgradeInMenu(int ingredientId)
    {
        var process = gameData.GetUpgradeIngredientProcessing(ingredientId);
        if (process != null)
        {
            process.hasCompleteUpgradeInMenu = true;
            process.timeStamp = 0;
        }
    }
    public bool HasJustCompletedIngredientUpgradeProcess(int ingredientId)
    {
        var process = gameData.GetUpgradeIngredientProcessing(ingredientId);
        if (process != null)
        {
            if (process.hasCompleteUpgradeInMenu || process.timeStamp == 0) return false;
            if (process.hasCompleteUpgradeInGame) return true;
            if (GetIngredient(ingredientId).UpgradeTime - (DataController.ConvertToUnixTime(DateTime.UtcNow) - process.timeStamp) <= 0) return true;
        }
        return false;
    }
    public List<int> GetAllUpgradeableIngredients()
    {
        List<int> upgradeableIngredients = new List<int>();
        RestaurantData currentRestaurant = GetRestaurantById(currentChapter);
        var ingredients = currentRestaurant.ingredients;
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (currentRestaurant.GetIngredientUpgradeCost(ingredients[i].id) <= Gold)
                upgradeableIngredients.Add(ingredients[i].id);
        }
        return upgradeableIngredients;
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
        if (!ignoreNoobMode)
        {
            if (PlayerClassifyController.Instance.GetPlayerTier() == -1) return GetRestaurantById(currentChapter).GetWorkTime(machineId) * 0.7f;
            else
            if (PlayerClassifyController.Instance.GetPlayerTier() == 1) return GetRestaurantById(currentChapter).GetWorkTime(machineId) * 1.15f;
        }
        return GetRestaurantById(currentChapter).GetWorkTime(machineId);
    }
    public float GetMachineNextLevelWorkTime(int machineId)
    {
        return GetRestaurantById(currentChapter).GetNextLevelWorkTime(machineId);
    }
    public int GetMachineUpgradeExp(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineUpgradeExp(machineId);
    }
    public MachineData GetMachine(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachine(machineId);
    }
    public int GetMachineLevel(int machineId)
    {
        return gameData.GetMachineState(machineId);
    }
    public void SetMachineLevel(int machineId, int level)
    {
        gameData.SetMachineState(machineId, level);
    }
    public int GetMachineCount(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineCount(machineId);
    }
    public int GetNextLevelMachineCount(int machineId)
    {
        return GetRestaurantById(currentChapter).GetNextLevelMachineCount(machineId);
    }
    public int GetMachineUpgradeCost(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineUpgradeCost(machineId);
    }
    public float GetMachineUpgradeTime(int machineId)
    {
        return GetRestaurantById(currentChapter).GetMachineUpgradeTime(machineId);
    }
    public void AddMachineUpgradeProcess(int machineId)
    {
        gameData.AddMachineUpgradeProcess(machineId);
    }
    public void RemoveMachineUpgradeProcess(int machineId)
    {
        gameData.RemoveMachineUpgradeProcess(machineId);
    }
    public void CompletedMachineUpgrade(int machineId, bool hasCompleteInMenu, bool hasCompleteInGame)
    {
        gameData.GetMachineStateData(machineId).Level++;
        var upgradeProcess = gameData.GetUpgradeMachineProcessing(machineId);
        upgradeProcess.hasCompleteUpgradeInGame = hasCompleteInGame;
        upgradeProcess.hasCompleteUpgradeInMenu = hasCompleteInMenu;
        upgradeProcess.timeStamp = 0;
    }
    public void HasCompletedMachineUpgradeInGame(int machineId)
    {
        var process = gameData.GetUpgradeMachineProcessing(machineId);
        if (process != null)
        {
            process.hasCompleteUpgradeInGame = true;
            process.timeStamp = 0;
        }
    }
    public void HasCompletedMachineUpgradeInMenu(int machineId)
    {
        var process = gameData.GetUpgradeMachineProcessing(machineId);
        if (process != null)
        {
            process.hasCompleteUpgradeInMenu = true;
            process.timeStamp = 0;
        }
    }
    public bool HasJustCompletedMachineUpgradeProcess(int machineId)
    {
        var process = gameData.GetUpgradeMachineProcessing(machineId);
        if (process != null)
        {
            if (process.hasCompleteUpgradeInMenu || process.timeStamp == 0) return false;
            if (process.hasCompleteUpgradeInGame) return true;
            if (GetMachine(machineId).UpgradeTime - (DataController.ConvertToUnixTime(DateTime.UtcNow) - process.timeStamp) <= 0) return true;
        }
        return false;
    }
    public void DecreaseMachineUpgradeTime(int machineId, int time)
    {
        var upgradeProcess = gameData.GetUpgradeMachineProcessing(machineId);
        upgradeProcess.timeStamp -= time;
    }
    public List<int> GetAllUpgradeableMachines()
    {
        List<int> upgradeableMachines = new List<int>();
        RestaurantData currentRestaurant = GetRestaurantById(currentChapter);
        var machines = currentRestaurant.machines;
        for (int i = 0; i < machines.Length; i++)
        {
            if (currentRestaurant.GetMachineUpgradeCost(machines[i].id) <= Gold)
                upgradeableMachines.Add(machines[i].id);
        }
        return upgradeableMachines;
    }
    #endregion
    #region Res Helper Function
    public RestaurantData GetRestaurantById(int id)
    {
        foreach (var res in restaurantDatas)
            if (res.id == id)
                return res;
        return null;
    }
    public int HighestRestaurant
    {
        get
        {
            if (gameData.highestRestaurant == 0 || gameData.highestRestaurant < currentChapter)
                gameData.highestRestaurant = GetHighestRestaurant();
            if (gameData.highestRestaurant > MAX_RESTAURANT) gameData.highestRestaurant = MAX_RESTAURANT;
            return gameData.highestRestaurant;
        }
        set
        {
            if (value > gameData.highestRestaurant && value <= MAX_RESTAURANT)
                gameData.highestRestaurant = value;
        }
    }
    public int HighestLevelInHighestChapter
    {
        get { return GetHighestLevel(HighestRestaurant); }
    }
    public int GetHighestRestaurant()
    {
        int result = 0;
        for (int i = 1; i <= MAX_RESTAURANT; i++)
        {
            if (IsRestaurantUnlocked(i))
                result++;
        }
        return result;
    }
    public bool IsRestaurantUnlocked(int id)
    {
#if UNITY_EDITOR
        return true;
#endif
        if (IsTestUser)
            return true;
        if (id >= 16)
            return false;
        if (GetRestaurantById(id) == null) return false;
        for (int i = 0; i < resUnlocked.Count; i++)
        {
            if (resUnlocked[i] == id)
                return true;
        }
        bool result = GetTotalKeyGranted() >= GetRestaurantById(id).keyRequired;
        if (result)
            resUnlocked.Add(id);
        return result;
    }
    public int GetTotalKeyGranted()
    {
#if UNITY_EDITOR
        return 700;
#endif
        int result = 0;
        int totalRestaurant = restaurantDatas.Length;
        for (int i = 0; i < totalRestaurant; i++)
        {
            int keyGrantedInRes = GetRestaurantKeyGranted(restaurantDatas[i].id);
            if (keyGrantedInRes == 0)
                break;
            result += keyGrantedInRes;
        }
        return result;
    }
    public int GetRestaurantKeyGranted(int chapterId)
    {
        int result = 0;
        for (int i = 0; i < gameData.levelStates.Length; i++)
        {
            if (gameData.levelStates[i].chapterId == chapterId && gameData.levelStates[i].HasKeyGranted)
            {
                result++;
            }
        }
        return result;
    }
    public bool HasRestaurantRewarded(int chapterId)
    {
        bool result = gameData.HasRestaurantRewarded(chapterId);
        if (!result && PlayerPrefs.GetInt("res_reward" + chapterId, 0) == 1)
            gameData.SetRestaurantRewarded(chapterId);//do the migrate from old version
        return gameData.HasRestaurantRewarded(chapterId);
    }
    public void SetRestaurantRewarded(int chapterId)
    {
        gameData.SetRestaurantRewarded(chapterId);
        SaveData();
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
        gameData.SetLevelState(chapter, levelId, value, true);
    }
    public void SetLevelState(LevelData levelData, int chapter, int levelId)
    {
        //string levelDataStr = FirebaseServiceController.Instance.GetLevelData("level_" + chapter + "_" + levelId);
        //var levelData = JsonUtility.FromJson<LevelData>(levelDataStr);
        var levelState = GetLevelState(chapter, levelId);
        bool hasGranted = (levelState + 1) >= levelData.key;
        gameData.SetLevelState(chapter, levelId, levelState + 1, hasGranted);
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
        for (index = 1; index < totalLevel + 1; index++)
            if (!LevelDataController.Instance.GetLevelData(chapter, index).IsKeyGranted())
                return index;
        for (index = 1; index < totalLevel + 1; index++)
            if (GetLevelState(chapter, index) <= level)
                return index;
        return index;
    }
    public int GetTotalStageGranted(int chapter)
    {
        int result = 0;

        for (int i = 0; i < gameData.levelStates.Length; i++)
        {
            if (gameData.levelStates[i].chapterId == chapter)
            {
                result += gameData.levelStates[i].level;
            }
        }


        //int totalLevel = GetTotalLevelsPerRestaurant(chapter);
        //for (int i = 1; i < totalLevel + 1; i++)
        //    result += GetLevelState(chapter, i);
        return result;
    }
    #endregion
    public List<int> GetTutorialData()
    {
        return gameData.completedTutorials;
    }
    public bool HasRestaurantTutorial(int id)
    {
        for (int i = 0; i < gameData.completedTutorials.Count; i++)
        {
            if (gameData.completedTutorials[i] == id)
                return false;
        }
        gameData.completedTutorials.Add(id);
        return true;
    }
    #region Static Time Helper
    public static double ConvertToUnixTime(DateTime time)
    {
        DateTime epoch = new System.DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return (time - epoch).TotalSeconds;
    }
    //public static float ConvertDateTimeToSecond(DateTime dateTime)
    //{
    //    int secsInAMin = 60;
    //    int secsInAnHour = 60 * secsInAMin;
    //    int secsInADay = 24 * secsInAnHour;
    //    int secsInAYear = (int)365.25 * secsInADay;

    //    float totalSeconds = (float)(dateTime.Year * secsInAYear) +
    //                   (dateTime.DayOfYear * secsInADay) +
    //                   (dateTime.Hour * secsInAnHour) +
    //                   (dateTime.Minute * secsInAMin) +
    //                   dateTime.Second;

    //    return totalSeconds;
    //}
    public static DateTime ConvertFromUnixTime(double timeStamp)
    {
        DateTime epoch = new System.DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime time = epoch.AddSeconds(timeStamp);
        return time;
    }
    public static int GetHours(float totalSeconds)
    {
        return (int)(totalSeconds / 3600f);
    }
    public static int GetMinutes(float totalSeconds)
    {
        return (int)((totalSeconds / 60) % 60);
    }
    public static int GetSeconds(float totalSeconds)
    {
        return (int)(totalSeconds % 60);
    }
    #endregion
    public void LoadVersiondata()
    {
        string _version = FirebaseServiceController.Instance.GetVersionApp();
        version = JsonUtility.FromJson<VersionApp>(_version);
    }
    public bool CheckVersion()
    {
        string current_version = Application.version;
        string new_version = "";

#if UNITY_ANDROID
        new_version = version.androidVersion;
#elif UNITY_IOS
        new_version= version.iosVersion;
#endif
        if (new_version == "")
            return false;
        string[] curr_ver_arr = current_version.Split('.');
        string[] new_ver_arr = new_version.Split('.');
        if (new_ver_arr.Length > current_version.Length)
            return true;
        else if (new_ver_arr.Length == curr_ver_arr.Length)
        {
            for (int i = 0; i < curr_ver_arr.Length; i++)
            {
                try
                {
                    int curVersion = int.Parse(curr_ver_arr[i]);
                    int newVersion = int.Parse(new_ver_arr[i]);
                    if (curVersion < newVersion)
                        return true;
                    else if (curVersion > newVersion)
                        return false;
                }
                catch
                {
                    Debug.Log("Can't Parse!!!");
                    return false;
                }
            }
        }
        return false;
    }
    public void LoadExtraJobData()
    {
        string _extrajob = FirebaseServiceController.Instance.GetExtraJobData();
        ExtraJobData = JsonUtility.FromJson<ExtraJobData>(_extrajob);
    }

    public void LoadDeviceIdData()
    {
        string deviceid = FirebaseServiceController.Instance.GetDeviceId();
        deviceIdData = JsonUtility.FromJson<DeviceIdData>(deviceid);
    }
    public void LoadABtesting()
    {
        string abTesting = FirebaseServiceController.Instance.GetABtestingIndex();
        string ShopIngameAbTesting = FirebaseServiceController.Instance.GetShopIngameABTesting();
        if (abTesting != "")
        {
            PlayerPrefs.SetString("ABtesting_data", abTesting);
        }
        if (ShopIngameAbTesting != "")
        {
            PlayerPrefs.SetString("shop_ingame_abtest", ShopIngameAbTesting);
        }
    }
    public void LoadRubyValueInWaveFriend()
    {
        int value = FirebaseServiceController.Instance.GetRubyValueInWaveFriend();
        PlayerPrefs.SetInt("wave_ruby_value", value);
    }
    public void LoadAdsRewardData()
    {
        string adsData = FirebaseServiceController.Instance.GetAdsRewardData();
        adsRewardData = JsonUtility.FromJson<AdsRewards>(adsData);
    }

    #region Cheat

    public bool IsTestUser { get { return isTestUser; } }
    public void CheckUser()
    {
        if (deviceIdData.CheckDeviceId(gameData.userID) || deviceIdData.CheckDeviceId(SystemInfo.deviceUniqueIdentifier))
            isTestUser = true;
        else
        {
            isTestUser = false;
            int type_user = PlayerPrefs.GetInt("LOGIN_TYPE", 2);
            if (type_user == 3)
                APIController.Instance.LogEventTrackingUserType("user_apple");
            else if (type_user == 1)
                APIController.Instance.LogEventTrackingUserType("user_facebook");
            else
                APIController.Instance.LogEventTrackingUserType("user_not_login");
        }
    }
    public void ResetAllData()
    {
        if (PlayerPrefs.GetInt("LOGIN_TYPE", 2) == 2)
        {

            //LevelData[] levelsData = LevelDataController.Instance.LoadAllLevels();
            gameData = new GameData(itemsData);
            Caching.ClearCache();
            SaveData(false);
        }
        else
        {
            string firebaseID = GetGameData().userID;
            //LevelData[] levelsData = LevelDataController.Instance.LoadAllLevels();
            gameData = new GameData(itemsData);
            SaveUserID(firebaseID);
            SaveData(true);
            LoadData();
        }

    }
    public void CheatRuby()
    {
        Ruby += 200;
        SaveData();
    }
    public void RemoveRuby()
    {
        Ruby -= 200;
        if(Ruby <= 0) Ruby=0;
        SaveData();
    }
    #endregion
}