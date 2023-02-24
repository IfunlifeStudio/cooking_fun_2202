using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoginController : MonoBehaviour
{
    bool isClicked = false;
    [SerializeField] Animator animator;
    private string[] LANGUAGE = { "English", "French", "Indonesian", "German", "Spanish", "Italian", "Chinese", "Korean", "Japanese", "Russian", "Portuguese", "Vietnamese" };
    private string[] LANGUAGE_CODE = { "EN", "FR", "ID", "DE", "ES", "IT", "CN", "KR", "JP", "RU", "PT", "VN" };
    public GameObject gdprPanelPrefab;
    public Transform gdprCanvas;
    void Start()
    {
        if (!PlayerPrefs.HasKey("language_code"))
        {
            for (int i = 0; i < LANGUAGE.Length; i++)
            {
                if (Lean.Localization.LeanLocalization.CurrentLanguage == LANGUAGE[i])
                {
                    PlayerPrefs.SetString("language_code", LANGUAGE_CODE[i]);
                    break;
                }
            }
        }
        isClicked = false;
        StartCoroutine(ShowGDPRPanel());
    }
    public IEnumerator ShowGDPRPanel()
    {
        yield return new WaitForSeconds(0.2f);
#if UNITY_ANDROID
        
        if (GDPRPopup.CanDisplayGDPRPopup())
        {            
            Instantiate(gdprPanelPrefab, gdprCanvas);           
        }
        else
        {
            PlayerPrefs.SetInt("gdpr", 1);
            IronsourceAdsController.Instance.Init();
        }
#endif
#if UNITY_IOS
        PlayerPrefs.SetInt("gdpr", 1);
        IronsourceAdsController.Instance.Init();
#endif
    }
    public void OnBtnPlayClick()
    {
        if (!isClicked)
        {
            isClicked = true;
            AudioController.Instance.ResetAudio();
            animator.Play("PlayBtnOff");
        }
    }
    public void LoadData()
    {
        DataController.Instance.LoadData();
        if (!PlayerPrefs.HasKey("LOGIN_TYPE"))
        {
            PlayerPrefs.SetString("paying_type", "f1");
            APIController.Instance.SetProperty("paying_type", "f1");
            PlayerPrefs.SetInt("LOGIN_TYPE", 2);
            DataController.Instance.SaveUserID(SystemInfo.deviceUniqueIdentifier);
        }
    }
}
