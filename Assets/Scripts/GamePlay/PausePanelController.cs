using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PausePanelController : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject pausePanel, quitPanel;
    [SerializeField] private GamePlayBuyItemPanelController buyItemPanelController;
    [SerializeField] private GamePlayBuyRuby buyRubyPrefab;
    [SerializeField] private TextMeshProUGUI message, rubyTxt;
    [SerializeField] private Animator pauseAnim, quitAnim;
    [Header("Battle Pass")]
    [SerializeField] private GameObject normalModePopup, battlepassModePopup;
    [SerializeField] private TextMeshProUGUI sockTxt;
    private List<int> activeItems;
    public void Init()
    {
        UIController.Instance.PushUitoStack(this);
        Time.timeScale = 0;
        AudioController.Instance.PlaySfx(popUpClip);
        pausePanel.SetActive(true);
        UpdateRubyAmount();
        var levelData = LevelDataController.Instance.currentLevel;
        int currentChapter = DataController.Instance.currentChapter;
        string resName = DataController.Instance.GetRestaurantById(currentChapter).restaurantName;
        message.text = resName + " " + levelData.id + "-" + (DataController.Instance.GetLevelState(levelData.chapter, levelData.id) + 1);
        activeItems = new List<int>();
        pauseAnim.Play("Appear");
        if (BattlePassDataController.Instance.CanClaimExp && !LevelDataController.Instance.IsExtraJob && DataController.Instance.IsShowNoel == 1)
        {
            normalModePopup.SetActive(false);
            battlepassModePopup.SetActive(true);
            int exp = 5;
            if (levelData.id > 10 && levelData.id <= 20) exp = 10;
            else if (levelData.id > 20 && levelData.id <= 40) exp = 15;
            sockTxt.text = "-" + exp;
        }
        else
        {
            normalModePopup.SetActive(true);
            battlepassModePopup.SetActive(false);
        }
    }
    public void InitWithoutAds()
    {
        UIController.Instance.PushUitoStack(this);
        Time.timeScale = 0;
        AudioController.Instance.PlaySfx(popUpClip);
        pausePanel.SetActive(true);
        var levelData = LevelDataController.Instance.currentLevel;
        int currentChapter = DataController.Instance.currentChapter;
        string resName = DataController.Instance.GetRestaurantById(currentChapter).restaurantName;
        message.text = resName + " " + levelData.id + "-" + (DataController.Instance.GetLevelState(levelData.chapter, levelData.id) + 1);
        activeItems = new List<int>();
        pauseAnim.Play("Appear");
    }
    public void OnClickContinue()
    {
        UIController.Instance.PopUiOutStack();
        foreach (int itemId in activeItems)
        {
            LevelDataController.Instance.AddItem(itemId);
            DataController.Instance.UseItem(itemId, 1);
            string itemName = DataController.Instance.GetItemName(itemId);
            DataController.Instance.SaveData(false);
        }
        StartCoroutine(DelayContinue());
    }
    public override void OnHide()
    {
        OnClickContinue();
    }
    private IEnumerator DelayContinue()
    {
        pauseAnim.Play("Disappear");
        quitAnim.Play("Disappear");
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        FindObjectOfType<GameController>().DisplayActiveIcon();
        pausePanel.SetActive(false);
        quitPanel.SetActive(false);
    }
    public void OnClickExit()
    {
        pausePanel.SetActive(false);
        AudioController.Instance.PlaySfx(popUpClip);
        quitPanel.SetActive(true);
        quitAnim.Play("Appear");
    }
    public void OnClickQuit()
    {
        Time.timeScale = 1;
        LevelDataController.Instance.ConsumeItems();
        LevelData levelData = LevelDataController.Instance.currentLevel;
        int stage = DataController.Instance.GetLevelState(levelData.chapter, levelData.id);
        string levelState = levelData.chapter + "." + levelData.id + stage;
        SetIsFirstWinState(levelState);
        if (!(Application.internetReachability == NetworkReachability.NotReachable))
        {
            var level = levelData.chapter + "_" + levelData.id + "_" + DataController.Instance.GetLevelState(levelData.chapter, levelData.id);
            LevelTracking eventTracking = new LevelTracking(level, "quit");
            string request = JsonUtility.ToJson(eventTracking);
            APIController.Instance.PostData(request, Url.LevelTracking);
        }
        SceneController.Instance.LoadScene("MainMenu");
    }
    public void OnClickOpenBuyItemPanel(int itemId)
    {
        buyItemPanelController.Spawn(itemId, transform.parent);
    }
    public void AddItem(int itemId)
    {
        activeItems.Add(itemId);
    }
    public void RemoveItem(int itemId)
    {
        activeItems.Remove(itemId);
    }
    public void SetIsFirstWinState(string levelState)
    {
        int firstWinStage_count = PlayerPrefs.GetInt(levelState, 0);
        firstWinStage_count++;
        PlayerPrefs.SetInt(levelState, firstWinStage_count);
    }
    public void OnClickBuyRuby()
    {
        if (PlayerPrefs.GetString("shop_ingame_abtest", "1") == "1")
            PlayerPrefs.SetString("shop_loaction", "shop_full_ingame_btnPause");
        else
            PlayerPrefs.SetString("shop_loaction", "shop_mini_ingame_btnPause");
        Instantiate(buyRubyPrefab, transform.parent);
    }
    public void UpdateRubyAmount()
    {
        rubyTxt.text = DataController.Instance.Ruby.ToString();
    }
}
