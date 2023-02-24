using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RestaurantData
{
    public int id = 0;
    public string restaurantName = "Breakfast Cafe";
    public int conversTimeValue = 120;
    public int nowRubyTime = 60;
    public int nowAdsTime = 0;
    public string sceneName = "Restaurant1";
    public int keyRequired = 15;//total key require to open restaurant
    public string shopUpgradeProgress = "";//this is the recommend upgrade progress
    public int[] levelProgressLock;
    public int[] levelRewardProgress;
    public IngredientData[] ingredients;
    public MachineData[] machines;
    public RestaurantData()
    {
    }
    public int GetRecommendUpgradeId()
    {
        int id = -1;
        string[] upgradeElements = shopUpgradeProgress.Split('-');
        string[] datas;
        int tmpId = 0, level = 1;
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            datas = upgradeElements[i].Split('_');
            tmpId = int.Parse(datas[0]);
            level = int.Parse(datas[1]);
            bool foundId = false;
            if (IsMachineId(tmpId))
            {
                if (DataController.Instance.GetMachineLevel(tmpId) < level)
                    foundId = true;
            }
            else
            {
                if (DataController.Instance.GetIngredientLevel(tmpId) < level)
                    foundId = true;
            }
            if (foundId)
            {
                id = tmpId;
                break;
            }
        }
        return id;
    }
    public int GetRecommendIngredientUpgradeId()
    {
        int id = -1;
        string[] upgradeElements = shopUpgradeProgress.Split('-');
        string[] datas;
        int tmpId = 0, level = 1;
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            datas = upgradeElements[i].Split('_');
            tmpId = int.Parse(datas[0]);
            level = int.Parse(datas[1]);
            if (!IsMachineId(tmpId))
                if (DataController.Instance.GetIngredientLevel(tmpId) < level)
                {
                    id = tmpId;
                    break;
                }
        }
        return id;
    }
    public int GetRecommendMachineUpgradeId()
    {
        int id = -1;
        string[] upgradeElements = shopUpgradeProgress.Split('-');
        string[] datas;
        int tmpId = 0, level = 1;
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            datas = upgradeElements[i].Split('_');
            tmpId = int.Parse(datas[0]);
            level = int.Parse(datas[1]);
            if (IsMachineId(tmpId))
                if (DataController.Instance.GetMachineLevel(tmpId) < level)
                {
                    id = tmpId;
                    break;
                }
        }
        return id;
    }
    public bool IsMachineId(int id)//true if this is an machineid
    {
        foreach (var machine in machines)
            if (machine.id == id) return true;
        return false;
    }
    public string GetMachineName(int machineId)
    {
        return GetMachine(machineId).machineName;
    }
    public float GetWorkTime(int machineId)
    {
        return GetMachine(machineId).WorkTime();
    }
    public float GetNextLevelWorkTime(int machineId)
    {
        return GetMachine(machineId).NextLevelWorkTime();
    }
    public int GetMachineUpgradeCost(int machineId)
    {
        return GetMachine(machineId).UpgradePrice();
    }
    public int GetMachineUpgradeExp(int machineId)
    {
        return GetMachine(machineId).RewardExp();
    }
    public int GetMachineCount(int machineId)
    {
        return GetMachine(machineId).MachineCount();
    }
    public int GetNextLevelMachineCount(int machineId)
    {
        return GetMachine(machineId).NextLevelMachineCount();
    }
    public int GetMachineUpgradeTime(int machineId)
    {
        return GetMachine(machineId).UpgradeTime;
    }
    public MachineData GetMachine(int machineId)
    {
        MachineData machine = null;
        foreach (var tmp in machines)
        {
            if (tmp.id == machineId)
            {
                machine = tmp;
                break;
            }
        }
        return machine;
    }
    public string GetIngredientName(int ingredientId)
    {
        return GetIngredient(ingredientId).ingredientName;
    }
    public int GetCurrentIngredientValue(int ingredientId)
    {
        return GetIngredient(ingredientId).Gold();
    }
    public int GetUpgradeIngredientValue(int ingredientId)
    {
        return GetIngredient(ingredientId).GetUpgradedGoldValue();
    }
    public int GetIngredientUpgradeExp(int ingredientId)
    {
        return GetIngredient(ingredientId).RewardExp();
    }
    public int GetIngredientUpgradeCost(int ingredientId)
    {
        return GetIngredient(ingredientId).UpgradePrice();
    }
    public int GetIngredientUpgradeTime(int ingredientId)
    {
        return GetIngredient(ingredientId).UpgradeTime;
    }
    public IngredientData GetIngredient(int ingredientId)
    {
        IngredientData ingredient = null;
        foreach (var tmp in ingredients)
            if (tmp.id == ingredientId)
            {
                ingredient = tmp;
                break;
            }
        return ingredient;
    }
}
