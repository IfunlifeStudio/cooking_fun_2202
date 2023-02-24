using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BattlePassData
{
    public int BpLevel;
    public int BpExp;
    public bool hasUnlockVIP, hasUnlockVIP2022;
    //public int BpLevel
    //{
    //    get { return bpLevel; }
    //    set
    //    {
    //        bpLevel = Mathf.Min(value, BP_MAX_LEVEL);
    //    }
    //}
    //public int BpExp
    //{
    //    get { return bpExp; }
    //    set
    //    {
    //        bpExp = Mathf.Max(value, 0);
    //    }
    //}
    public List<BattlePassLevelState> bpLevelStates;
    public BattlePassData()
    {
        BpLevel = 1;
        hasUnlockVIP = false;
        hasUnlockVIP2022 = false;
        bpLevelStates = new List<BattlePassLevelState>();
    }
    public void AddBpLevelState(int level)
    {
        bpLevelStates.Add(new BattlePassLevelState(level));
    }
    public void ClaimedBpFreeItem(int level)
    {
        GetBpLevelState(level).hasClaimedFree = true;
    }
    public void ClaimBpVipItem(int level)
    {
        GetBpLevelState(level).hasClaimedVip = true;
    }
    public BattlePassLevelState GetBpLevelState(int level)
    {
        for (int i = 0; i < bpLevelStates.Count; i++)
        {
            if (bpLevelStates[i].Level == level)
                return bpLevelStates[i];
        }
        AddBpLevelState(level);
        return bpLevelStates[bpLevelStates.Count - 1];
    }
}

[Serializable]
public class BattlePassLevelState
{
    public int Level;
    public bool hasClaimedFree;
    public bool hasClaimedVip;
    public BattlePassLevelState(int level)
    {
        Level = level;
        hasClaimedFree = false;
        hasClaimedVip = false;
    }
}
[Serializable]
public class BattlePassLevelData
{
    public BattlePassElementData[] BattlePassElementDatas;
    public BattlePassElementData GetBpElementData(int level)
    {
        for (int i = 0; i < BattlePassElementDatas.Length; i++)
        {
            if (level == BattlePassElementDatas[i].Level)
                return BattlePassElementDatas[i];
        }
        return new BattlePassElementData();
    }
}
[Serializable]
public class BattlePassElementData
{
    public int Level;
    public int Exp;
    public BattlePassItemData FreeItem;
    public BattlePassItemData VipItem;
    public BattlePassElementData()
    {
        Exp = 9999;
    }
}
[Serializable]
public class BattlePassItemData
{
    public int ItemId;
    public int ItemQuantity;
}
