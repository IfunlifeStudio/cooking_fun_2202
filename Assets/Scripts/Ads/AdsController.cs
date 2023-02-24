using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
public class AdsController : MonoBehaviour
{
    private static AdsController instance;
    public static AdsController Instance
    {
        get { return instance; }
    }
    [SerializeField] private AdsSetting[] adsSettings;
    private AdsDataCollection adsDataCollection;
    private string dataPath = "";
    public AdsFullCondition AdsFullCondition;
    public int interstitialDisplayedCount;
    int AdsFullConditionWinCount, AdsFullConditionFailCount, AdsFullConditionPauseCount;
    private float timeStamp;
    bool isPassLevel13 = false;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            dataPath = Path.Combine(Application.persistentDataPath, "ads_data.dat");
            LoadAdsData();
            DontDestroyOnLoad(gameObject);
            timeStamp = -1000;
            interstitialDisplayedCount = 0;
        }
        else Destroy(gameObject);
    }
    public void InitAds()
    {
        adsSettings = PlayerClassifyController.Instance.GetAdsSettings().adsSettings;
        AdsFullCondition = PlayerClassifyController.Instance.GetAdsFullCondition();
        adsDataCollection.UpdateAdsCollection(adsSettings);
    }
    public void LoadAdsData()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string data;
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                try
                {
                    data = (string)binaryFormatter.Deserialize(fileStream);
                    adsDataCollection = JsonUtility.FromJson<AdsDataCollection>(data);
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
    }
    public void SaveAdsData()
    {
        string origin = JsonUtility.ToJson(adsDataCollection);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, origin);
        }
    }
    private void ResetData()
    {
        adsDataCollection = new AdsDataCollection(adsSettings);
        SaveAdsData();
    }
    public bool CanShowAds(string adsName)
    {
        return adsDataCollection.CanShowAds(adsName) && !(Application.internetReachability == NetworkReachability.NotReachable);
    }
    public void OnWatchAdsCompleted(string adsName)
    {
        adsDataCollection.OnWatchAdsCompleted(adsName);
        SaveAdsData();
    }
    public void ResetAdsCount(string adsName)
    {
        adsDataCollection.ResetAdsCount(adsName);
    }
    public int GetAdsCooldown(string adsName)
    {
        return adsDataCollection.GetAdsCooldown(adsName);
    }
    public AdsSetting GetAdsSettingByName(string name)
    {
        AdsSetting result = new AdsSetting();
        foreach (var adsSetting in adsSettings)
            if (adsSetting.adsName == name)
                result = adsSetting;
        return result;
    }
    public bool IsRewardVideoAdsReady()
    {
        if (isPassLevel13) return true;
        else
            isPassLevel13 = (DataController.Instance.GetLevelState(1, 3) > 0);
        return isPassLevel13;
    }
    public void ShowVideoReward(System.Action onUserEarnedReward, System.Action onAdClosed)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;
        if (IronsourceAdsController.Instance.IsRewardedVideoAdReady())
            IronsourceAdsController.Instance.OnShowRewardedVideoAd(onUserEarnedReward, onAdClosed);
        else
            CSCAdsController.Instance.ShowVideoAds(onUserEarnedReward, onAdClosed);
        AdsFullConditionWinCount = 0;
        AdsFullConditionFailCount = 0;
        APIController.Instance.LogEventRewardShow();
    }
    public bool CanShowIntersAds(int chap, int Level, int type)
    {
        //return false;
        if (DataController.Instance.RemoveAds == 1 || Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        //if (!CanShowAds("no_interstitial")) return false;//disable interstitial ads for one day if has watch video ads
        float totalMoneyPay = PlayerPrefs.GetFloat("user_iap_value", 0);
        if (totalMoneyPay >= PlayerClassifyController.Instance.playerClassifyData.moneyThreshold)
        {
            return false;
        }
        if (chap > AdsFullCondition.Chap)
        {
        }
        else
        {
            if (Level < AdsFullCondition.Level)
                return false;
        }
        if (type == 1)
        {
            if (AdsFullConditionWinCount < AdsFullCondition.WinCount)
            {
                AdsFullConditionWinCount++;
                return false;
            }
            else
                AdsFullConditionWinCount = 0;
        }
        else if (type == 2)
        {
            if (AdsFullConditionFailCount < AdsFullCondition.FailCount)
            {
                AdsFullConditionFailCount++;
                return false;
            }
            else
                AdsFullConditionFailCount = 0;
        }
        else if (type == 0)
        {
            if (AdsFullConditionPauseCount < AdsFullCondition.PauseCount || AdsFullCondition.PauseCount == -1)
            {
                AdsFullConditionPauseCount++;
                return false;
            }
            else
                AdsFullConditionPauseCount = 0;
        }
        if (Time.time - timeStamp > AdsFullCondition.TimeCheck)
            return true;
        else
            return false;
    }
    public void ShowInterstitial(Action onIntersAdsClosed)
    {
        if (IronsourceAdsController.Instance.IsInterstialAdReady())
            IronsourceAdsController.Instance.OnShowInterstialAd(onIntersAdsClosed);
        else
            CSCAdsController.Instance.ShowInterstitial(onIntersAdsClosed);
        interstitialDisplayedCount++;
        ResetTimeStamp();
    }
    public void ResetTimeStamp()
    {
        timeStamp = Time.time;
    }
}
[Serializable]
public class AdsFullCondition
{
    public int Chap, Level;
    public int PauseCount, FailCount, WinCount;
    public int TimeCheck;
}
[Serializable]
public class AdsSettings
{
    public AdsSetting[] adsSettings;
}
[Serializable]
public class AdsSetting
{
    public string adsName = "default";
    public int adsQuota = 1;
    public int adsCooldown = 1800;//cooldown in seconds
}
[Serializable]
public class AdsData
{
    public string adsName = "";
    public int adsCount = 0;
    public double adsTimeStamp = 0;//store timestamp when last time open ads
    public AdsData(string name)
    {
        adsName = name;
        adsCount = 0;
        adsTimeStamp = 0;
    }
    public bool CanShowAds()
    {
        bool result = false;
        AdsSetting adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        var tmpTimestamp = DataController.ConvertToUnixTime(DateTime.Now);
        if (tmpTimestamp - adsTimeStamp > 64800)
            adsCount = 0;//reset ads count after 24h cooldown
        if (adsCount < adsSetting.adsQuota && tmpTimestamp - adsTimeStamp > adsSetting.adsCooldown)
            result = true;
        return result;
    }
    public void OnShowAds()
    {
        adsCount++;
        adsTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
    }
    public int GetAdsCooldown()
    {
        AdsSetting adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        var remainTimeInSecond = adsTimeStamp + adsSetting.adsCooldown - DataController.ConvertToUnixTime(DateTime.Now);
        return Mathf.Max(1, (int)(remainTimeInSecond / 60));
    }
}
[Serializable]
public class AdsDataCollection
{
    public AdsData[] adsCollection;
    public AdsDataCollection(AdsSetting[] settings)
    {
        List<AdsData> tmp = new List<AdsData>();
        foreach (var setting in settings)
            tmp.Add(new AdsData(setting.adsName));
        adsCollection = tmp.ToArray();
    }
    public void UpdateAdsCollection(AdsSetting[] settings)
    {
        List<AdsData> tmpAdsData = new List<AdsData>();
        foreach (var setting in settings)
            tmpAdsData.Add(new AdsData(setting.adsName));
        if (tmpAdsData.Count > adsCollection.Length)
        {
            for (int i = 0; i < tmpAdsData.Count; i++)//update machine state
            {
                for (int j = 0; j < adsCollection.Length; j++)
                {
                    if (tmpAdsData[i].adsName == adsCollection[j].adsName)
                        tmpAdsData[i] = adsCollection[j];
                }
            }
            adsCollection = tmpAdsData.ToArray();
        }
    }
    public bool CanShowAds(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        return ads.CanShowAds();
    }
    public void OnWatchAdsCompleted(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        ads.OnShowAds();
    }
    public void ResetAdsCount(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        ads.adsCount = 0;
    }
    public int GetAdsCooldown(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        return ads.GetAdsCooldown();
    }
    private AdsData GetAdsDataByName(string name)
    {
        AdsData result = new AdsData("default");
        foreach (var ads in adsCollection)
            if (ads.adsName == name) result = ads;
        return result;
    }
}