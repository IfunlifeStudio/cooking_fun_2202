using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Facebook.Unity;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using Firebase.Extensions;

public class DatabaseController : MonoBehaviour
{
    public const string LOGIN_TYPE = "LOGIN_TYPE";
    public DatabaseClient data;
    private static DatabaseController instance;
    public static DatabaseController Instance { get { return instance; } }
    [SerializeField] private AccountAlert accountAlertPrefab;
    [SerializeField] private GameObject loadingPanelPrefab;
    [SerializeField] private GameObject restoredPanelPrefab;
    [SerializeField] private GameObject RemoveadssuccessPrefab;
    [SerializeField] private GameObject restoredfailPanelPrefab;
    private GameObject activeLoadingPanel;
    private AppleSignInController AppleSignInController;
    int loginType = 2;
    bool pulldata = false, isNoServerData = false, isTokenExpired = false, dataReady = false, isAppleExpired, getProfile = false, IsNewUser = false;
    void Start()
    {
        if (instance == null)
        {
            AppleSignInController = GetComponent<AppleSignInController>();
            instance = this;
            if (!FB.IsInitialized)
                FB.Init(InitCallback, OnHideUnity);// Initialize the Facebook SDK
            else
                FB.ActivateApp();// Already initialized, signal an app activation App Event
            data = new DatabaseClient();
        }
        else Destroy(gameObject);
    }
    public FirebaseAuth GetAthInstanceDatabase()
    {
        return FirebaseAuth.DefaultInstance;
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
            FB.ActivateApp();
        else
            Debug.Log("Failed to Initialize the Facebook SDK");
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
            Time.timeScale = 0;// Pause the game - we will need to hide
        else
            Time.timeScale = 1;// Resume the game - we're getting focus again
    }
    public void SpawnLoadingOverlay()
    {
        if (activeLoadingPanel != null) Destroy(activeLoadingPanel);
        activeLoadingPanel = Instantiate(loadingPanelPrefab);
        UIController.Instance.CanBack = false;
        DontDestroyOnLoad(activeLoadingPanel);
    }
    private IEnumerator IESpawnLoading()
    {
        if (activeLoadingPanel != null) Destroy(activeLoadingPanel);
        activeLoadingPanel = Instantiate(loadingPanelPrefab);
        UIController.Instance.CanBack = false;
        DontDestroyOnLoad(activeLoadingPanel);
        yield return new WaitForSeconds(1.5f);
        if (activeLoadingPanel != null) Destroy(activeLoadingPanel);
    }
    public void SpawnLoading()
    {
        StartCoroutine(IESpawnLoading());
    }
    public void SpawnRestoreOverlay()
    {
        Instantiate(restoredPanelPrefab);
    }
    public void SpawnRestoreFailOverlay()
    {
        Instantiate(restoredfailPanelPrefab);
    }
    public void SpawnRemoveAdsSuccessOverlay()
    {
        Instantiate(RemoveadssuccessPrefab);
    }
    public void DespawnLoadingOverlay()
    {
        UIController.Instance.CanBack = true;
        if (activeLoadingPanel != null) Destroy(activeLoadingPanel);
    }
    public void LogOutFacebook()
    {
        data = new DatabaseClient();
        LevelDataController.Instance.lastestPassedLevel = null;
        PlayerPrefs.SetInt("LOGIN_TYPE", 2);
        FB.LogOut();
    }
    public void LogoutAppleId()
    {
        PlayerPrefs.SetInt("LOGIN_TYPE", 2);
        LevelDataController.Instance.lastestPassedLevel = null;
    }
    public void LoginWithFaceBook()
    {
        SpawnLoadingOverlay();
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    public void QuickLoginWithAppleID()
    {
        SpawnLoadingOverlay();
        AppleSignInController.AttemptQuickLogin(AuthAppleIDCallBack);
    }
    public void LoginWithAppleID()
    {
        SpawnLoadingOverlay();
        AppleSignInController.PerformLoginWithAppleIdAndFirebase(AuthAppleIDCallBack);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            PlayerPrefs.SetInt("LOGIN_TYPE", 1);
            loginType = 1;
            Credential credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
            GetAthInstanceDatabase().SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithCredentialAsync was canceled.");
                    dataReady = true;
                }
                else
                if (task.IsFaulted)
                {
                    Debug.Log("SignInWithCredentialAsync encountered an error: ");
                    dataReady = true;
                }
                else if (task.IsCompleted)
                {
                    FirebaseUser newUser = task.Result;
                    FirebaseDatabase.DefaultInstance
                          .GetReference(newUser.UserId)
                          .GetValueAsync().ContinueWith(task1 =>
                          {
                              if (task1.IsFaulted)
                              {
                                  dataReady = true;
                              }
                              else if (task1.IsCompleted)
                              {
                                  DataSnapshot snapshot = task1.Result;
                                  if (snapshot == null)
                                      isNoServerData = true;
                                  else
                                  {
                                      if (snapshot.Value != null)
                                      {
                                          pulldata = true;
                                          Debug.Log("newUser.UserId snapshot " + newUser.UserId);
                                      }
                                      else
                                      {
                                          IsNewUser = true;
                                          Debug.Log("newUser.UserId IsNewUser " + newUser.UserId);
                                      }
                                  }
                              }
                          });
                }
            });
            getProfile = true;
        }
        else
        {
            DataController.Instance.LoadData();
            DespawnLoadingOverlay();
        }
    }
    public void GetFacebookProfileImage()
    {
        FB.API("/me/picture?redirect=false&width=500&height=500", HttpMethod.GET, CacheProfilePicture);
        FB.API("me?fields=name", HttpMethod.GET, NameCallBack);
    }
    void NameCallBack(IGraphResult result)
    {
        string fbname = result.ResultDictionary["name"].ToString();
        string userId = result.ResultDictionary["id"].ToString();
        DataController.Instance.GetGameData().profileDatas.FullName = fbname;
        DataController.Instance.GetGameData().profileDatas.FBUserID = userId;
        DataController.Instance.SaveData(false);
    }
    private void CacheProfilePicture(IGraphResult result)
    {
        if (String.IsNullOrEmpty(result.Error) && !result.Cancelled)
        { //if there isn't an error
            IDictionary data = result.ResultDictionary["data"] as IDictionary; //create a new data dictionary
            string photoURL = data["url"] as String; //add a URL field to the dictionary
            //PlayerPrefs.SetString("fb_url", photoURL);
            DataController.Instance.GetGameData().profileDatas.AvatarUrl = photoURL;
            StartCoroutine(FetchProfilePic(photoURL));
        }
    }
    private IEnumerator FetchProfilePic(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        byte[] bytes = DownloadHandlerTexture.GetContent(www).EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/profile_img.png", bytes);
        yield return new WaitForSeconds(0.1f);
        var certificationController = FindObjectOfType<CertificationPanelController>();
        if (certificationController != null) certificationController.LoadProfileImage();
    }
    public void AuthAppleIDCallBack(FirebaseUser firebaseUser)
    {
        SpawnLoadingOverlay();
        FirebaseUser newUser = firebaseUser;
        FirebaseDatabase.DefaultInstance
              .GetReference(newUser.UserId)
              .GetValueAsync().ContinueWith(task1 =>
              {
                  if (task1.IsFaulted)
                  {
                      dataReady = true;
                  }
                  else if (task1.IsCompleted)
                  {
                      DataSnapshot snapshot = task1.Result;
                      if (snapshot == null)
                          isNoServerData = true;
                      else
                      {
                          loginType = 3;
                          if (snapshot.Value != null)
                          {
                              pulldata = true;
                              Debug.Log("newUser.UserId snapshot " + newUser.UserId);
                          }
                          else
                          {
                              IsNewUser = true;
                              Debug.Log("newUser.UserId IsNewUser " + newUser.UserId);
                          }
                      }
                  }
              });
    }
    public void DeleteAccount()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child(GetAthInstanceDatabase().CurrentUser.UserId).RemoveValueAsync().
        ContinueWithOnMainThread(ReloadAfterDeleteAcc);
    }
    private void ReloadAfterDeleteAcc(Task deleteTask)
    {
        if (deleteTask.IsFaulted)
        {
            Debug.LogError("can't delete data");
            Debug.Log(deleteTask.Exception);
        }
        else if (deleteTask.IsCompleted)
        {
            LevelDataController.Instance.currentLevel = LevelDataController.Instance.GetLevelData(1, 1);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
            foreach (string filePath in filePaths)
                if (filePath.Contains(".dat"))
                    File.Delete(filePath);
            LogOutFacebook();
            DataController.Instance.ResetAllData();
            SceneController.Instance.LoadScene("ReLogin", true);
        }
    }
    public void PostData()
    {
        if (PlayerPrefs.GetInt(LOGIN_TYPE) == 2) return;
        DatabaseClient DatabaseClient = new DatabaseClient();
        DatabaseClient.DateTime = DataController.Instance.GetGameData().SaveTime;
        if (DataController.Instance.GetGameData().userID.Equals(""))
        {
            string userid = PlayerPrefs.GetString("ath_user_id", "");
            DatabaseClient.ClientID = userid;
            DataController.Instance.SaveUserID(userid);
        }
        else DatabaseClient.ClientID = DataController.Instance.GetGameData().userID;
        DatabaseClient.ClientID = DataController.Instance.GetGameData().userID;
        DatabaseClient.GameData = DataController.Instance.GetGameData();
        //FirestoreUser ftUser = new FirestoreUser(PlayerPrefs.GetString("fb_id"), DataController.Instance.HighestRestaurant.ToString(), DataController.Instance.HighestLevelInHighestChapter.ToString());
        if (FB.IsLoggedIn && DataController.Instance.GetGameData().profileDatas.FBUserID != "")
        {
            FirebaseServiceController.Instance.PushProfileData();
        }

        FirebaseDatabase.DefaultInstance.RootReference.Child(DatabaseClient.ClientID).SetRawJsonValueAsync(JsonUtility.ToJson(DatabaseClient));
    }
    public void GetData(string user)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child(user).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Cant download player data, use local one");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                data = JsonUtility.FromJson<DatabaseClient>(snapshot.GetRawJsonValue());
                data.GameData.UpdateBattePassData();
            }
            dataReady = true;
        });
    }
    public void ProgressAfterLogin(bool writeOver)
    {
        DataController.Instance.SaveUserID(data.ClientID);
        if (writeOver)
            DataController.Instance.SaveData(data.GameData);
        DataController.Instance.PrepareGameData();
    }
    private void Update()
    {
        if (isTokenExpired)
        {
            isTokenExpired = false;
            //PlayerPrefs.DeleteKey(LOGIN_TYPE);
            DespawnLoadingOverlay();
            DataController.Instance.PrepareGameData();
            //LoginWithFaceBook();//relogin facebook to get new token
        }
        if (isNoServerData)
        {
            isNoServerData = false;
            PlayerPrefs.SetInt(LOGIN_TYPE, loginType);
            DataController.Instance.LoadData();
            DataController.Instance.SaveUserID(GetAthInstanceDatabase().CurrentUser.UserId);
            DespawnLoadingOverlay();

        }
        if (IsNewUser)
        {
            IsNewUser = false;
            string userid = GetAthInstanceDatabase().CurrentUser.UserId;
            PlayerPrefs.SetInt(LOGIN_TYPE, loginType);
            PlayerPrefs.SetString("ath_user_id", userid);
            DespawnLoadingOverlay();
            dataReady = true;
            return;
        }
        if (getProfile)
        {
            getProfile = false;
            PlayerPrefs.SetInt(LOGIN_TYPE, loginType);
            GetFacebookProfileImage();
            DespawnLoadingOverlay();
        }
        if (pulldata)
        {
            pulldata = false;
            PlayerPrefs.SetInt(LOGIN_TYPE, loginType);
            GetData(GetAthInstanceDatabase().CurrentUser.UserId);
            DataController.Instance.SaveUserID(GetAthInstanceDatabase().CurrentUser.UserId);
        }
        if (dataReady)
        {
            dataReady = false;
            DataController.Instance.LoadLocalData();
            if (data.ClientID == "null")
            {
                DataController.Instance.PrepareGameData();
            }
            else
            {
                PlayerPrefs.SetInt(LOGIN_TYPE, loginType);
                var gd = DataController.Instance.GetGameData();
                if (data.ClientID != gd.userID)
                {
                    int compareData = data.CompareData(gd);
                    if (compareData == -1)
                    {
                        DataController.Instance.SaveUserID(GetAthInstanceDatabase().CurrentUser.UserId);
                        DataController.Instance.PrepareGameData();
                    }
                    else
                    if (compareData == 1)
                        ProgressAfterLogin(true);
                    else
                        accountAlertPrefab.Spawn(gd, data.GameData);
                }
                else
                {
                    DateTime t1 = new DateTime(gd.SaveTime);
                    DateTime t2 = new DateTime(data.DateTime);
                    int t = DateTime.Compare(t1, t2);
                    if (t > 0)
                        DataController.Instance.PrepareGameData();
                    else
                        ProgressAfterLogin(true);
                }
            }
            DespawnLoadingOverlay();
        }
    }
}
[Serializable]
public class DatabaseClient
{
    public string ClientID = "null";
    public long DateTime = 0;
    public GameData GameData;
    private static bool isNull = false;
    public int CompareData(GameData other)
    {
        isNull = false;
        int result = 0;
        if (GameData.xp >= other.xp && GameData.gold >= other.gold && GameData.ruby >= other.ruby && GetTotalItem(GameData) >= GetTotalItem(other) && GetTotalStars(GameData) >= GetTotalStars(other) && GetTotalIngredientLevels(GameData) >= GetTotalIngredientLevels(other) && GetTotalMachineLevels(GameData) >= GetTotalMachineLevels(other))
            result = 1;
        if (GameData.xp <= other.xp && GameData.gold <= other.gold && GameData.ruby <= other.ruby && GetTotalItem(GameData) <= GetTotalItem(other) && GetTotalStars(GameData) <= GetTotalStars(other) && GetTotalIngredientLevels(GameData) <= GetTotalIngredientLevels(other) && GetTotalMachineLevels(GameData) <= GetTotalMachineLevels(other))
            result = -1;
        if (isNull)
            return 0;
        return result;
    }
    private static int GetTotalItem(GameData data)
    {
        int total = 0;
        try
        {
            foreach (var itemState in data.itemStates)
                total += itemState.quantity;
            return total;
        }
        catch
        {
            isNull = true;
            Debug.LogError("Total Item=null");
            return 0;
        }
        return 0;

    }

    private static int GetTotalStars(GameData data)
    {
        int total = 0;
        try
        {
            foreach (var level in data.levelStates)
                total += level.Level;
            return total;
        }
        catch
        {
            isNull = true;
            Debug.LogError("Total star=null");
            return 0;
        }

    }
    private static int GetTotalIngredientLevels(GameData data)
    {
        int total = 0;
        try
        {
            foreach (var ingredient in data.ingredientStates)
                total += ingredient.Level;
            return total;
        }
        catch
        {
            isNull = true;
            Debug.LogError("Total ingredient level =null");
            return 0;
        }

    }
    private static int GetTotalMachineLevels(GameData data)
    {
        int total = 0;
        try
        {
            foreach (var machine in data.machineStates)
                total += machine.Level;
            return total;
        }
        catch
        {
            isNull = true;
            Debug.LogError("Total machine level =null");
            return 0;
        }
        return 0;
    }
}
[Serializable]
public class DeviceIdData
{
    public string[] deviceId;
    public bool CheckDeviceId(string id)
    {
        for (int i = 0; i < deviceId.Length; i++)
        {
            if (deviceId[i].Equals(id))
                return true;
        }
        return false;
    }
}
//[Serializable]
//public class FbUserData
//{
//    public string name;
//    public string url;
//    public string userId;
//    public int chapter;
//    public int level;
//    public FbUserData() { }
//    public void SetLevelState(int chapter, int level)
//    {
//        if (chapter > this.chapter)
//        {
//            this.chapter = chapter;
//            this.level = level;
//        }
//        else if (this.chapter == chapter)
//        {
//            if (this.level >= level)
//                this.level = level;
//        }
//    }

//    public FbUserData(string name, string url, string userId, int chapter = 1, int level = 1)
//    {
//        this.name = name;
//        this.url = url;
//        this.userId = userId;
//        SetLevelState(chapter, level);
//    }
//}
[Serializable]
public class ProfileData
{
    [SerializeField] float totalWin;
    [SerializeField] float totalCombo;
    [SerializeField] float totalLike;
    [SerializeField] float totalServeFood;
    [SerializeField] float totalPlayedGame;
    [SerializeField] float avatarID;
    [SerializeField] float borderID;
    [SerializeField] float keyGranted;
    [SerializeField] float lastActiveTime;
    [SerializeField] string fbuserID;
    [SerializeField] string avatarUrl;
    [SerializeField] string fullName;
    [SerializeField] string certiTile;
    [SerializeField] string certiColor;
    public ProfileData() { }

    public ProfileData(int totalWin, int totalCombo, int totalLike, int totalServeFood, int totalPlayedGame, int avatarID, int borderID, int keyGranted, float lastActiveTime, string fullName, string certiTile, string certiColor)
    {
        this.totalWin = totalWin;
        this.totalCombo = totalCombo;
        this.totalLike = totalLike;
        this.totalServeFood = totalServeFood;
        this.totalPlayedGame = totalPlayedGame;
        this.avatarID = avatarID;
        this.borderID = borderID;
        this.keyGranted = keyGranted;
        this.lastActiveTime = lastActiveTime;
        this.fbuserID = "";
        this.avatarUrl = "";
        this.fullName = fullName;
        this.certiTile = certiTile;
        this.certiColor = certiColor;
    }
    public float TotalWin { get { return totalWin; } set { totalWin = value; } }
    public float TotalCombo { get { return totalCombo; } set { totalCombo = value; } }
    public float TotalLike { get { return totalLike; } set { totalLike = value; } }
    public float TotalServeFood { get { return totalServeFood; } set { totalServeFood = value; } }
    public float TotalPlayedGame { get { return totalPlayedGame; } set { totalPlayedGame = value; } }
    public float AvatarID { get { return avatarID; } set { avatarID = value; } }
    public float BorderID { get { return borderID; } set { borderID = value; } }
    public float KeyGranted { get { return keyGranted; } set { keyGranted = value; } }
    public float LastActiveTime { get { return lastActiveTime; } set { lastActiveTime = value; } }
    public string FBUserID { get { return fbuserID; } set { fbuserID = value; } }
    public string AvatarUrl { get { return avatarUrl; } set { avatarUrl = value; } }
    public string FullName { get { return fullName; } set { fullName = value; } }
    public string CertiTile { get { return certiTile; } set { certiTile = value; } }
    public string CertiColor { get { return certiColor; } set { certiColor = value; } }

}
[Serializable]
public struct BoosterItem
{
    public int id;
    public GameObject booster;
}
[System.Serializable]
public struct Items
{
    public int itemId;
    public int itemQuantity;
    public Items(int id, int quantity)
    {
        this.itemId = id;
        this.itemQuantity = quantity;
    }
}
