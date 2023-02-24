using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassifyController : MonoBehaviour
{
    public int winCount, loseCount;
    int tier = -1;
    int playerclassifyLevel = -1;
    private static PlayerClassifyController instance;
    public static PlayerClassifyController Instance
    {
        get { return instance; }
    }
    public PlayerClassifyData playerClassifyData;
    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void LoadData()
    {
        playerClassifyData = JsonUtility.FromJson<PlayerClassifyData>(FirebaseServiceController.Instance.GetPlayerClassifyData());
        AdsController.Instance.InitAds();
    }
    public int GetPlayerClassifyLevel()
    {
        if (playerclassifyLevel == -1)
        {
            UpdatePlayerClassifyLevel();
        }
        return playerclassifyLevel;
    }
    public void UpdatePlayerClassifyLevel()
    {

        float totalMoneyPay = DataController.Instance.GetGameData().userIapValue;
        if (totalMoneyPay == 0)
            totalMoneyPay = DataController.Instance.GetGameData().userIapValue = PlayerPrefs.GetFloat("user_iap_value", 0);
        if (totalMoneyPay > 300)
        {
            playerclassifyLevel = 9;
            PlayerPrefs.SetString("paying_type", "i300");
            APIController.Instance.SetProperty("paying_type", "i300");
            return;
        }
        if (150 < totalMoneyPay && totalMoneyPay <= 300)
        {
            PlayerPrefs.SetString("paying_type", "i150");
            APIController.Instance.SetProperty("paying_type", "i150");
            playerclassifyLevel = 8;
            return;
        }
        if (50 < totalMoneyPay && totalMoneyPay <= 150)
        {
            PlayerPrefs.SetString("paying_type", "i50");
            APIController.Instance.SetProperty("paying_type", "i50");
            playerclassifyLevel = 7;
            return;
        }
        if (25 < totalMoneyPay && totalMoneyPay <= 50)
        {
            PlayerPrefs.SetString("paying_type", "i25");
            APIController.Instance.SetProperty("paying_type", "i25");
            playerclassifyLevel = 6;
            return;
        }
        if (10 < totalMoneyPay && totalMoneyPay <= 25)
        {
            PlayerPrefs.SetString("paying_type", "i10");
            APIController.Instance.SetProperty("paying_type", "i10");
            playerclassifyLevel = 5;
            return;
        }
        if (3 < totalMoneyPay && totalMoneyPay <= 10)
        {
            PlayerPrefs.SetString("paying_type", "i3");
            APIController.Instance.SetProperty("paying_type", "i3");
            playerclassifyLevel = 4;
            return;
        }
        if (0 < totalMoneyPay && totalMoneyPay <= 3)
        {
            PlayerPrefs.SetString("paying_type", "i1");
            APIController.Instance.SetProperty("paying_type", "i1");
            playerclassifyLevel = 3;
            return;
        }
        int totalKeyGranted = DataController.Instance.GetTotalKeyGranted();
        if (totalMoneyPay == 0 && totalKeyGranted >= playerClassifyData.keyThresholds[1])
        {
            PlayerPrefs.SetString("paying_type", "f3");
            APIController.Instance.SetProperty("paying_type", "f3");
            playerclassifyLevel = 2;
            return;
        }
        if (totalMoneyPay == 0 && totalKeyGranted >= playerClassifyData.keyThresholds[0])
        {
            PlayerPrefs.SetString("paying_type", "f2");
            APIController.Instance.SetProperty("paying_type", "f2");
            playerclassifyLevel = 1;
            return;
        }
        PlayerPrefs.SetString("paying_type", "f1");
        APIController.Instance.SetProperty("paying_type", "f1");
        playerclassifyLevel = 0;
    }
    public void SetPlayerClassifyLevel(int level)
    {
        playerclassifyLevel = level;
    }
    public AdsSettings GetAdsSettings()
    {
        return playerClassifyData.adsSettingsCollection[GetPlayerClassifyLevel()];
    }
    public AdsFullCondition GetAdsFullCondition()
    {
        return playerClassifyData.adsFullConditions[GetPlayerClassifyLevel()];
    }
    public void SaveGameResult(bool gameResult, bool isUseItem)
    {
        if (gameResult) winCount++;
        else loseCount++;
        DataController.Instance.GetGameData().SaveGameResult(gameResult, isUseItem);
    }
    public int GetPlayerTier()
    {
        if (DataController.Instance.currentChapter <= 1)
            return 0;
        return DataController.Instance.GetGameData().PlayerTier;
    }
}
