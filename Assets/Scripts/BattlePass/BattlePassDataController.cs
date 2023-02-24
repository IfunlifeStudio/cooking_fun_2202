using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassDataController : MonoBehaviour
{
    private static BattlePassDataController instance = null;
    public static BattlePassDataController Instance { get { return instance; } }
    public const int BP_MAX_LEVEL = 30;
    public const int TREE_MAX_LEVEL = 7;
    public static DateTime EndTime = new DateTime(2023, 3, 22);
    public BattlePassLevelData battlePassLevelData;
    private bool hasUnlockEvent, hasUnlockPoster, isFirstOpenPoster = true;

    public BattlePassData BattlePassData
    {
        get { return DataController.Instance.GetGameData().battlePassData; }
    }
    public bool IsNormalUser
    {
        get
        {
            if (BattlepassUser == 2 || BattlepassUser == 3)
                return true;
            else
                return false;
        }
    }
    private int BattlepassUser
    {
        get
        {
            int userType = PlayerPrefs.GetInt("battlepass_user", 0);
            if (userType == 0)
            {
                userType = PlayerClassifyController.Instance.GetPlayerClassifyLevel();
                PlayerPrefs.SetInt("battlepass_user", userType);
            }
            return userType;
        }
    }
    public bool HasUnlockEvent
    {
        set { hasUnlockEvent = value; }
        get
        {
            if (hasUnlockEvent) return true;
            else if (DataController.Instance.GetLevelState(2, 1) > 0)
            {
                hasUnlockEvent = true;
                return true;
            }
            return hasUnlockEvent;
        }
    }
    public bool HasUnlockPoster
    {
        set { hasUnlockPoster = value; }
        get
        {
            if (hasUnlockPoster) return true;
            else if (DataController.Instance.GetLevelState(1, 5) > 0)
            {
                hasUnlockPoster = true;
                return true;
            }
            return hasUnlockPoster;
        }
    }
    public bool HasFinishEvent
    {
        get
        {
            return (EndTime - DateTime.UtcNow).TotalSeconds < 0;
        }
    }
    public bool CanClaimExp
    {
        get
        {
            return !HasFinishEvent && HasUnlockEvent && CurrentBpLevel < 30;
        }
    }
    public bool IsFirstOpenPoster
    {
        set { isFirstOpenPoster = value; }
        get { return isFirstOpenPoster; }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            battlePassLevelData = JsonUtility.FromJson<BattlePassLevelData>(FirebaseServiceController.Instance.GetBattlePassLevelData());
            DontDestroyOnLoad(gameObject);
        }

    }
    public int CurrentBpLevel
    {
        set { BattlePassData.BpLevel = Mathf.Min(value, BP_MAX_LEVEL); }
        get { return BattlePassData.BpLevel; }
    }
    public int CurrentBpExp
    {
        set { BattlePassData.BpExp = Mathf.Max(value, 0); }
        get { return BattlePassData.BpExp; }
    }
    public bool HasUnlockVip
    {
        set { BattlePassData.hasUnlockVIP = value; }
        get { return BattlePassData.hasUnlockVIP; }
    }
    public bool HasUnlockVip2022
    {
        set { BattlePassData.hasUnlockVIP2022 = value; }
        get { return BattlePassData.hasUnlockVIP2022; }
    }
    public void ClaimFreeBpIttem(int level)
    {
        GetBattlePassLevelState(level).hasClaimedFree = true;
    }
    public BattlePassLevelState GetBattlePassLevelState(int level)
    {
        return BattlePassData.GetBpLevelState(level);
    }
    public void ClaimVipBpIttem(int level)
    {
        GetBattlePassLevelState(level).hasClaimedVip = true;
    }
    public BattlePassElementData GetBattlePassElementData(int level)
    {
        return battlePassLevelData.GetBpElementData(level);
    }
}
