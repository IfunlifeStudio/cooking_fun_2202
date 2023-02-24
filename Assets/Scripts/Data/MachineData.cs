using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MachineData
{
    public int id;
    public string machineName;
    public int level = 1;
    public int unlock = 1;
    public bool IsUnlocked(int highestLevel)
    {
        return highestLevel >= unlock;
    }
    public int[] upgradeTimes;
    public int UpgradeTime
    {
        get
        {
            int level = DataController.Instance.GetMachineLevel(id);
            int nextLevel = Mathf.Min(level, 2);
            return upgradeTimes[nextLevel];
        }
    }
    public int[] machineCounts = { 1, 2, 3 };
    public int MachineCount()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        return machineCounts[level - 1];
    }
    public int NextLevelMachineCount()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        int nextLevel = Mathf.Min(level, 2);
        return machineCounts[nextLevel];
    }
    public float[] workTimes;
    public float WorkTime()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        return workTimes[level - 1];
    }
    public float NextLevelWorkTime()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        int nextLevel = Mathf.Min(level, 2);
        return workTimes[nextLevel];
    }
    public int[] upgradePrices;
    public int UpgradePrice()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        if (level > 2) return 10000000;
        return upgradePrices[level];
    }
    public int[] rewardExps;
    public int RewardExp()
    {
        int level = DataController.Instance.GetMachineLevel(id);
        return rewardExps[level];
    }
}
