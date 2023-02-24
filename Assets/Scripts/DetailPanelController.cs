using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailPanelController : UIView
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform overlayCanvas;
    [SerializeField] private TextMeshProUGUI restaurantName;
    [SerializeField] private GameObject ingredientNotiIcon, machineNotiIcon;
    [SerializeField] private GameObject freeCoinPanelPrefab;
    [SerializeField] private MachineUpgradeController[] machineUpgrades;
    [SerializeField] private IngredientUpgradeController[] ingredientUpgrades;
    [SerializeField] private LevelSelectorController levelSelectorController;
    [SerializeField] private MainMenuController mainMenuController;
    [SerializeField] private Level6_2TutorialPanel level6_2TutorialPanelPrefab;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject Panel, CheatBtn;
    private IngredientUpgradeController activeIngredientUpgrade;
    private MachineUpgradeController activeMachineUpgrade;
    bool hasShowFreeCoin;
    // Start is called before the first frame update
    public override void OnShow()
    {
        UIController.Instance.PushUitoStack(this);
        Panel.SetActive(true);
        animator.Play("Appear");
    }
    public override void OnHide()
    {
        mainMenuController.setIndexTab(0);
        UIController.Instance.PopUiOutStack();

        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        var keybox = FindObjectOfType<KeyBoxBtnController>();
        if (keybox != null && keybox.CanUnboxClaimRestaurantReward())
            yield return new WaitForSeconds(0.3f);
        animator.Play("Disappear");
        yield return new WaitForSeconds(0.25f);
        Panel.SetActive(false);
        levelSelectorController.HidePanel();
        if (activeMachineUpgrade != null)
            activeMachineUpgrade.HidePanel();
        if (activeIngredientUpgrade != null)
            activeIngredientUpgrade.HidePanel();
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        if (lastestPassedLevel != null && DataController.Instance.currentChapter > 0)
        {//we lose last game
            if (!hasShowFreeCoin)
            {
                var upgradeableMachines = DataController.Instance.GetAllUpgradeableMachines();
                var upgradeableIngredients = DataController.Instance.GetAllUpgradeableIngredients();
                bool canShowFreeCoin = false;
                bool adsReady = AdsController.Instance.IsRewardVideoAdsReady() && AdsController.Instance.CanShowAds("menuRewardCoin");
                if (upgradeableMachines.Count == 0 && upgradeableIngredients.Count == 0)
                    canShowFreeCoin = true;
                if (PlayerPrefs.GetInt("close_ruby_upgrade", 0) == 1)
                {
                    PlayerPrefs.SetInt("close_ruby_upgrade", 0);
                    canShowFreeCoin = true;
                }
                if (canShowFreeCoin && adsReady)
                {
                    int goldAmount = 0;
                    float randValue = Random.Range(0f, 100f);
                    if (randValue < 70)
                        goldAmount = Random.Range(100, 200);
                    else if (randValue < 90)
                        goldAmount = Random.Range(200, 300);
                    else if (randValue < 98)
                        goldAmount = Random.Range(300, 400);
                    else
                        goldAmount = Random.Range(400, 500);
                    FreeCoinPanelController freeCoinPanel = Instantiate(freeCoinPanelPrefab, overlayCanvas).GetComponent<FreeCoinPanelController>();
                    freeCoinPanel.Init(goldAmount);
                    mainMenuController.setIndexTab(5);
                    hasShowFreeCoin = true;
                }
            }
        }
    }
    public void OnClickIngredientUpgrade()
    {
        mainMenuController.setIndexTab(2);
        levelSelectorController.HidePanel();
        if (activeMachineUpgrade != null)
            activeMachineUpgrade.HidePanel();
        activeIngredientUpgrade = ingredientUpgrades[DataController.Instance.currentChapter - 1].Spawn(transform.GetChild(0).GetChild(0));
        ingredientNotiIcon.SetActive(false);
    }
    public void OnClickMachineUpgrade()
    {
        mainMenuController.setIndexTab(3);
        levelSelectorController.HidePanel();
        if (activeIngredientUpgrade != null)
            activeIngredientUpgrade.HidePanel();
        activeMachineUpgrade = machineUpgrades[DataController.Instance.currentChapter - 1].Spawn(transform.GetChild(0).GetChild(0));
        machineNotiIcon.SetActive(false);
    }
    public void OnClickLevelSelect()
    {
        mainMenuController.setIndexTab(1);
        if (activeIngredientUpgrade != null)
            activeIngredientUpgrade.HidePanel();
        if (activeMachineUpgrade != null)
            activeMachineUpgrade.HidePanel();
        levelSelectorController.Init(DataController.Instance.currentChapter);

    }
    public void OpenDetailPanel(int panelIndex = 1)
    {
#if UNITY_EDITOR
        CheatBtn.SetActive(true);
#else
        CheatBtn.SetActive(DataController.Instance.deviceIdData.CheckDeviceId(DataController.Instance.GetGameData().userID) || DataController.Instance.deviceIdData.CheckDeviceId(SystemInfo.deviceUniqueIdentifier));
#endif
        int currentChapter = DataController.Instance.currentChapter;
        if (PlayerPrefs.GetInt("level6_2_tut", 0) == 1)//handle old data
        {
            if (!DataController.Instance.GetTutorialData().Contains(62))
                DataController.Instance.GetTutorialData().Add(62);
        }
        if (!DataController.Instance.GetTutorialData().Contains(62)
        && currentChapter == 1
        && DataController.Instance.GetFocusKeyLevel(1) == 11
        && DataController.Instance.GetLevelState(1, 11) == 1)
        {
            UIController.Instance.CanBack = false;
            Instantiate(level6_2TutorialPanelPrefab);
            mainMenuController.setIndexTab(5);
            DataController.Instance.GetTutorialData().Add(62);
        }
        StartCoroutine(DelayOpenDetailPanel(panelIndex));
        // OnShow();
    }
    public IEnumerator DelayOpenDetailPanel(int panelIndex)
    {
        UIController.Instance.PushUitoStack(this);
        yield return new WaitForSeconds(0.1f);
        int currentChapter = DataController.Instance.currentChapter;
        Panel.SetActive(true);
        restaurantName.text = DataController.Instance.GetRestaurantById(currentChapter).restaurantName;
        ingredientNotiIcon.SetActive(ingredientUpgrades[currentChapter - 1].IsIngredientUpgradeAvailable());
        machineNotiIcon.SetActive(machineUpgrades[currentChapter - 1].IsMachineUpgradeAvailable());
        if (panelIndex == 1)
            OnClickLevelSelect();
        else if (panelIndex == 2)
            OnClickIngredientUpgrade();
        else
            OnClickMachineUpgrade();
        animator.Play("Appear");
        AudioController.Instance.PlaySfx(popUpClip);
    }
    public IngredientUpgradeController ActiveIngredientUpgrade
    {
        set { activeIngredientUpgrade = value; }
        get { return activeIngredientUpgrade; }
    }
    public MachineUpgradeController ActiveMachineUpgrade
    {
        set { activeMachineUpgrade = value; }
        get { return activeMachineUpgrade; }
    }
#region Cheat
    public void CheatKey()
    {
        var currentLevel = LevelDataController.Instance.currentLevel;
        int currentStar = DataController.Instance.GetLevelState(currentLevel.chapter, currentLevel.id);
        currentStar++;
        if (currentStar > 3) currentStar = 3;
        DataController.Instance.SetLevelState(currentLevel.chapter, currentLevel.id, currentStar);
        levelSelectorController.Init(DataController.Instance.currentChapter);
    }
#endregion
}
