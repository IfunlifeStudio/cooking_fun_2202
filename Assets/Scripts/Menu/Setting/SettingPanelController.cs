using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : UIView
{
    [SerializeField] private LanguagesPanel languagePanelPrefab;
    [SerializeField] private GameObject settingPanel, newVersionNotification, saveProcessPanelPrefab/*, settingNotification, btnLanguage, SignOutAppleIDBtn*/, BtnFBLogin, BtnFBLogged;
    [SerializeField] private RateUsController rateUsPrefab;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private LogOutController logOutPanelPrefab;
    [SerializeField] private GameObject noInternetPanel, GiftCodePanel;
    [SerializeField] private TMP_InputField ChapterInf, LevelInf;
    [SerializeField] Image musicIcon, sfxIcon, vibrationIcon;
    [SerializeField] private GameObject TestGroup;
    [SerializeField] Sprite musicIconOn, musicIconOff, sfxIconOn, sfxIconOff, vibrationIconOn, vibrationIconOff;
    [SerializeField] private GameObject GiftCode;
    [SerializeField] private GameObject deleteAccountBtn, deleteAccountPanel;
    [SerializeField] TextMeshProUGUI textLanguage;
    AudioController audioController;
    bool isIOS, isShowRate, isShowFeedBack;
    // Start is called before the first frame update
    private void Start()
    {
        int level = DataController.Instance.GetLevelState(1, 7);
        GiftCode.SetActive(level > 0);
        bool isAppleConnected = (PlayerPrefs.GetInt("LOGIN_TYPE") == 3);
        bool isFbConnected = (PlayerPrefs.GetInt("LOGIN_TYPE") == 1);
        BtnFBLogin.SetActive(!isAppleConnected && !FB.IsLoggedIn);
        BtnFBLogged.SetActive(FB.IsLoggedIn || isAppleConnected);
        deleteAccountBtn.SetActive(FB.IsLoggedIn || isAppleConnected);
    }
    private void Awake()
    {
        audioController = AudioController.Instance;
    }

    public override void OnShow()
    {
        UIController.Instance.PushUitoStack(this);
        settingPanel.SetActive(true);
        //SettingDisplayButton();
        newVersionNotification.SetActive(DataController.Instance.CheckVersion());
        GetComponent<Animator>().Play("Appear");
        if (audioController.Music)
            musicIcon.sprite = musicIconOn;
        else
            musicIcon.sprite = musicIconOff;
        if (audioController.SFX)
            sfxIcon.sprite = sfxIconOn;
        else
            sfxIcon.sprite = sfxIconOff;
        if (audioController.Vibration)
            vibrationIcon.sprite = vibrationIconOn;
        else
            vibrationIcon.sprite = vibrationIconOff;
        UpdateLanguage();
        AudioController.Instance.PlaySfx(popUpClip);
        if (TestGroup != null)
        {
            TestGroup.gameObject.SetActive(DataController.Instance.deviceIdData.CheckDeviceId(DataController.Instance.GetGameData().userID) || DataController.Instance.deviceIdData.CheckDeviceId(SystemInfo.deviceUniqueIdentifier));
        }
    }
    public void ChangeMusic()
    {
        if (audioController.Music)
        {
            audioController.Music = false;
            musicIcon.sprite = musicIconOff;
        }
        else
        {
            audioController.Music = true;
            musicIcon.sprite = musicIconOn;
        }
    }
    public void ChangeSfx()
    {
        if (audioController.SFX)
        {
            audioController.SFX = false;
            sfxIcon.sprite = sfxIconOff;
        }
        else
        {
            audioController.SFX = true;
            sfxIcon.sprite = sfxIconOn;
        }
    }

    public void ChangeVibration()
    {
        if (audioController.Vibration)
        {
            audioController.Vibration = false;
            vibrationIcon.sprite = vibrationIconOff;
        }
        else
        {
            audioController.Vibration = true;
            vibrationIcon.sprite = vibrationIconOn;
        }
    }
    public void OnLoginClick()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            DatabaseController.Instance.LoginWithFaceBook();
        else
        {
            Instantiate(noInternetPanel, transform.parent);
        }

    }
    public void OnClickSaveProcess()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Instantiate(saveProcessPanelPrefab, transform.parent).GetComponent<LoginPanelController>();
        }
        else
        {
            Instantiate(noInternetPanel, transform.parent);
        }
    }
    public void OnClickLogOut()
    {
        DatabaseController.Instance.LogOutFacebook();
        BtnFBLogin.SetActive(true);
        BtnFBLogged.SetActive(false);
        SceneController.Instance.LoadScene("MainMenu", false);
    }
    public void OnClickLanguageBtn()
    {
        languagePanelPrefab.OnShow(transform.parent);
        //Instantiate(languagePanelPrefab, transform.parent);
    }
    public void OnClickRateUs()
    {
        Instantiate(rateUsPrefab, transform).OnShow();
    }
    public void OnClickContactUs()
    {
        Application.OpenURL("mailto:" + FeedbackController.EMAIL_ADDRESS + "?subject=" + FeedbackController.SUBJECT + DataController.Instance.GetGameData().userID + "_" + SystemInfo.deviceUniqueIdentifier + "_" + SystemInfo.deviceModel + "_" + Application.version + "&body=" + FeedbackController.BODY);
    }
    public void OnClickNewVersion()
    {
#if UNITY_ANDROID  
        Application.OpenURL("market://details?id=com.cscmobi.cooking.love.free");
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com");
#endif
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.4f);
        settingPanel.SetActive(false);
    }

    public void OnClickClearAllData()
    {
        DataController.Instance.ResetAllData();
    }
    public void CheatRuby1()
    {
        DataController.Instance.CheatRuby();
    }
    public void RemoveRuby()
    {
        DataController.Instance.RemoveRuby();
    }
    public void PlayAnyLevel()
    {
        if (ChapterInf.text != null && LevelInf.text != null)
        {
            int chapter = 1, level = 1;
            try
            {
                chapter = Int32.Parse(ChapterInf.text);
                level = Int32.Parse(LevelInf.text);
                LevelDataController.Instance.LoadLevel(chapter, level);
                DataController.Instance.currentChapter = chapter;
                LevelDataController.Instance.lastestPassedLevel = null;
                LevelDataController.Instance.collectedGold = 0;
                string gameScene = DataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
                SceneController.Instance.LoadScene(gameScene);
            }
            catch
            {
                Debug.Log("Can't Parse");
            }

        }
    }
    public void UpdateLanguage()
    {
        string language = PlayerPrefs.GetString("language_code", "");
        if (language.Equals("")) language = "EN";
        textLanguage.text = language;
    }
    public void ResetFlashSale()
    {
        PlayerPrefs.SetFloat("fs_waittime", 0);
        PlayerPrefs.SetFloat("FLASH_SALE", 0);
    }
    public void OnClickGiftCodeBtn()
    {
        Instantiate(GiftCodePanel, transform.parent);
        settingPanel.SetActive(false);
    }
    public void OnClickDelete()
    {
        Instantiate(deleteAccountPanel, transform.parent);
    }
}
