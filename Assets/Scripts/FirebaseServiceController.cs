using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Threading;
using System;
using Firebase.Firestore;
using Firebase;
using System.Reflection;
using System.Linq;
using Firebase.Auth;
using UnityEngine.Purchasing;

public enum FireStoreColection
{
    GiveEnergy,
    Help,
    Request,
    Wave,
    GiveRuby,
    Certificate
}

public class FirebaseServiceController : MonoBehaviour
{
    private const string FB_CACHE = "firebase_cache_ver";
    private static FirebaseServiceController instance = null;
    public static FirebaseServiceController Instance
    {
        get { return instance; }
    }
    public bool isTestMode;
    public FirebaseFirestore firestoreDB;
    private FirebaseAuth firebaseAuth;
    private IEnumerator Start()
    {
        if (instance == null)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            instance = this;
            DontDestroyOnLoad(gameObject);
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {                    
                    InitFirebase();                   
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }
        //SceneController.Instance.LoadScene("Login", false);
    }
    private void InitFirebase()
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(new Dictionary<string, object>());
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        fetchTask.ContinueWithOnMainThread(OnFetchCompleted);
    }
    private void OnFetchCompleted(Task fetchTask)
    {
        if (fetchTask.IsFaulted)
        {
            Debug.LogError("cant fetch remote config");
            Debug.Log(fetchTask.Exception);
        }
        else if (fetchTask.IsCompleted)
        {
            APIController.Instance.SetUserProperty();
            firestoreDB = FirebaseFirestore.DefaultInstance;
            firebaseAuth = FirebaseAuth.DefaultInstance;
            Debug.Log("Fetch completed successfully!");
        }
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task =>
                {                  
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                   info.FetchTime));
                });

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
        //SceneController.Instance.LoadScene("Login", false);
    }
    public FirebaseAuth GetFirebaseAuth()
    {
        return firebaseAuth;
    }
    public string GetRestaurantData(string key)
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        if (value == null || value == "")
        {
            TextAsset resData = Resources.Load<TextAsset>("RestaurantsData/" + key);
            if (resData != null)
                return resData.text;
            return null;
        }
        return value;
    }
    public string GetEventRestaurantData(string key)
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        if (value == null || value == "")
        {
            TextAsset resData = Resources.Load<TextAsset>("Events/RestaurantsData/" + key);
            if (resData != null)
                return resData.text;
            return null;
        }
        return value;
    }
    public string GetItemsData(string key)
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        if (value == null || value == "")
        {
            TextAsset resData = Resources.Load<TextAsset>(key);
            if (resData != null)
                return resData.text;
            return null;
        }
        return value;
    }
    public string GetLevelData(string key)
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        if (value == null || value == "")
        {
            TextAsset levelData = null;
            levelData = Resources.Load<TextAsset>("LevelsData/" + key);
            if (levelData == null)
                Debug.Log(key);
            return levelData.text;
        }
        return value;
    }
    public string GetEventLevelData(string key)
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        if (value == null || value == "")
        {
            TextAsset levelData = null;
            levelData = Resources.Load<TextAsset>("Events/LevelsData/" + key);
            return levelData.text;
        }
        return value;
    }
    public string GetIAPPacksData()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("iap_packs").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("iap_packs");
            return data.text;
        }
        return value;
    }
    public string GetAchivement()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("achivement_datas").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("achivement");
            return data.text;
        }
        return value;
    }
    public string GetPlayerClassifyData()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("player_classify_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("player_classify_data");
            return data.text;
        }
        return value;
    }
    public string GetVersionApp()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("version_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("version_data");
            return data.text;
        }
        return value;
    }
    public string GetExtraJobData()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("extrajob_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("extrajob_data");
            return data.text;
        }
        return value;
    }
    public string GetDeviceId()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("deviceid_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("deviceid_data");
            return data.text;
        }
        return value;
    }
    public string GetABtestingIndex()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("showsalet7cn").StringValue;
        if (value == null || value == "")
        {
            return "1";
        }
        return value;
    }
    public string GetShopIngameABTesting()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("showshopfull").StringValue;
        if (value == null || value == "")
        {
            return "1";
        }
        return value;
    }
    public string GetDailyRewardData()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("daily_reward_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("daily_reward_data");
            return data.text;
        }
        return value;
    }
    public string GetAdsRewardData()
    {
        TextAsset data = null;
        data = Resources.Load<TextAsset>("ads_reward_data");
        return data.text;
    }
    public string GetRateConfig()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("rate_config").StringValue;
        if (value == null || value == "")
        {
            return PlayerPrefs.GetString("rate_config", "1");
        }
        return value;
    }
    public double GetNoelSetting()
    {
        var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("noel_setting").DoubleValue;

        return value;
    }
    public int GetRubyValueInWaveFriend()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("ruby_wave_friend").StringValue;
        if (value == null || value == "")
            return 1;
        return int.Parse(value);
    }
    public string GetBattlePassLevelData()
    {
        string value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("battle_pass_data").StringValue;
        if (value == null || value == "")
        {
            TextAsset data = null;
            data = Resources.Load<TextAsset>("battle_pass_data");
            return data.text;
        }
        return value;
    }
    #region Firestore

    public void PushProfileData()
    {
        ProfileData profileData = DataController.Instance.GetGameData().profileDatas;
        profileData.LastActiveTime = (float)DataController.ConvertToUnixTime(DateTime.UtcNow);
        var json = JsonUtility.ToJson(profileData);
        var dictionary = (Dictionary<string, object>)MiniJson.JsonDecode(json);
        DocumentReference userDocRef = firestoreDB.Collection("User").Document(profileData.FBUserID);
        userDocRef.SetAsync(dictionary);
    }
    public void PushCertificateData(string chapter, int winRate)
    {
        var dictionary = new Dictionary<string, object> { { "Winrate", winRate } };
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        var CertiRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.Certificate.ToString()).Document(chapter);
        CertiRef.SetAsync(dictionary);
    }
    public async Task<ProfileData> GetFriendProfileData(string friendId)
    {
        ProfileData ftUser = new ProfileData();
        DocumentReference docRef = firestoreDB.Collection("User").Document(friendId);
        var task = docRef.GetSnapshotAsync();
        if (await Task.WhenAny(task, Task.Delay(3000)) == task)
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                ftUser.TotalWin = float.Parse(userData["totalWin"].ToString());
                ftUser.TotalCombo = float.Parse(userData["totalCombo"].ToString());
                ftUser.TotalLike = float.Parse(userData["totalLike"].ToString());
                ftUser.TotalServeFood = float.Parse(userData["totalServeFood"].ToString());
                ftUser.TotalPlayedGame = float.Parse(userData["totalPlayedGame"].ToString());
                ftUser.AvatarID = float.Parse(userData["avatarID"].ToString());
                ftUser.BorderID = float.Parse(userData["borderID"].ToString());
                ftUser.KeyGranted = float.Parse(userData["keyGranted"].ToString());
                ftUser.LastActiveTime = float.Parse(userData["lastActiveTime"].ToString());
                ftUser.FBUserID = (string)userData["fbuserID"];
                ftUser.AvatarUrl = (string)userData["avatarUrl"];
                ftUser.FullName = (string)userData["fullName"];
                ftUser.CertiTile = (string)userData["certiTile"];
                ftUser.CertiColor = (string)userData["certiColor"];
            }
        }
        return ftUser;
    }
    public void SendFriendRequest(string friendID)
    {
        //Send request to your own document
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        Dictionary<string, object> reqTimeValue = new Dictionary<string, object> { { "Time", DataController.ConvertToUnixTime(DateTime.UtcNow) } };
        var requestRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.Request.ToString()).Document(friendID);
        requestRef.SetAsync(reqTimeValue);
        var helpRef = firestoreDB.Collection("User").Document(friendID).Collection(FireStoreColection.Help.ToString()).Document(userID);
        helpRef.SetAsync(new Dictionary<string, object>());
        //Send help to your friend's document
    }

    public void SendEnergyToFriend(string friendID)
    {
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        Dictionary<string, object> claimValueDic = new Dictionary<string, object> { { "Value", 1 } };
        var giveRef = firestoreDB.Collection("User").Document(friendID).Collection(FireStoreColection.GiveEnergy.ToString()).Document(userID);
        giveRef.SetAsync(claimValueDic);
        var requestRef = firestoreDB.Collection("User").Document(friendID).Collection(FireStoreColection.Request.ToString()).Document(userID);
        requestRef.DeleteAsync();
        var HelpRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.Help.ToString()).Document(friendID);
        HelpRef.DeleteAsync();
    }
    public void BotSendEnergy(string botID)
    {
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        var giveRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.GiveEnergy.ToString()).Document(botID);
        giveRef.SetAsync(new Dictionary<string, object>());
    }
    public void SendRubyToFriend(string friendID)
    {
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        Dictionary<string, object> claimValueDic = new Dictionary<string, object> { { "Value", 1 } };
        var giveRef = firestoreDB.Collection("User").Document(friendID).Collection(FireStoreColection.GiveRuby.ToString()).Document(userID);
        giveRef.SetAsync(claimValueDic);
        var waveRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.Wave.ToString()).Document(friendID);
        waveRef.DeleteAsync();
    }
    public async void WavingFriend(string friendId)
    {
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        var giveRef = firestoreDB.Collection("User").Document(friendId).Collection("Wave");
        Dictionary<string, object> ValueDic = new Dictionary<string, object> { { "Value", 1 } };
        await giveRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
       {
           QuerySnapshot snapshot = task.Result;
           if (snapshot.Count == 0)
           {
               if (task.IsCompleted)
               {
                   giveRef.Document(userID).SetAsync(ValueDic);
               }
           }
       });
    }
    public void DeleteFriendInAction(string ActionStr, string friendID)
    {
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        var requestRef = firestoreDB.Collection("User").Document(userID).Collection(ActionStr).Document(friendID);
        requestRef.DeleteAsync();
    }
    public async Task<List<string>> GetFrListWithAction(string Id, string ActionStr)
    {
        List<string> frList = new List<string>();
        CollectionReference actionRef = firestoreDB.Collection("User").Document(Id).Collection(ActionStr);
        await actionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
                frList.Add(documentSnapshot.Id);
        });
        return frList;
    }
    public async Task<double> GetRequestTime(string friendID)
    {
        double reqTime = 0;
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        DocumentReference docRef = firestoreDB.Collection("User").Document(userID).Collection(FireStoreColection.Request.ToString()).Document(friendID);
        await docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                reqTime = (double)userData["Time"];
            }
        });
        return reqTime;
    }
    public async Task<Dictionary<string, object>> GetCertiInfo(string friendId)
    {
        Dictionary<string, object> userData = new Dictionary<string, object>();
        CollectionReference actionRef = firestoreDB.Collection("User").Document(friendId).Collection(FireStoreColection.Certificate.ToString());
        await actionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
            {
                //Dictionary<string, object> certi = documentSnapshot.ToDictionary();
                userData.Add(documentSnapshot.Id, documentSnapshot.ToDictionary()["Winrate"]);
            }

        });
        return userData;
    }
    public async Task<double> GetFriendProfileFieldValue(string friendID, string Field)
    {
        double value = 0;
        DocumentReference docRef = firestoreDB.Collection("User").Document(friendID);
        await docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> userData = snapshot.ToDictionary();
            if (userData != null)
            {
                if (userData.ContainsKey(Field))
                    value = (double)userData[Field];
            }
        });
        return value;
    }
    void OnApplicationQuit()
    {
        firestoreDB.TerminateAsync();
        firestoreDB.ClearPersistenceAsync();
    }
    #endregion
}