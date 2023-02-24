using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class IngredientData
{
    public int id;
    public string ingredientName;
    public int level;
    public int unlock;
    public int[] upgradeTimes;
    public int UpgradeTime
    {
        get
        {
            int level = DataController.Instance.GetIngredientLevel(id);
            int nextLevel = Mathf.Min(level, 2);
            return upgradeTimes[nextLevel];
        }
    }
    public bool IsUnlocked(int highestLevel)
    {
        return highestLevel >= unlock;
    }
    public int[] golds;
    public int Gold()
    {
        int level = DataController.Instance.GetIngredientLevel(id);
        return golds[level - 1];
    }
    public int GetUpgradedGoldValue()
    {
        int level = DataController.Instance.GetIngredientLevel(id);
        int nextLevel = Mathf.Min(level, 2);
        return golds[nextLevel];
    }
    public int[] upgradePrices;
    public int UpgradePrice()
    {
        int level = DataController.Instance.GetIngredientLevel(id);
        if (level > 2) return 1000000;
        return upgradePrices[level];
    }
    public int[] rewardExps;
    public int RewardExp()
    {
        int level = DataController.Instance.GetIngredientLevel(id);
        return rewardExps[level];
    }
    public IngredientData()
    {

    }
}
