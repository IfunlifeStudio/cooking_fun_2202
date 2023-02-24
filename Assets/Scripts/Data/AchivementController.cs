using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
public class AchivementController : MonoBehaviour
{
    private static AchivementController instance = null;
    public static AchivementController Instance { get { return instance; } }
    [SerializeField] private AchivementDataDefaults achivementDataDefaults;
    //private AchivementDatas GetAchivementDatas(int achievementID)
    //{
    //    return DataController.Instance.ResAchievementData(achievementID);
    //}
    //private List<AchivementDatas> AchievementDatasList
    //{
    //    get { return DataController.Instance.AchievementList; }
    //}
    // Start is called before the first frame update
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void LoadDefaultAchievement()
    {
        //string achivementsData = FirebaseServiceController.Instance.GetAchivement();
        //achivementDataDefaults = JsonUtility.FromJson<AchivementDataDefaults>(achivementsData);
    }
    //public AchivementDatas GetDefaultAchievement()
    //{
    //    AchivementDatas achivementDatas = new AchivementDatas();
    //    for (int i = 0; i < achivementDataDefaults.Achivement.Count; i++)
    //        achivementDatas.ListAchivementData.Add(new AchivementData(achivementDataDefaults.Achivement[i].id));
    //    return achivementDatas;
    //}
    public int GetAchivementCount()
    {
        return 0;
        //return AchivementDatas.ListAchivementData.Count;
    }
    public bool HasAchievement()
    {
        if (PlayerPrefs.GetInt("has_certificate", 0) == 1)
            return true;
        for (int i = 0; i < 7; i++)
        {
            if (GetAchievement(i) != "")
            {
                return true;
            }
        }
        return false;
    }
    private string GetAchievement(int id)
    {
        switch (id)
        {
            case 0:
                if (DataController.Instance.HighestRestaurant > 1)
                    return GetWinrateCerti(DataController.Instance.HighestRestaurant - 1);
                return "";
            case 1:
                return GetComboCerti();
            case 2:
                if (DataController.Instance.HighestRestaurant > 1)
                    return GetUnlockResCerti(DataController.Instance.HighestRestaurant);
                return "";
            case 3:
                return GetFlashHandCerti();
            case 4:
                if (DataController.Instance.GetGameData().userIapValue >= 1000)
                    return "best_friend";
                return "";
            case 5:
                if (PlayerPrefs.GetInt("has_buy_starter_pack", 0) != 0)
                    return "play_boy";
                return "";
            case 6:
                PlayerPrefs.GetInt("ruby_spent", 0);
                float rb_spent_time = PlayerPrefs.GetFloat("ruby_spent_time", 0);
                if (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - rb_spent_time < 604800 && PlayerPrefs.GetInt("ruby_spent", 0) >= 1000)
                    return "rick_kid";
                return "";
        }
        return "";
    }

    //public void AddAchivementProgress(int resID, AchivementID achivementID, int count = 1)
    //{
    //    var achievementDatas = GetAchivementDatas((int)achivementID);

    //    for (int i = 0; i < achievementDatas.ListResAchivementData.Count; i++)
    //    {
    //        if (achievementDatas.ListResAchivementData[i].resId == resID)
    //        {
    //            if (achievementDatas.ListResAchivementData[i].idDone == 0)
    //            {
    //                if ((int)achivementID == 0 || (int)achivementID == 2)
    //                    achievementDatas.ListResAchivementData[i].CurrentValue = count;
    //                else
    //                    achievementDatas.ListResAchivementData[i].CurrentValue += count;
    //            }
    //            return;
    //        }
    //    }
    //    achievementDatas.ListResAchivementData.Add(new ResAchivementData(resID));
    //    if ((int)achivementID == 0 || (int)achivementID == 2)
    //        achievementDatas.ListResAchivementData[achievementDatas.ListResAchivementData.Count - 1].CurrentValue = count;
    //    else
    //        achievementDatas.ListResAchivementData[achievementDatas.ListResAchivementData.Count - 1].CurrentValue += count;
    //}

    //public int GetAllAchievementValue(AchivementID achivementID)
    //{
    //    int result = 0;
    //    for (int i = 0; i < AchievementDatasList.Count; i++)
    //    {
    //        if (AchievementDatasList[i].achievementID == (int)achivementID)
    //        {
    //            var resAchievement = AchievementDatasList[i].ListResAchivementData;
    //            for (int j = 0; j < resAchievement.Count; j++)
    //                result += resAchievement[j].CurrentValue;
    //        }
    //    }
    //    //if ((int)(achivementID) == 0) result /= AchievementDatasList.Count;
    //    return result;
    //}
    //public int GetCurrentAchievementValue(int resID, AchivementID achivementID)
    //{
    //    return GetAchievementDataAtRes(resID, achivementID).CurrentValue;
    //}
    //public string GetAchievementColorWithResID(int resID, AchivementID achivementID)
    //{
    //    var crrAchieve = GetCurrentAchievementValue(resID, achivementID);
    //    var achieveDefault = GetAchivementDataDefault(achivementID);
    //    if (achieveDefault != null)
    //    {
    //        int tmpIdDone = 0;
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (achieveDefault.Target[i] <= crrAchieve)
    //                tmpIdDone++;
    //        }
    //        if (tmpIdDone == 0) return " ";
    //        return achieveDefault.Color[tmpIdDone - 1];
    //    }
    //    return " ";
    //}
    //public string GetAchievementNameWithResID(int resID, AchivementID achivementID)
    //{
    //    var crrAchieve = GetCurrentAchievementValue(resID, achivementID);
    //    var achieveDefault = GetAchivementDataDefault(achivementID);
    //    if (achieveDefault != null)
    //    {
    //        int tmpIdDone = 0;
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (achieveDefault.Target[i] <= crrAchieve)
    //                tmpIdDone++;
    //        }
    //        if (tmpIdDone == 0) return " ";
    //        return achieveDefault.AchivementName[tmpIdDone - 1];
    //    }
    //    return " ";
    //}
    //public AchivementDataDefault GetAchivementDataDefault(AchivementID achivementID)
    //{
    //    for (int i = 0; i < achivementDataDefaults.Achivement.Count; i++)
    //    {
    //        if (achivementDataDefaults.Achivement[i].id == achivementID.ToString())
    //            return achivementDataDefaults.Achivement[i];
    //    }
    //    return null;
    //}
    //public ResAchivementData GetAchievementDataAtRes(int resID, AchivementID achivementID)
    //{
    //    var achi_Datas = GetAchivementDatas((int)achivementID);
    //    for (int i = 0; i < achi_Datas.ListResAchivementData.Count; i++)
    //    {
    //        if (achi_Datas.ListResAchivementData[i].resId == resID)
    //            return achi_Datas.ListResAchivementData[i];
    //    }
    //    achi_Datas.ListResAchivementData.Add(new ResAchivementData(resID));
    //    return achi_Datas.ListResAchivementData[achi_Datas.ListResAchivementData.Count - 1];
    //}

    #region NewCertificateLogic

    public string GetWinrateCerti(int resID, bool isGetName = true)
    {
        int winrate = DataController.Instance.GetGameData().GetCertificateDataAtRes(resID).WinRate;
        if (winrate >= 90)
            return isGetName ? "king_chef" : "ffff00";
        else if (winrate >= 80)
            return isGetName ? "master_chef" : "ab38ed";
        else if (winrate >= 70)
            return isGetName ? "talent_chef" : "1d7ee1";
        else
            return isGetName ? "protential_chef" : "04b100";
    }
    public string GetComboCerti(bool isGetName = true)
    {
        int combo = 0;
        for (int i = 0; i < DataController.Instance.GetGameData().certificateDatas.Count; i++)
        {
            combo += DataController.Instance.GetGameData().certificateDatas[i].Combo;
        }
        if (combo >= 2000)
            return isGetName ? "combo_beginner" : "ffff00";
        else if (combo >= 1000)
            return isGetName ? "combo_expert" : "ab38ed";
        else if (combo >= 500)
            return isGetName ? "combo_master" : "1d7ee1";
        else if (combo >= 100)
            return isGetName ? "combo_legend" : "04b100";
        return "";
    }
    public string GetUnlockResCerti(int resID, bool isGetName = true)
    {
        if (resID >= 13)
            return isGetName ? "cooking_myth" : "ffff00";
        else if (resID >= 8)
            return isGetName ? "madness" : "ab38ed";
        else if (resID >= 4)
            return isGetName ? "craze" : "1d7ee1";
        else if (resID >= 2)
            return isGetName ? "lover" : "04b100";
        return "";
    }
    public string GetFlashHandCerti()
    {
        int flashHand = 0;
        for (int i = 0; i < DataController.Instance.GetGameData().certificateDatas.Count; i++)
        {
            flashHand += DataController.Instance.GetGameData().certificateDatas[i].Combo;
        }
        if (flashHand >= 50)
            return "flash_hand";
        return "";
    }
    public void AddCertificateValue(int chapter, int cer_combo, int cer_like, int cer_food)
    {
        var certiData = DataController.Instance.GetGameData().GetCertificateDataAtRes(chapter);
        certiData.Combo += cer_combo;
        certiData.Like += cer_like;
        certiData.Food += cer_food;
        certiData.WinGame++;
        float winRate;
        try
        {
            winRate = ((float)certiData.WinGame / (float)certiData.PlayedGame) * 100;
            if (winRate > 0)
                certiData.WinRate = (int)winRate;
        }
        catch { Debug.LogError("not divide"); }
    }
    public void OnGameStart(int resID)
    {
        var certiData = DataController.Instance.GetGameData().GetCertificateDataAtRes(resID);
        certiData.PlayedGame++;
        float winRate = 0;
        try
        {
            winRate = ((float)certiData.WinGame / (float)certiData.PlayedGame) * 100;
            if (winRate > 0)
                certiData.WinRate = (int)winRate;
        }
        catch { Debug.LogError("not divide"); }
    }

    public void AddTimeAchievement(int resID)
    {
        DataController.Instance.GetGameData().GetCertificateDataAtRes(resID).CompletedTime = DateTime.Now.Date.ToString("MM/dd/yyyy");
        //var achievementDatas = GetAchivementDatas(0);

        //for (int i = 0; i < achievementDatas.ListResAchivementData.Count; i++)
        //{
        //    if (achievementDatas.ListResAchivementData[i].resId == resID)
        //    {
        //        achievementDatas.ListResAchivementData[i].time = DateTime.Now.Date.ToString("MM/dd/yyyy");
        //    }
        //}
    }
    //public void OnCompleteTier(int resID, int tier)
    //{
    //    DataController.Instance.GetGameData().GetCertificateDataAtRes(resID).RewardRecord[tier] = 1;
    //}
    #endregion


}
[System.Serializable]
public class AchivementDataDefault
{
    public string id;
    public string[] AchivementName;
    public int[] Target;
    public string[] Color;
}
[System.Serializable]
public class AchivementDataDefaults
{
    public List<AchivementDataDefault> Achivement;
}
[System.Serializable]
public class ResAchivementData
{
    public int resId;
    public int CurrentValue;
    public int idDone;
    public string time;
    public ResAchivementData(int id)
    {
        this.resId = id;
        CurrentValue = 0;
        idDone = 0;
        time = DateTime.Now.Date.ToString("MM/dd/yyyy");
    }
}

[System.Serializable]
public class AchivementDatas
{
    [SerializeField]
    public int achievementID;
    public List<ResAchivementData> ListResAchivementData;
    public int hasCompleteId;
    public AchivementDatas(int achievementId)
    {
        this.achievementID = achievementId;
        ListResAchivementData = new List<ResAchivementData>();
        hasCompleteId = 0;
    }
}

public enum AchivementID
{
    Achivement_1, // win rate
    Achivement_2, // combo 5
    Achivement_3, // unlockres
    Achivement_4, // serve food
    Achivement_5, // like
    Achivement_6, // flash hand
    Achivement_7, // win game
    Achivement_0  // played_game
}
[Serializable]
public class CertificateData
{
    [SerializeField] int resId;
    [SerializeField] int winRate;
    [SerializeField] int combo;
    [SerializeField] int like;
    [SerializeField] int food;
    [SerializeField] int winGame;
    [SerializeField] int playedGame;
    [SerializeField] int flashHand;
    [SerializeField] string completedTime;
    [SerializeField] int[] rewardRecord = new int[3];
    [SerializeField] bool isCompletedCertificate;
    [SerializeField] int idDone;
    public CertificateData(int chapter) { resId = chapter; }

    public CertificateData(int resId, int winRate, int combo, int like, int food, int winGame, int playedGame, int flashHand, string completedTime, int[] rewardRecord, int idDone)
    {
        this.resId = resId;
        this.winRate = winRate;
        this.combo = combo;
        this.like = like;
        this.food = food;
        this.winGame = winGame;
        this.playedGame = playedGame;
        this.flashHand = flashHand;
        this.completedTime = completedTime;
        this.rewardRecord = rewardRecord;
        this.idDone = idDone;
        this.isCompletedCertificate = false;
    }
    public void SetRewardRecord(int index, int value)
    {
        rewardRecord[index] = value;
    }
    public int ResId { get { return resId; } set { resId = value; } }
    public int WinRate { get { return winRate; } set { winRate = value; } }
    public int Combo { get { return combo; } set { combo = value; } }
    public int Like { get { return like; } set { like = value; } }
    public int Food { get { return food; } set { food = value; } }
    public int WinGame { get { return winGame; } set { winGame = value; } }
    public int PlayedGame { get { return playedGame; } set { playedGame = value; } }
    public int FlashHand { get { return flashHand; } set { flashHand = value; } }
    public string CompletedTime { get { return completedTime; } set { completedTime = value; } }
    public int[] RewardRecord { get { return rewardRecord; } set { rewardRecord = value; } }
    public int IdDone { get { return idDone; } set { idDone = value; } }
    public bool IsCompletedCertificate { get { return isCompletedCertificate; } set { isCompletedCertificate = value; } }
}