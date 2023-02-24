using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PiggyBankBtnController : MonoBehaviour
{
    [SerializeField] private GameObject piggyBankBtn, notify, timeText;
    [SerializeField] private Image progressImg;
    [SerializeField] private GameObject[] piggyBankPanelArray, tutorialPanels;
    [SerializeField] private FullPiggyBankPanelController fullPiggyPanel;

    bool isPassLevel16;
    GameObject tutorialGo;
    MainMenuController mainmenu;
    void Start()
    {
        SetupData();
    }
    public void SetupData()
    {
        mainmenu = FindObjectOfType<MainMenuController>();
        DataController.Instance.PbTimeDuration -= (int)(DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp);
        if (DataController.Instance.isFirstOpenPB && DataController.Instance.PbTimeDuration <= 0)
        {
            PlayerPrefs.SetInt("has_full_pb", 0);
            PlayerPrefs.SetInt("open_full_piggy", 0);
            DataController.Instance.PbRuby = 0;
            DataController.Instance.PbTimeDuration = 86400;
            DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
            DataController.Instance.isFirstOpenPB = false;
            DataController.Instance.SaveData();
        }
        //DataController.Instance.PbTimeDuration -= (int)(DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp);
        DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
        isPassLevel16 = DataController.Instance.GetLevelState(1, 5) >= 1 && DataController.Instance.PbRuby > 0;
        bool isFullPB = DataController.Instance.IsFullPB();
        SetActiveIcon(isPassLevel16);
        if (isFullPB && DataController.Instance.PbTimeDuration <= 0)
            SetActiveIcon(false);
        float currentFillAmount = (1f * DataController.Instance.PbRuby) / DataController.Instance.CurrentPbStorage;
        progressImg.fillAmount = currentFillAmount;
        notify.SetActive(isPassLevel16 && currentFillAmount >= 0.6f);
        if (currentFillAmount == 1)
            timeText.SetActive(true);
    }
    public void ShowIntroTutorial()
    {
        tutorialGo = Instantiate(tutorialPanels[0], mainmenu.cameraCanvas);
        tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickIntrotutorial);
        Time.timeScale = 0;
    }

    public void OnClickIntrotutorial()
    {
        Time.timeScale = 1;
        Destroy(tutorialGo);
        StartCoroutine(DelayShowIntroTut2());
    }
    IEnumerator DelayShowIntroTut2()
    {
        yield return new WaitForSeconds(0.2f);
        tutorialGo = Instantiate(tutorialPanels[1], mainmenu.cameraCanvas);
        tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickIntroTut2);
        Time.timeScale = 0;
    }
    public void OnClickIntroTut2()
    {
        Time.timeScale = 1;
        Destroy(tutorialGo);
        StartCoroutine(DelayShowPanel());

    }
    public void OnClickTurnOffTutorials()
    {
        Time.timeScale = 1;
        SetupData();
        Destroy(tutorialGo);
    }
    public bool CanShowTutorial()
    {
        return !DataController.Instance.GetTutorialData().Contains(209) && isPassLevel16;
    }
    IEnumerator DelayShowPanel()
    {
        yield return new WaitForSeconds(0.2f);
        OnClickPiggybankBtn();
        yield return new WaitForSeconds(0.2f);
        tutorialGo = Instantiate(tutorialPanels[2], mainmenu.cameraCanvas);
        tutorialGo.GetComponentInChildren<Button>().onClick.AddListener(OnClickTurnOffTutorials);
        Time.timeScale = 0;
    }
    public bool CanShowFullPiggyPanel()
    {
        if (PlayerPrefs.GetInt("open_full_piggy", 0) == 0 && DataController.Instance.IsFullPB())
        {
            PlayerPrefs.SetInt("open_full_piggy", 1);
            return true;
        }
        return false;
    }
    public void SetActiveIcon(bool status)
    {
        piggyBankBtn.SetActive(status);
    }
    public void ShowFullPiggyPanel()
    {
        var panel = Instantiate(fullPiggyPanel, FindObjectOfType<MainMenuController>().cameraCanvas);
        panel.Init(piggyBankBtn.transform.position, OnClickPiggybankBtn);
        FindObjectOfType<MainMenuController>().setIndexTab(5);
    }
    public void OnClickPiggybankBtn()
    {
        var piggy = Instantiate(piggyBankPanelArray[DataController.Instance.PbLevel - 1], FindObjectOfType<MainMenuController>().cameraCanvas);
        piggy.GetComponent<PiggyBankPanelController>().ForceOpen = false;
        FindObjectOfType<MainMenuController>().setIndexTab(5);
    }
    public void OnForceClickPiggyBankBtn()
    {
        var piggy = Instantiate(piggyBankPanelArray[DataController.Instance.PbLevel - 1], FindObjectOfType<MainMenuController>().cameraCanvas);
        piggy.GetComponent<PiggyBankPanelController>().ForceOpen = true;
        FindObjectOfType<MainMenuController>().setIndexTab(5);
    }
}
