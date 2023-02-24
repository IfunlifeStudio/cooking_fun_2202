using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Firebase.Analytics;

public class APIController : MonoBehaviour
{
    private static APIController instance;
    public static APIController Instance { get { return instance; } }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }

    public bool CanConnetInternet()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            return true;
        }
        else
        {
            UIController.Instance.ShowNotiFy(NotifyType.NoInternet);
            return false;
        }
    }
    public void SetUserProperty()
    {
        StartCoroutine(IESetUserProperty());
    }
    private IEnumerator IESetUserProperty()
    {
        yield return new WaitForSeconds(0.3f);
        string first_date = PlayerPrefs.GetString("first_date", "");
        if (first_date == "")
        {
            APIController.Instance.SetProperty("retent_type", "0");
            APIController.Instance.SetProperty("days_played", "1");
            PlayerPrefs.SetInt("days_played", 2);
            PlayerPrefs.SetString("first_date", DataController.ConvertToUnixTime(DateTime.UtcNow).ToString());
        }
        else
        {
            int days_played = PlayerPrefs.GetInt("days_played", 1);
            DateTime retent_time = DataController.ConvertFromUnixTime(Convert.ToDouble(first_date));
            int retent_date = PlayerPrefs.GetInt("retent_date", 0);
            int totalDates = (DateTime.UtcNow - retent_time).Days;
            if (Mathf.Abs(totalDates) > retent_date)
            {
                APIController.Instance.SetProperty("days_played", days_played.ToString());
                APIController.Instance.SetProperty("retent_type", totalDates.ToString());
                PlayerPrefs.SetInt("retent_date", totalDates);
                PlayerPrefs.SetInt("days_played", days_played++);
            }
        }
    }
    public IEnumerator IEPostPlayerInfo(string strRequest, string strUrl)
    {
        var request = new UnityWebRequest(strUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(strRequest);
        var uploadHandleRaw = new UploadHandlerRaw(bodyRaw);
        request.uploadHandler = uploadHandleRaw;
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error : " + request.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
    public IEnumerator IECheckGiftCode(string strRequest, UnityAction<GiftPack> SuccessCallback, UnityAction<string> FailCallback)
    {
        var request = new UnityWebRequest("https://gsmlog.cscmobicorp.com/api/giftcodev2/check", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(strRequest);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error : " + request.error);
            FailCallback.Invoke(request.error);
        }
        else
        {
            if (request.downloadHandler != null && request.downloadHandler.text != null && request.downloadHandler.text != "")
            {

                var response = JsonUtility.FromJson<GiftCodeRespone>(request.downloadHandler.text);
                if (response.result.giftType == 2)
                {
                    DataController.Instance.Subcribed = 1;
                    FindObjectOfType<SubscribeController>().SetSubscribeBtnActive(DataController.Instance.Subcribed != 1);
                    DataController.Instance.SaveData(false);
                }
                if (response.code == 0)
                    SuccessCallback.Invoke(response.result.gift);
                else
                    FailCallback.Invoke(response.result.message);
            }
            Debug.Log("Form upload complete!");
        }
    }
    public void CheckGiftCode(string code, UnityAction<GiftPack> SuccessCallback, UnityAction<string> FailCallback)
    {
        GiftCodeChecking giftCode = new GiftCodeChecking(code);
        StartCoroutine(IECheckGiftCode(JsonUtility.ToJson(giftCode), SuccessCallback, FailCallback));
    }
    public void PostData(string request, string url)
    {
        StartCoroutine(IEPostPlayerInfo(request, url));
    }
    public void LogEventEarnRuby(int rubyAmount, string source)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "ruby"),
            new Parameter(FirebaseAnalytics.ParameterValue, rubyAmount),
            new Parameter(FirebaseAnalytics.ParameterItemName, source) });
        }
    }
    public void LogEventSpentRuby(int rubyAmount, string source)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSpendVirtualCurrency, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "ruby"),
            new Parameter(FirebaseAnalytics.ParameterValue, rubyAmount),
            new Parameter(FirebaseAnalytics.ParameterItemName , source) });
        }
    }
    public void LogEventSpentGold(int goldAmount, string source)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSpendVirtualCurrency, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "gold"),
            new Parameter(FirebaseAnalytics.ParameterValue, goldAmount),
            new Parameter(FirebaseAnalytics.ParameterItemName, source) });
        }
    }
    public void LogEventEarnGold(int goldAmount, string source)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterVirtualCurrencyName, "gold"),
            new Parameter(FirebaseAnalytics.ParameterValue, goldAmount),
            new Parameter(FirebaseAnalytics.ParameterItemName, source) });
        }
    }
    public void LogEventUseItem(string itemName, string para_level_name)
    {
        FirebaseAnalytics.LogEvent("booster_used", new Parameter[] {
                new Parameter("source", itemName),
                new Parameter(FirebaseAnalytics.ParameterLevelName,  para_level_name)});
    }
    public void LogEventShowAds(string adsName)
    {
        FirebaseAnalytics.LogEvent("ads_show", new Parameter[] {
            new Parameter("source", adsName.ToLower()) });
    }
    public void LogEventViewAds(string adsName)
    {
        FirebaseAnalytics.LogEvent("ads_viewed", new Parameter[] {
            new Parameter("source", adsName.ToLower()) });
    }
    public void LogEventCertificateTracking(string certificate, int chapter)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent("chef_certificate", new Parameter[] {
            new Parameter("cert_type",certificate),
            new Parameter("chapter_id", chapter)});
        }
    }
    public void LogEventCertiAction(string action, int chapter)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent("chef_certificate", new Parameter[] {
            new Parameter("action_type",action),
            new Parameter("chapter_id", chapter)});
        }
    }
    public void LogEventSalePackAction(string action, string packID)
    {
        FirebaseAnalytics.LogEvent(action, new Parameter[] {
            new Parameter("source", packID)});
    }
    public void LogEventLevelStart(string para_level_name)
    {
        FirebaseAnalytics.LogEvent(
       FirebaseAnalytics.EventLevelStart,
       new Parameter[] { new Parameter(FirebaseAnalytics.ParameterLevelName, para_level_name) });
    }
    public void LogEventWinLevel(string para_level_name)
    {
        FirebaseAnalytics.LogEvent(
       FirebaseAnalytics.EventLevelEnd,
       new Parameter[] { new Parameter(FirebaseAnalytics.ParameterLevelName, para_level_name) });
    }
    public void LogEventFBFriendAction(string action, int value = 0)
    {
        if (value != 0)
            FirebaseAnalytics.LogEvent("friend_facebook", new Parameter[] {
            new Parameter("friendfb_action",action)});
        else
            FirebaseAnalytics.LogEvent("friend_facebook", new Parameter[] {
            new Parameter("friendfb_action",action),
            new Parameter(FirebaseAnalytics.ParameterValue,value)});
    }
    public void LogEventTrackingUserType(string user_type)
    {
        if (!DataController.Instance.IsTestUser)
        {
            FirebaseAnalytics.LogEvent("user_on_game", new Parameter[] {
            new Parameter("type_user",user_type)});
        }
    }
    public void LogEventBeginTut()
    {
        FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventTutorialBegin, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterValue,"1_1_0")});
    }
    public void LogEventCompleteTut()
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin, new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterValue,"11_2_0")});
    }
    public void LogEventPiggyBankTracking(int level, int ruby)
    {
        FirebaseAnalytics.LogEvent("tracking_piggy", new Firebase.Analytics.Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterItemName, "piggy"+level),
            new Parameter(FirebaseAnalytics.ParameterValue, ruby) });
    }
    public void LogEventBuySuccessIap(string shopType, string id)
    {
        FirebaseAnalytics.LogEvent("shop_inapp_success", new Parameter[] {
            new Parameter("source", shopType),
            new Parameter("item_name", id)});
    }
    public void LogEventHalloween(string userType, string source, string iapId)
    {
        FirebaseAnalytics.LogEvent("inapp_event_halloween", new Parameter[] {
            new Parameter("source", source),
            new Parameter("type_user", userType ),
            new Parameter(FirebaseAnalytics.ParameterItemName, iapId ) });
    }
    public void SetProperty(string name, string property)
    {
        FirebaseAnalytics.SetUserProperty(name, property);
    }
    public void LogEventFirstInapp(string packageId)
    {
        string level = "";
        if (LevelDataController.Instance.lastestPassedLevel != null)
        {
            var leveldata = LevelDataController.Instance.lastestPassedLevel;
            level = leveldata.chapter + "_" + leveldata.id + "_" + DataController.Instance.GetLevelState(leveldata.chapter, leveldata.id);
        }
        else if (LevelDataController.Instance.lastestLoseLevel != null)
        {
            var leveldata = LevelDataController.Instance.lastestLoseLevel;
            level = leveldata.chapter + "_" + leveldata.id + "_" + DataController.Instance.GetLevelState(leveldata.chapter, leveldata.id);
        }
        else
        {
            int chapter = DataController.Instance.GetHighestRestaurant();
            int id = DataController.Instance.GetHighestLevel(chapter);
            int state = DataController.Instance.GetLevelState(chapter, id);
            level = chapter + "_" + id + "_" + state;
        }
        FirebaseAnalytics.LogEvent("first_inapp", new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterLevelName, level ),
            new Parameter("source", packageId ) });
    }
    public void LogEventOpenRewardMilestone(string source)
    {
        FirebaseAnalytics.LogEvent("open_gift_milestones_on_level", new Parameter[] {
            new Parameter("source", source ) });
    }
    public void LogEventBuySuccessBP( string levelName)
    {
        FirebaseAnalytics.LogEvent("battlepass_user_buy_success", new Parameter[] {
            new Parameter(FirebaseAnalytics.ParameterLevelName, levelName ) });
    }
    public void LogEventRewarded()
    {
        StartCoroutine(IELogEventRewarded());
    }
    public void LogEventRewardShow()
    {
        StartCoroutine(IELogEventRewardShow());
    }
    private IEnumerator IELogEventRewarded()
    {
        yield return new WaitForSeconds(0.1f);
        if (!DataController.Instance.IsTestUser)
            FirebaseAnalytics.LogEvent(
               "track_ads_reward",
               new Parameter[] {
          new Parameter("source",DataController.Instance.panelAdsShow),
          new Parameter("adviewed", 1)
            });

    }
    private IEnumerator IELogEventRewardShow()
    {
        yield return new WaitForSeconds(0.1f);
        if (!DataController.Instance.IsTestUser)
            FirebaseAnalytics.LogEvent(
               "track_ads_reward",
               new Parameter[] {
          new Parameter("source",DataController.Instance.panelAdsShow),
          new Parameter("adshow", 1)
            });

    }
    public void LogEventClickBuyBP(string source,string levelName)
    {
        FirebaseAnalytics.LogEvent("battlepass_user_click_buy", new Parameter[] {
            new Parameter("source", source ),
            new Parameter(FirebaseAnalytics.ParameterLevelName, levelName )});
    }
    public void LogEventClaimBpReward(string source)
    {
        FirebaseAnalytics.LogEvent("battlepass_claim_gift_level", new Parameter[] {
            new Parameter("source", source ) });
    }
}

[Serializable]
public class ScreenDataTracking : DeviceInfoTracking
{
    public string screenName;

    public ScreenDataTracking() { }
    public ScreenDataTracking(string screenName)
    {
        this.screenName = screenName;
        if (DatabaseController.Instance.data.ClientID != null && DatabaseController.Instance.data.ClientID.Length > 0)
            facebookId = DatabaseController.Instance.data.ClientID;
        if (FirebaseServiceController.Instance.GetFirebaseAuth() != null && FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser != null)
            firebaseId = FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser.UserId;
    }
}
public class IapTracking : DeviceInfoTracking
{
    public string name;
    public string value;
    public int num;
    public string numstr;
    public IapTracking(string name, string value, int num, string numstr)
    {
        this.name = name; this.value = value; this.num = num; this.numstr = numstr;
        if (DatabaseController.Instance.data.ClientID != null && DatabaseController.Instance.data.ClientID.Length > 0)
            facebookId = DatabaseController.Instance.data.ClientID;
        if (FirebaseServiceController.Instance.GetFirebaseAuth() != null && FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser != null)
            firebaseId = FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser.UserId;
    }
}
public class IapMessageLog : DeviceInfoTracking
{
    public string name;
    public string log;
    public IapMessageLog(string name, string log)
    {
        this.name = name; this.log = log;
        if (DatabaseController.Instance.data.ClientID != null && DatabaseController.Instance.data.ClientID.Length > 0)
            facebookId = DatabaseController.Instance.data.ClientID;
        if (FirebaseServiceController.Instance.GetFirebaseAuth() != null && FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser != null)
            firebaseId = FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser.UserId;
    }
}
[Serializable]
public class LevelTracking : DeviceInfoTracking
{
    public string name;
    public string value;
    public LevelTracking() { }
    public LevelTracking(string name, string value)
    {
        this.name = name; this.value = value;
        if (DatabaseController.Instance.data.ClientID != null && DatabaseController.Instance.data.ClientID.Length > 0)
            facebookId = DatabaseController.Instance.data.ClientID;
        if (FirebaseServiceController.Instance.GetFirebaseAuth() != null && FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser != null)
            firebaseId = FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser.UserId;
    }
}
public class GiftCodeChecking
{
    //public string appId = "5f152222d40cd6003c0b53c9"; //paid
    public string appId = "60053454ace29200810ec91a";//nopaid
    public string deviceId = SystemInfo.deviceUniqueIdentifier;
    public string os = (Application.platform.ToString().ToLower() == "android") ? "1" : "2";
    public string code;
    public GiftCodeChecking(string code)
    {
        this.code = code;
    }
}
[Serializable]
public class FirstWinTracking : DeviceInfoTracking
{
    public string name;
    public string value;
    public string numstr;
    public FirstWinTracking(string name, string value, string numstr)
    {
        this.name = name; this.value = value; this.numstr = numstr;
        if (DatabaseController.Instance.data.ClientID != null && DatabaseController.Instance.data.ClientID.Length > 0)
            facebookId = DatabaseController.Instance.data.ClientID;
        if (FirebaseServiceController.Instance.GetFirebaseAuth() != null && FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser != null)
            firebaseId = FirebaseServiceController.Instance.GetFirebaseAuth().CurrentUser.UserId;
    }
}
[Serializable]
public class DeviceInfoTracking
{
    //public string appId = "5f152222d40cd6003c0b53c9"; //paid
    public string appId = "60053454ace29200810ec91a";//nopaid
    public string deviceId = SystemInfo.deviceUniqueIdentifier;
    public string version = Application.version;
    public string os = (Application.platform.ToString().ToLower() == "android") ? "1" : "2";
    public string deviceName = SystemInfo.deviceModel;
    public string iapType = PlayerPrefs.GetString("paying_type", "f1");
    public string facebookId = "null";
    public string firebaseId = "null";
}
public class Url
{
    public static string ScreenTracking = "https://gsmlog.cscmobicorp.com/api/user/trackscreen",
                        LevelTracking = "https://gsmlog.cscmobicorp.com/api/user/event",
                        GiftCodeCheck = "https://gsmlog.cscmobicorp.com/api/giftcode/check",
                        IapMessageLog = "https://gsmlog.cscmobicorp.com/api/user/log";
}
