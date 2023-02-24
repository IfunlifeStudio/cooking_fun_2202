using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class LanguagesPanel : UIView
{
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private GameObject uiLanguagePanel;
    private bool isScrolled;
    private int targetScrollValue;
    [SerializeField] RectTransform ContentRect;
    private int index = 0;
    private SettingPanelController settingPanel;
    [SerializeField]
    private string[] LANGUAGE_CODE = { "EN", "FR", "ID", "DE", "ES", "IT", "CN", "KR", "JP", "RU", "PT", "VN" };
    public override void OnShow(Transform parent)
    {
        UIController.Instance.PushUitoStack(this);       
        for (int i = 0; i < toggles.Length; i++)
        {
            if (Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name)
            {
                toggles[i].isOn = Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name;
                index = i;
                break;
            }
            toggles[i].isOn = Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name;
            index = i;
        }       
        uiLanguagePanel.SetActive(true);
        GetComponent<Animator>().Play("Appear");    
        isScrolled = true;
    }
    void Start()
    {        
        GetLanguageCode();
        settingPanel = FindObjectOfType<SettingPanelController>();
        isScrolled = true;
    }
    public void GetLanguageCode()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (Lean.Localization.LeanLocalization.CurrentLanguage == toggles[i].name)
            {
                index = i;
                break;
            }
        }
    }
    void Update()
    {
        if (isScrolled)
        {
            targetScrollValue = (index > 6) ? 168 : 0;
            float deltaPos = targetScrollValue - ContentRect.anchoredPosition.y;
            float sign = deltaPos / Mathf.Abs(deltaPos);
            float baseSpeed = Mathf.Max(600f, 2 * deltaPos * sign);
            if (Mathf.Abs(deltaPos) > 20)
                ContentRect.anchoredPosition += new Vector2(0, sign * Time.deltaTime * baseSpeed);
            else
                isScrolled = false;
        }
    }
    private int GetContentScrollPos(int Index)
    {
        int result = 0;
        if (Index / 4 >= 1)
            result =140+( 310 * Mathf.Max(1, Index / 4));
        return result;
    }
    public void OnClickEnglishToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "English";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[0]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickFrenchToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "French";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[1]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickIndonesiaToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Indonesian";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[2]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickDeutschToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "German";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[3]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickEspanolToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Spanish";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[4]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickItalianToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Italian";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[5]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickChineseToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Chinese";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[6]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickKoreanToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Korean";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[7]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickJapaneseToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Japanese";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[8]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickRussiaToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Russian";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[9]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickPortugalToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Portuguese";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[10]);
            settingPanel.UpdateLanguage();
        }
    }
    public void OnClickVietNamToggle(bool state)
    {
        if (state)
        {
            Lean.Localization.LeanLocalization.CurrentLanguage = "Vietnamese";
            PlayerPrefs.SetString("language_code", LANGUAGE_CODE[11]);
            settingPanel.UpdateLanguage();
        }
    }
    public override void OnHide()
    {
        GetComponent<Animator>().Play("Disappear");
        uiLanguagePanel.SetActive(false);
        UIController.Instance.PopUiOutStack();
    }
}
