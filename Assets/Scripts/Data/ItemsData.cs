using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Anti_OverCook = 100000,
    Double_Coin = 110000,
    Instant_Cook = 120000,
    Pudding = 101000,
    Add_Customers = 100100,
    Add_Seconds = 100200,
    Second_Chance = 100300,
    IceCream = 102000,
    AutoServe = 130000
}
[System.Serializable]
public class ItemsData
{
    public ItemData[] items = new ItemData[0];
    public int GetRandomItemId()
    {
        List<int> ids = new List<int>();
        foreach (var data in items)
        {
            if (data.IsUnlock())
                ids.Add(data.id);
        }
        return ids[Random.Range(0, ids.Count)];
    }
    public ItemData GetItemDataById(int itemId)
    {
        foreach (ItemData item in items)
            if (item.id == itemId)
                return item;
        return null;
    }
    public string GetItemName(int itemId)
    {
        foreach (ItemData item in items)
            if (item.id == itemId)
                return item.itemName;
        return "";
    }
    public bool IsItemUnlock(int itemId)
    {
        foreach (ItemData item in items)
            if (item.id == itemId)
                return item.IsUnlock();
        return false;
    }
    public string GetItemUnlockMessage(int itemId)
    {
        foreach (ItemData item in items)
            if (item.id == itemId)
                return item.GetItemUnlockMessage();
        return null;
    }
    public int GetItemCost(int itemId)
    {
        foreach (ItemData item in items)
            if (item.id == itemId)
                return item.cost;
        return -1;
    }
}
[System.Serializable]
public class ItemData
{
    public int id;
    public string itemName;
    public string unlock;//chapter_level
    public int cost;
    public int unitPrice = 0;
    public bool IsUnlock()
    {
        bool result = false;
        string[] unlocks = unlock.Split('_');
        int chapter = int.Parse(unlocks[0]);
        int level = int.Parse(unlocks[1]);
        if (level == 1)
            result = true;
        else if (chapter == 1 && level == 12)
        {
            if (DataController.Instance.GetLevelState(1, 11) >= 2)
                return true;
            else
                return false;
        }
        else if (DataController.Instance.GetLevelState(chapter, level - 1) > 0)
            result = true;
        return result; ;
    }
    public int[] GetUnlockCondition()
    {
        string[] unlocks = unlock.Split('_');
        return new int[] { int.Parse(unlocks[0]), int.Parse(unlocks[1]) };
    }
    public string GetItemUnlockMessage()
    {
        string[] unlocks = unlock.Split('_');
        string resName = Lean.Localization.LeanLocalization.GetTranslationText("res_name_" + int.Parse(unlocks[0]));
        return string.Format(Lean.Localization.LeanLocalization.GetTranslationText("item_panel_message"), int.Parse(unlocks[1]) - 1, resName);
    }
}
