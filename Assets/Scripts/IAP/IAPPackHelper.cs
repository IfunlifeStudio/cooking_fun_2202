using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Packs
{
    public Pack[] packs;
    public Pack GetPackById(string packId)
    {
        foreach (Pack pack in packs)
            if (pack.id == packId)
                return pack;
        return null;
    }
    public float GetPackPrice(string packId)
    {
        foreach (var pack in packs)
            if (pack.id == packId)
                return pack.price;
        return 0;
    }
    public float GetPackLifeTime(string packId)
    {
        foreach (var pack in packs)
            if (pack.id == packId)
                return pack.lifeTime;
        return 0;
    }
}
[System.Serializable]
public class Pack
{
    public string id;
    public float price;
    public float lifeTime;//total live time in second
    public int rubyAmount, unlimitedEnergyTime;
    public int[] itemIds, itemAmounts;
}
[System.Serializable]
public class GiftPack
{
    public int RubyAmount, GoldAmount, UnlimitedEnergyTime;
    public int[] ItemId, ItemAmount;
}
[System.Serializable]
public class GiftCodeRespone
{
    public int code;
    public GiftCodeData result;
}
[System.Serializable]
public class GiftCodeData
{
    public int giftType;
    public GiftPack gift;
    public string message;
}

public class IAPPackHelper
{
    public const string IAP_VALUE = "user_iap_value";
    public static float GetPackPrice(string packId)
    {
        string packsData = FirebaseServiceController.Instance.GetIAPPacksData();
        Packs packs = JsonUtility.FromJson<Packs>(packsData);
        return packs.GetPackPrice(packId);
    }
    public static float GetPackLifeTime(string packId)
    {
        string packsData = FirebaseServiceController.Instance.GetIAPPacksData();
        Packs packs = JsonUtility.FromJson<Packs>(packsData);
        return packs.GetPackLifeTime(packId);
    }
    public static float GetTrackingPackPrice(string packId)
    {
        string packsData = Resources.Load<TextAsset>("iap_packs_tracking").text;
        Packs packs = JsonUtility.FromJson<Packs>(packsData);
        return packs.GetPackPrice(packId);
    }
    public static string GetContentPack(string packId)
    {
        string packsData = Resources.Load<TextAsset>("iap_packs_tracking").text;
        Packs packs = JsonUtility.FromJson<Packs>(packsData);
        var pack = packs.GetPackById(packId);
        if (pack == null) return "";
        return JsonUtility.ToJson(pack);
    }
}