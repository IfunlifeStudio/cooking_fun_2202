using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    private const int BASE_LEVEL_TIER1 = 350, BASE_LEVEL_TIER2 = 2051;
    private const int BASE_LEVEL_INCREASE_TIER1 = 10, BASE_LEVEL_INCREASE_TIER2 = 115;
    public int xp, gold, ruby, energy, rank;
    public double energyTimeStamp = 0, unlimitedEnergyTimeStamp = 0, IceCreamTimeStamp, videoRewardTimeStamp = 0, specialWeekendTimeStamp = 0, bonusSundayTimeStamp = 0, pbTimeStamp;
    public int unlimitedEnergyTime, IceCreamTimeDuration, videoRewardDuration = 0, pbTimeDuration = 0;
    public int[] gameResultRecords = new int[] { -1, -1, -1, -1, -1 };
    public int[] restaurantRewardRecords = new int[] { };
    public int[] customerCollection = new int[] { };
    public int[] avatarCollection = new int[] { };
    public List<LevelRewardProgressData> levelRewardProgress;
    public ItemState[] itemStates;
    public MachineState[] machineStates;
    public IngredientState[] ingredientStates;
    public LevelState[] levelStates;
    public List<AchivementDatas> achivementDatas;
    public List<int> completedTutorials = new List<int>();
    public int IceCreamLevel = 0;
    public bool IsRemoveNoel2021 = false;
    public string userID;
    public long SaveTime;
    public float IapDaily = 0, userIapValue;
    public int RemoveAds = 0, hasClaimLoginReward = 0, Subscribed, pbLevel = 1, pbRuby = 0, canShowZoneOffer = 0, highestRestaurant = 0, restaunrantUnlockReward = 0;
    public int totalWin, totalCombo, totalLike, totalServedFood, totalPlayedGame;
    public bool hasKeyGrantedFeatureUpdated, hasUpdateLevelStatePatch;
    public ProfileData profileDatas;
    public List<int> borderCollection = new List<int>();
    public List<string> WavedFriendList = new List<string>();
    public List<CertificateData> certificateDatas = new List<CertificateData>();
    public List<UpgradeIngredientProcess> upgradeIngredientProcesses = new List<UpgradeIngredientProcess>();
    public List<UpgradeMachineProcess> upgradeMachineProcesses = new List<UpgradeMachineProcess>();
    public BattlePassData battlePassData;
    public void AddBorderId(int borderId)
    {
        if (!borderCollection.Contains(borderId))
        {
            borderCollection.Add(borderId);
        }
    }
    public int Level
    {
        get
        {
            int level = 0;
            int _xp = xp;
            do
            {
                if (level < 9)
                    _xp = _xp - BASE_LEVEL_TIER1 - level * BASE_LEVEL_INCREASE_TIER1;
                else
                    _xp = _xp - BASE_LEVEL_TIER2 - (level - 9) * BASE_LEVEL_INCREASE_TIER2;
                level++;
            } while (_xp >= 0);
            return level;
        }
    }
    public int GetLevelState(int chapter, int levelId)
    {
        foreach (LevelState levelState in levelStates)
            if (levelState.chapterId == chapter && levelState.id == levelId)
                return levelState.Level;
        List<LevelState> levels = new List<LevelState>(levelStates);//init new level state
        var newLevel = new LevelState(chapter, levelId, false)
        {
            Level = 0,
            HasKeyGranted = false
        };
        levels.Add(newLevel);
        levelStates = levels.ToArray();
        return 0;
    }
    public void SetLevelState(int chapter, int levelId, int value, bool hasGranted)
    {
        LevelState levelState = null;
        foreach (LevelState _levelState in levelStates)
            if (_levelState.chapterId == chapter && _levelState.id == levelId)
                levelState = _levelState;
        if (levelState != null)
        {
            levelState.Level = value;
            levelState.HasKeyGranted = hasGranted;
        }
        else
        {
            List<LevelState> levels = new List<LevelState>(levelStates);//init new level state
            var newLevel = new LevelState(chapter, levelId, hasGranted);
            newLevel.Level = value;
            levels.Add(newLevel);
            levelStates = levels.ToArray();
            UnityEngine.Debug.LogWarning("Cant find this level state. Init a new one");
        }
    }
    public int GetMachineState(int machineId)
    {
        foreach (var machineState in machineStates)
            if (machineState.id == machineId)
                return machineState.Level;
        List<MachineState> machines = new List<MachineState>(machineStates);//init new machine state
        machines.Add(new MachineState(machineId, 1));
        machineStates = machines.ToArray();
        return 1;
    }
    public void SetMachineState(int machineId, int level)
    {
        MachineState machine = null;
        foreach (var machineState in machineStates)
            if (machineState.id == machineId)
                machine = machineState;
        if (machine != null) machine.Level = level;
        else
        {
            List<MachineState> machines = new List<MachineState>(machineStates);//init new machine state
            machines.Add(new MachineState(machineId, level));
            machineStates = machines.ToArray();
            UnityEngine.Debug.LogError("Cant find this machine, init a new one");
        }
    }
    public int GetIngredientState(int ingredientId)
    {
        foreach (var ingredientState in ingredientStates)
            if (ingredientState.id == ingredientId)
                return ingredientState.Level;
        List<IngredientState> ingredients = new List<IngredientState>(ingredientStates);//init new machine state
        ingredients.Add(new IngredientState(ingredientId, 1));
        ingredientStates = ingredients.ToArray();
        return 1;
    }
    public void AddIngredientUpgradeProcess(int ingredientId)
    {
        bool hasProcessExist = false;
        foreach (var process in upgradeIngredientProcesses)
            if (process.id == ingredientId)
            {
                hasProcessExist = true;
                process.Refresh();
                process.timeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
            }
        if (!hasProcessExist)
            upgradeIngredientProcesses.Add(new UpgradeIngredientProcess(ingredientId, DataController.ConvertToUnixTime(System.DateTime.UtcNow)));
    }
    public void AddMachineUpgradeProcess(int machineId)
    {
        bool hasProcessExist = false;
        foreach (var process in upgradeMachineProcesses)
            if (process.id == machineId)
            {
                hasProcessExist = true;
                process.Refresh();
                process.timeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
            }
        if (!hasProcessExist)
            upgradeMachineProcesses.Add(new UpgradeMachineProcess(machineId, DataController.ConvertToUnixTime(System.DateTime.UtcNow)));
    }
    public void RemoveIngredientUpgradeProcess(int ingredientId)
    {
        for (int i = 0; i < upgradeIngredientProcesses.Count; i++)
        {
            if (upgradeIngredientProcesses[i].id == ingredientId)
                upgradeIngredientProcesses.RemoveAt(i);
        }

    }
    public void RemoveMachineUpgradeProcess(int ingredientId)
    {
        for (int i = 0; i < upgradeMachineProcesses.Count; i++)
        {
            if (upgradeMachineProcesses[i].id == ingredientId)
                upgradeMachineProcesses.RemoveAt(i);
        }

    }

    public IngredientState GetIngredientStateData(int ingredientId)
    {
        IngredientState ingredient = null;
        foreach (var ingredientState in ingredientStates)
            if (ingredientState.id == ingredientId)
                ingredient = ingredientState;
        if (ingredient != null) return ingredient;
        else return null;

    }
    public MachineState GetMachineStateData(int machineId)
    {
        foreach (var machineState in machineStates)
            if (machineState.id == machineId)
                return machineState;
        return null;
    }
    public double GetIngredientTimestamp(int ingredientId)
    {
        foreach (var ingredientState in ingredientStates)
            if (ingredientState.id == ingredientId)
                return ingredientState.timeStamp;
        return 0;
    }
    public void AddItem(int itemId, int amount)
    {
        foreach (ItemState itemState in itemStates)
            if (itemState.id == itemId)
            {
                itemState.quantity += amount;
                return;
            }
        List<ItemState> itemStatesTmp = new List<ItemState>(itemStates);
        itemStatesTmp.Add(new ItemState(itemId, amount));
        itemStates = itemStatesTmp.ToArray();
    }
    public int GetItemQuantity(int itemId)
    {
        foreach (ItemState itemState in itemStates)
            if (itemState.id == itemId)
                return itemState.quantity;
        return 0;
    }
    public void UseItem(int itemId, int amount)
    {
        foreach (ItemState itemState in itemStates)
            if (itemState.id == itemId)
                itemState.quantity = UnityEngine.Mathf.Max(0, itemState.quantity - amount);
    }
    public bool HasClaimFree(int itemId)
    {
        foreach (ItemState itemState in itemStates)
            if (itemState.id == itemId)
                return itemState.HasClaimFree;
        return true;
    }
    public void ClaimFree(int itemId)
    {
        foreach (ItemState itemState in itemStates)
            if (itemState.id == itemId)
                itemState.HasClaimFree = true;
    }
    public void SaveGameResult(bool isWin, bool isUseItem)
    {
        if (DataController.Instance.currentChapter <= 1) return;
        if (gameResultRecords.Length < 5) gameResultRecords = new int[] { -1, -1, -1, -1, -1 };
        int result = -1;
        if (isWin)
        {
            if (isUseItem)
                result = 6;
            else
                result = 5;
        }
        else
        {
            if (isUseItem)
                result = 1;
            else
                result = 0;
        }
        for (int i = 0; i < gameResultRecords.Length - 1; i++)
            gameResultRecords[i] = gameResultRecords[i + 1];
        gameResultRecords[gameResultRecords.Length - 1] = result;
        CalculatePlayerTier();
    }
    private int playerTier = 0;
    public int PlayerTier
    {
        get { return playerTier; }
    }
    private void CalculatePlayerTier()
    {
        int totalWin = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] >= 5) totalWin++;
            else
                totalWin = 0;
        }
        int totalLoseUseItem = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] == 1) totalLoseUseItem++;
            else
                totalLoseUseItem = 0;
        }
        int totalLoseNotUseItem = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] == 0) totalLoseNotUseItem++;
            else
                totalLoseNotUseItem = 0;
        }
        if (totalWin > 4)
        {
            playerTier = 1;
            return;
        }
        if (playerTier == 1)
        {
            if (totalLoseUseItem > 2 || totalLoseNotUseItem > 2)
            {
                playerTier = 0;
                for (int i = 0; i < gameResultRecords.Length - 1; i++)
                    gameResultRecords[i] = -1;
                return;
            }
        }
        if (playerTier == 0)
        {
            if (totalLoseUseItem > 2 || totalLoseNotUseItem > 3)
            {
                playerTier = -1;
                for (int i = 0; i < gameResultRecords.Length - 1; i++)
                    gameResultRecords[i] = -1;
                return;
            }
        }
        if (playerTier == -1)
        {
            if (totalWin > 0)
            {
                playerTier = 0;
                return;
            }
        }
    }
    public int GetPlayerTier()
    {
        int totalWin = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] >= 4) totalWin++;
            else
                totalWin = 0;
        }
        if (totalWin > 4) return 1;
        int totalLose = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] == 1) totalLose++;
            else
                totalLose = 0;
        }
        if (totalLose > 2) return -1;
        totalLose = 0;
        for (int i = 0; i < gameResultRecords.Length; i++)
        {
            if (gameResultRecords[i] == 0) totalLose++;
            else
                totalLose = 0;
        }
        if (totalLose > 3) return -1;
        return 0;//-1 is noob, 1 is pro, 0 is normal
    }
    public bool HasRestaurantRewarded(int chapterId)
    {
        if (restaurantRewardRecords == null)
            restaurantRewardRecords = new int[] { };
        if (restaurantRewardRecords.Length < chapterId) return false;
        return restaurantRewardRecords[chapterId - 1] == 1;
    }
    public void SetRestaurantRewarded(int chapterId)
    {
        if (restaurantRewardRecords.Length < chapterId)
        {
            int[] newRecords = new int[chapterId];
            for (int i = 0; i < chapterId; i++)
            {
                if (i < restaurantRewardRecords.Length) newRecords[i] = restaurantRewardRecords[i];
                else newRecords[i] = 0;
            }
            restaurantRewardRecords = newRecords;
        }
        restaurantRewardRecords[chapterId - 1] = 1;
    }
    public UpgradeIngredientProcess GetUpgradeIngredientProcessing(int ingredientId)
    {
        for (int i = 0; i < upgradeIngredientProcesses.Count; i++)
        {
            if (upgradeIngredientProcesses[i].id == ingredientId)
                return upgradeIngredientProcesses[i];
        }
        return null;
    }
    public UpgradeMachineProcess GetUpgradeMachineProcessing(int machineId)
    {
        for (int i = 0; i < upgradeMachineProcesses.Count; i++)
        {
            if (upgradeMachineProcesses[i].id == machineId)
                return upgradeMachineProcesses[i];
        }
        return null;
    }
    public void UpdateGameData(ItemsData itemsData)
    {
        if (!IsRemoveNoel2021)
        {
            IsRemoveNoel2021 = true;
            customerCollection = new int[] { };
            battlePassData = new BattlePassData();
        }
        if (itemsData.items.Length > itemStates.Length)
        {
            List<ItemState> tmpItemStates = new List<ItemState>();//update item data
            foreach (ItemData data in itemsData.items)
                tmpItemStates.Add(new ItemState(data.id));
            for (int i = 0; i < tmpItemStates.Count; i++)
            {
                for (int j = 0; j < itemStates.Length; j++)
                {
                    if (tmpItemStates[i].id == itemStates[j].id)
                        tmpItemStates[i] = itemStates[j];
                }
            }
            itemStates = tmpItemStates.ToArray();
        }
        if (!hasKeyGrantedFeatureUpdated)
        {
            UpdateKeyGrantedFeature();
        }
        UpdateLevelState();
        UpdateBattePassData();
    }
    public void UpdateBattePassData()
    {
        if (battlePassData == null)
            battlePassData = new BattlePassData();
        if (customerCollection == null)
            customerCollection = new int[] { };
        if (avatarCollection == null)
            avatarCollection = new int[] { };
        if (borderCollection == null)
            borderCollection = new List<int>();
        if (restaurantRewardRecords == null)
            restaurantRewardRecords = new int[] { };
    }
    public void UpdateLevelState()
    {
        if (!hasUpdateLevelStatePatch)
        {
            hasUpdateLevelStatePatch = true;
            List<LevelState> newLevelState = new List<LevelState>();
            Dictionary<string, int> dicLevelState = new Dictionary<string, int>();
            foreach (var levelstate in levelStates)
            {

                string lvlStateStr = levelstate.chapterId + "_" + levelstate.id;
                if (!dicLevelState.ContainsKey(lvlStateStr))
                {
                    dicLevelState.Add(lvlStateStr, 0);
                    newLevelState.Add(levelstate);
                }
            }
            levelStates = newLevelState.ToArray();
        }


    }
    public void UpdateKeyGrantedFeature()
    {
        List<LevelState> newLevelState = new List<LevelState>();
        Dictionary<string, int> dicLevelState = new Dictionary<string, int>();
        foreach (var levelstate in levelStates)
        {
            string lvlStateStr = levelstate.chapterId + "_" + levelstate.id;
            if (!dicLevelState.ContainsKey(lvlStateStr))
            {
                dicLevelState.Add(lvlStateStr, 0);
                if (levelstate.Level == 0)
                {
                    newLevelState.Add(levelstate);
                    continue;
                }
                if (!levelstate.hasKeyGranted)
                {
                    LevelData levelData = LevelDataController.Instance.GetLevelData(levelstate.chapterId, levelstate.id);
                    if (levelstate.Level >= levelData.key)
                        levelstate.hasKeyGranted = true;
                }
                newLevelState.Add(levelstate);
            }
        }
        levelStates = newLevelState.ToArray();
        hasUpdateLevelStatePatch = true;
        hasKeyGrantedFeatureUpdated = true;
    }
    public CertificateData GetCertificateDataAtRes(int chapter)
    {
        for (int i = 0; i < certificateDatas.Count; i++)
        {
            if (certificateDatas[i].ResId == chapter)
                return certificateDatas[i];
        }
        var new_cer = new CertificateData(chapter);
        if (chapter < highestRestaurant)
        {
            new_cer.RewardRecord = new int[] { 1, 1, 1 };
            new_cer.IsCompletedCertificate = true;
        }
        certificateDatas.Add(new_cer);
        return certificateDatas[certificateDatas.Count - 1];
    }
    public LevelRewardProgressData GetLevelRewardProgress(int chapter)
    {
        if (levelRewardProgress == null) levelRewardProgress = new List<LevelRewardProgressData>();
        for (int i = 0; i < levelRewardProgress.Count; i++)
        {
            if (levelRewardProgress[i].chapterId == chapter) return levelRewardProgress[i];
        }
        LevelRewardProgressData data = new LevelRewardProgressData(chapter);
        levelRewardProgress.Add(data);
        return levelRewardProgress[levelRewardProgress.Count - 1];
    }
    public void AddLevelRewardProgress(int chapter, int milestone)
    {
        for (int i = 0; i < levelRewardProgress.Count; i++)
        {
            if (levelRewardProgress[i].chapterId == chapter) levelRewardProgress[i].rewardProgress[milestone - 1] = 1;
        }
    }
    public GameData(ItemsData itemsData)
    {
        xp = 0;
        gold = 0;
        ruby = 0;
        energy = 5;
        pbLevel = 1;
        pbRuby = 0;
        IceCreamTimeDuration = 0;
        canShowZoneOffer = 0;
        energyTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        IceCreamTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        videoRewardTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        specialWeekendTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        bonusSundayTimeStamp = DataController.ConvertToUnixTime(System.DateTime.UtcNow);
        itemStates = new ItemState[0];
        machineStates = new MachineState[0];
        ingredientStates = new IngredientState[0];
        List<ItemState> tmpItemStates = new List<ItemState>();
        foreach (ItemData data in itemsData.items)
            tmpItemStates.Add(new ItemState(data.id));
        itemStates = tmpItemStates.ToArray();
        levelStates = new LevelState[0];
        battlePassData = new BattlePassData();
        achivementDatas = new List<AchivementDatas>(/*restaurantDatas.Length*/);
        profileDatas = new ProfileData(totalWin, totalWin, totalLike, totalServedFood, totalPlayedGame, 1, 0, 1, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow), "136886", "", "00bb30");
        //achivementDatas = AchivementController.Instance.GetDefaultAchievement();
        gameResultRecords = new int[] { -1, -1, -1, -1, -1 };
        customerCollection = new int[] { };
        avatarCollection = new int[] { };
        borderCollection = new List<int>();
        restaurantRewardRecords = new int[] { };
    }
}
[System.Serializable]
public class ItemState
{
    public int id = 100000;
    public int quantity = 0;
    public bool hasClaimFree = false;
    public ItemState(int itemId)
    {
        this.id = itemId;
    }
    public ItemState(int itemId, int quantity)
    {
        this.id = itemId;
        this.quantity = quantity;
    }
    public bool HasClaimFree
    {
        get { return hasClaimFree; }
        set { hasClaimFree = value; }
    }
}
[System.Serializable]
public class MachineState
{
    public int id;
    public int level;
    public MachineState(int machineId, int machineLevel)
    {
        id = machineId;
        level = machineLevel;
    }
    public int Level
    {
        get { return level; }
        set { level = UnityEngine.Mathf.Min(3, value); }
    }
}
[System.Serializable]
public class IngredientState
{
    public int id;
    public int level = 0;
    public double timeStamp = 0;
    public int Level
    {
        get { return level; }
        set { level = UnityEngine.Mathf.Min(3, value); }
    }
    public IngredientState(int ingredientId, int ingredientLevel)
    {
        id = ingredientId;
        level = ingredientLevel;
        timeStamp = 0;
    }
}
[System.Serializable]
public class UpgradeIngredientProcess
{
    public int id;
    public double timeStamp = 0;
    public bool hasCompleteUpgradeInMenu;
    public bool hasCompleteUpgradeInGame;
    public bool hasUsedX2Speed;
    public UpgradeIngredientProcess(int ingredientId, double time)
    {
        id = ingredientId;
        timeStamp = time;
        hasCompleteUpgradeInMenu = false;
        hasCompleteUpgradeInGame = false;
        hasUsedX2Speed = false;
    }
    public void Refresh()
    {
        timeStamp = 0;
        hasCompleteUpgradeInMenu = false;
        hasCompleteUpgradeInGame = false;
        hasUsedX2Speed = false;
    }
}
[System.Serializable]
public class UpgradeMachineProcess
{
    public int id;
    public int level = 0;
    public double timeStamp = 0;
    public bool hasCompleteUpgradeInMenu;
    public bool hasCompleteUpgradeInGame;
    public bool hasUsedX2Speed;
    public UpgradeMachineProcess(int machineId, double time)
    {
        id = machineId;
        timeStamp = time;
        hasCompleteUpgradeInMenu = false;
        hasCompleteUpgradeInGame = false;
        hasUsedX2Speed = false;
    }
    public void Refresh()
    {
        timeStamp = 0;
        hasCompleteUpgradeInMenu = false;
        hasCompleteUpgradeInGame = false;
        hasUsedX2Speed = false;
    }
}
[System.Serializable]
public class LevelRewardProgressData
{
    public int chapterId;
    public int[] rewardProgress;
    public LevelRewardProgressData(int chapter)
    {
        chapterId = chapter;
        rewardProgress = new int[5] { 0, 0, 0, 0, 0 };
    }
}

[System.Serializable]
public class LevelState
{
    public int chapterId;
    public int id;
    public int level;
    public bool hasKeyGranted;
    public LevelState(int chapter, int id, bool hasKeyGranted)
    {
        chapterId = chapter;
        this.id = id;
        level = 0;
        this.hasKeyGranted = hasKeyGranted;
    }
    public int Level
    {
        get { return level; }
        set
        {
            level = UnityEngine.Mathf.Min(3, value);
        }
    }
    public bool HasKeyGranted
    {
        get
        {
            return hasKeyGranted;
        }
        set
        {
            hasKeyGranted = value;
        }
    }
}
[System.Serializable]
public class VersionApp
{
    public string androidVersion = "";
    public string iosVersion = "";
}
[System.Serializable]
public class SpecialWeekendData
{
    public int saleOffPercent;
    public bool hasBought;
}
