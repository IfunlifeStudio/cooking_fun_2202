using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
public class LevelSelectorController : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private SubLevelBtnController[] subLevelBtns;
    [SerializeField] private Image objectiveIcon, limitIcon, conditionIcon;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private Sprite[] objectiveSprites, limitSprites, conditionSprites;
    [SerializeField] public Transform levelContainer, playBtn;
    [SerializeField] private GameObject notify, extraJob, subLevelGo, detailLevel;
    [Header("Prefab")]
    [SerializeField] private LevelBtnContainer levelContainerPrefab;
    [SerializeField] private GameObject tapParticle;
    [Header("Battle Pass")]
    [SerializeField] private GameObject sock;
    [SerializeField] private TextMeshProUGUI sockQuantity;
    private LevelBtnController currentLevelBtn;
    private List<GameObject> activeLvlContainer = new List<GameObject>();
    private bool isScrollDirty, isLockUI;
    private int targetScrollValue, focusLevelIndex;
    private RectTransform levelContainerRect;
    private void Start()
    {
        levelContainerRect = levelContainer.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (isScrollDirty)
        {
            float deltaPos = targetScrollValue - levelContainerRect.anchoredPosition.y;
            float sign = deltaPos / Mathf.Abs(deltaPos);
            float baseSpeed = Mathf.Max(600f, 4 * deltaPos * sign);
            if (Mathf.Abs(deltaPos) > 20)
                levelContainerRect.anchoredPosition += new Vector2(0, sign * Time.deltaTime * baseSpeed);
            else
                isScrollDirty = false;
        }
    }
    private int GetContentScrollPos(int levelIndex)
    {
        int result = 0;
        if ((levelIndex - 1) / 3 > 1)
            result = 14 + Mathf.Max(0, ((levelIndex - 1) / 3 - 1)) * 227;
        if ((levelIndex - 1) / 3 >= activeLvlContainer.Count - 2 && activeLvlContainer.Count - 2 > 0)
            result = 14 + Mathf.Max(0, (activeLvlContainer.Count - 3)) * 227;
        return Mathf.Max(0, result);
    }
    public void FocusBoxKey()//get the last level index then focus to the box
    {
        int boxIndex = activeLvlContainer.Count * 3 - 1;
        targetScrollValue = GetContentScrollPos(boxIndex);
        isScrollDirty = true;
    }
    public void FocusCurrentLevel()//return focus to current level
    {
        int focus = DataController.Instance.GetFocusKeyLevel(DataController.Instance.currentChapter);
        targetScrollValue = GetContentScrollPos(focus);
        isScrollDirty = true;
        var levelData = LevelDataController.Instance.currentLevel;
        MessageManager.Instance.SendMessage(
            new Message(CookingMessageType.OnFocusLevel, new object[] { levelData.chapter, levelData.id }));
    }
    public void FocusLevel(int level)
    {
        targetScrollValue = GetContentScrollPos(level);
        isScrollDirty = true;
    }
    public int GetFocusLevelIndex()
    {
        DataController dataController = DataController.Instance;
        if (focusLevelIndex == 12 && dataController.currentChapter == 1 && dataController.GetLevelState(1, 11) < 2)
            focusLevelIndex = 11;
        return focusLevelIndex;
    }
    public bool IsEnableGiftBox()
    {
        int chapter = DataController.Instance.currentChapter;
        int[] levelProgressUnlock = DataController.Instance.GetLevelProgressUnlock(chapter);//get the level progress unlock 
        int lastestLevel = DataController.Instance.GetHighestLevel(chapter);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        int index = 0;
        int totalLevels;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        int totalKeys = 0;
        for (int i = 1; i < totalLevels + 1; i++)//check if box has been click and init the level
        {
            if (LevelDataController.Instance.GetLevelData(chapter, i).IsKeyGranted())
                totalKeys++;
        }
        //if ((lastestPassedLevel == null || !lastestPassedLevel.IsJustGrantKey()) && totalKeys == totalLevels)
        //    totalLevels = levelProgressUnlock[Mathf.Min(index, levelProgressUnlock.Length - 1)];
        focusLevelIndex = DataController.Instance.GetNextLevel(chapter);
        int focusKeyIndex = DataController.Instance.GetFocusKeyLevel(chapter);
        if (focusLevelIndex < focusKeyIndex)
            focusLevelIndex = focusKeyIndex;
        if (focusLevelIndex > totalLevels)
            focusLevelIndex = focusKeyIndex;
        if (focusLevelIndex > totalLevels)
            focusLevelIndex = totalLevels;
        if (totalKeys == totalLevels && focusLevelIndex == totalLevels)//reward player here
        {
            return true;
        }
        return false;
    }
    public void OnClickOpenLevelBox()
    {
        int chapter = DataController.Instance.currentChapter;
        levelSelectPanel.SetActive(true);
        int[] levelsIndex = null;
        int[] levelProgressUnlock = DataController.Instance.GetLevelProgressUnlock(chapter);//get the level progress unlock 
        int lastestLevel = DataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        int totalLevels;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        if (index == levelProgressUnlock.Length)//reward player here
        {

            FindObjectOfType<ProfileController>().ShowCertificateCover(chapter);
            return;
        }
        totalLevels = levelProgressUnlock[index];//increase the level progress unlock threshold
        int activeContainerCount = activeLvlContainer.Count;
        bool isLastContainer = false;
        int totalKeys = 0;
        for (int i = 1; i < totalLevels + 1; i++)//check if box has been click and init the level
        {
            if (LevelDataController.Instance.GetLevelData(chapter, i).IsKeyGranted())
                totalKeys++;
        }
        GameObject lvlContainer;
        for (int i = 1; i < totalLevels + 1; i += 3)//spawn level container.
        {
            isLastContainer = false;
            if (totalLevels - i > 1)
                levelsIndex = new int[] { i, i + 1, i + 2 };
            else
            {
                isLastContainer = true;
                levelsIndex = new int[totalLevels + 1 - i];
                for (int j = 0; j < totalLevels + 1 - i; j++)
                    levelsIndex[j] = i + j;
            }
            if (i / 3 >= activeContainerCount)
            {
                lvlContainer = levelContainerPrefab.Spawn(chapter, levelsIndex, levelContainer, isLastContainer);
                activeLvlContainer.Add(lvlContainer);
            }
            else
                activeLvlContainer[i / 3].GetComponent<LevelBtnContainer>().Init(chapter, levelsIndex, isLastContainer);
        }
        if (!isLastContainer && totalKeys != totalLevels)
        {
            lvlContainer = levelContainerPrefab.Spawn(chapter, new int[0], levelContainer, true);
            activeLvlContainer.Add(lvlContainer);
        }
        focusLevelIndex = DataController.Instance.GetNextLevel(chapter);
        targetScrollValue = GetContentScrollPos(focusLevelIndex);
        isScrollDirty = true;
    }
    public void Init(int chapter)
    {
        for (int i = activeLvlContainer.Count - 1; i > -1; i--)
            Destroy(activeLvlContainer[i]);
        activeLvlContainer.Clear();
        levelSelectPanel.SetActive(true);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        int[] levelsIndex = null;
        GameObject lvlContainer;
        int[] levelProgressUnlock = DataController.Instance.GetLevelProgressUnlock(chapter);//get the level progress unlock 
        int lastestLevel = DataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        int totalLevels;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        int totalKeys = 0;
        for (int i = 1; i < totalLevels + 1; i++)//check if box has been click and init the level
        {
            if (LevelDataController.Instance.GetLevelData(chapter, i).IsKeyGranted())
                totalKeys++;
        }
        if ((lastestPassedLevel == null || !lastestPassedLevel.IsJustGrantKey()) && totalKeys == totalLevels)
            totalLevels = levelProgressUnlock[Mathf.Min(index, levelProgressUnlock.Length - 1)];
        focusLevelIndex = DataController.Instance.GetNextLevel(chapter);
        int focusKeyIndex = DataController.Instance.GetFocusKeyLevel(chapter);
        if (focusLevelIndex < focusKeyIndex)
            focusLevelIndex = focusKeyIndex;
        if (focusLevelIndex > totalLevels)
            focusLevelIndex = focusKeyIndex;
        if (focusLevelIndex > totalLevels)
            focusLevelIndex = totalLevels;
        bool isLastContainer = false;
        for (int i = 1; i < totalLevels + 1; i += 3)//spawn level container.
        {
            if (totalLevels - i > 1)
                levelsIndex = new int[] { i, i + 1, i + 2 };
            else
            {
                isLastContainer = !DataController.Instance.HasRestaurantRewarded(chapter);
                levelsIndex = new int[totalLevels + 1 - i];
                for (int j = 0; j < totalLevels + 1 - i; j++)
                    levelsIndex[j] = i + j;
            }
            lvlContainer = levelContainerPrefab.Spawn(chapter, levelsIndex, levelContainer, isLastContainer);
            activeLvlContainer.Add(lvlContainer);
        }
        int focusLevel = -1;
        if (!isLastContainer && (totalKeys != totalLevels || !DataController.Instance.HasRestaurantRewarded(chapter)))
        {
            lvlContainer = levelContainerPrefab.Spawn(chapter, new int[0], levelContainer, true);
            activeLvlContainer.Add(lvlContainer);
        }
        if (totalKeys == totalLevels && !DataController.Instance.HasRestaurantRewarded(chapter))
            focusLevel = totalLevels;
        else
            if (lastestPassedLevel?.chapter == chapter)
            focusLevel = lastestPassedLevel.id;
        if (focusLevel > -1)
        {
            targetScrollValue = GetContentScrollPos(focusLevel);
            isScrollDirty = true;
        }
    }
    public void HidePanel()
    {
        levelSelectPanel.SetActive(false);
        if (isScrollDirty) return;
        for (int i = activeLvlContainer.Count - 1; i > -1; i--)
            Destroy(activeLvlContainer[i]);
        activeLvlContainer.Clear();
    }
    public void OnSelectLevel(LevelBtnController levelBtn)//load all level information and display in the level detail panel
    {
        LevelData levelData = LevelDataController.Instance.currentLevel;
        int subLevel = DataController.Instance.GetLevelState(levelData.chapter, levelData.id);
        for (int i = 0; i < subLevelBtns.Length; i++)
            subLevelBtns[i].Init(subLevel > i, levelData.key == (i + 1));
        objectiveIcon.sprite = objectiveSprites[levelData.ObjectiveType() - 1];
        objectiveText.text = levelData.Objective().ToString();
        limitIcon.sprite = limitSprites[levelData.LimitType() - 1];
        if (levelData.ConditionType() > 0)
        {
            conditionIcon.gameObject.SetActive(true);
            conditionIcon.sprite = conditionSprites[levelData.ConditionType() - 1];
        }
        else
            conditionIcon.gameObject.SetActive(false);
        if (currentLevelBtn != null)
        {
            if (currentLevelBtn != levelBtn)
            {
                var items = FindObjectsOfType<ItemBtnController>();
                foreach (var item in items)
                    item.OnSelectLevelChange();
            }
            currentLevelBtn.OnLostFocus();
        }
        currentLevelBtn = levelBtn;
        ShowDetailLevel(levelData.chapter, levelData.id);

        if (!isScrollDirty)
        {
            targetScrollValue = GetContentScrollPos(levelBtn.index);
            isScrollDirty = true;
        }
        MessageManager.Instance.SendMessage(
            new Message(CookingMessageType.OnOpenLevelSelect, new object[] { levelData.chapter, levelData.id }));
    }
    public void ShowDetailLevel(int chapter, int id) //show detail level in detail panel when selected level
    {
        var btn = playBtn;
        if (DataController.Instance.GetLevelState(chapter, id) == 3 &&
            DataController.Instance.ExtraJobData.IsExtraJob(chapter, id))
        {
            playBtn.gameObject.SetActive(true);
            extraJob.SetActive(true);
            subLevelGo.SetActive(false);
            detailLevel.SetActive(true);
            notify.gameObject.SetActive(false);
            LevelDataController.Instance.IsExtraJob = true;
            //sock.SetActive(false);
        }
        else if (DataController.Instance.GetLevelState(chapter, id) == 3 &&
            !DataController.Instance.ExtraJobData.IsExtraJob(chapter, id))
        {
            playBtn.gameObject.SetActive(false);
            notify.gameObject.SetActive(true);
            btn = notify.GetComponentInChildren<Button>().gameObject.transform;
            detailLevel.SetActive(false);
            extraJob.SetActive(false);
            subLevelGo.SetActive(true);
            LevelDataController.Instance.IsExtraJob = false;
        }
        else
        {
            playBtn.gameObject.SetActive(true);
            sock.SetActive(BattlePassDataController.Instance.CanClaimExp && DataController.Instance.IsShowNoel == 1);
            int exp = 5;
            if (id > 10 && id <= 20) exp = 10;
            else if (id > 20 && id <= 40) exp = 15;
            sockQuantity.text = "x" + exp;
            notify.gameObject.SetActive(false);
            detailLevel.SetActive(true);
            extraJob.SetActive(false);
            subLevelGo.SetActive(true);
            LevelDataController.Instance.IsExtraJob = false;
        }

    }
    public void OnClickPlay()
    {
        if (/*isScrollDirty ||*/ isLockUI) return;
        isLockUI = true;

        var levelSelectHelper = FindObjectOfType<LevelSelectHelper>();
        if (levelSelectHelper.CanShowHelper())
        {
            isLockUI = false;
            levelSelectHelper.OpenHelperPanel();
            FindObjectOfType<MainMenuController>().setIndexTab(5);
            return;
        }
        StartCoroutine(DelayLoadLevel());
    }
    public void OnFindExtraJob()
    {
        int chapter = DataController.Instance.currentChapter;
        int lastestLevel = DataController.Instance.GetHighestLevel(chapter);
        int extraJob_Id = DataController.Instance.ExtraJobData.GetExtraJobId(chapter);
        if (lastestLevel >= extraJob_Id && DataController.Instance.GetLevelState(chapter, extraJob_Id) <= 3)
        {
            FocusLevel(extraJob_Id);
            GetLevelBtn(chapter, extraJob_Id).OnSelectLevel();
        }
        else
        {
            FocusCurrentLevel();
            int focus = DataController.Instance.GetFocusKeyLevel(DataController.Instance.currentChapter);
            var nextFocusLevel = GetLevelBtn(chapter, focus);
            if (nextFocusLevel != null)
                GetLevelBtn(chapter, focus).OnSelectLevel();
            else
                FindObjectOfType<KeyBoxBtnController>().OnClickBox();
        }
    }
    private IEnumerator DelayLoadLevel()
    {
        yield return new WaitForSeconds(0.6f);
        LevelDataController.Instance.lastestPassedLevel = null;
        LevelDataController.Instance.collectedGold = 0;
        string gameScene = DataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
        if (LevelDataController.Instance.currentLevel.chapter == 1
        && LevelDataController.Instance.currentLevel.id == 8
        && DataController.Instance.HasRestaurantTutorial(111118))
            gameScene = "Restaurant1_2";
        if (LevelDataController.Instance.currentLevel.chapter == 1
       && LevelDataController.Instance.currentLevel.id == 2
       && DataController.Instance.HasRestaurantTutorial(111112))
            gameScene = "Restaurant1_5";
        if (LevelDataController.Instance.currentLevel.chapter == 1
        && LevelDataController.Instance.currentLevel.id == 10
        && DataController.Instance.HasRestaurantTutorial(1111110))
            gameScene = "Restaurant1_3";
        if (LevelDataController.Instance.currentLevel.chapter == 2
        && LevelDataController.Instance.currentLevel.id == 1
        && DataController.Instance.HasRestaurantTutorial(111121))
            gameScene = "Restaurant12_1";
        if (LevelDataController.Instance.currentLevel.chapter == 1
            && LevelDataController.Instance.currentLevel.id == 13
            && DataController.Instance.HasRestaurantTutorial(1111113)
            && !DataController.Instance.HasClaimFree((int)ItemType.Pudding))
        {
            if (DataController.Instance.IsUnlimitedEnergy() || DataController.Instance.Energy > 0)//can play level so we claim free pudding
            {
                DataController.Instance.AddItem((int)ItemType.Pudding, 3);
                DataController.Instance.ClaimFree((int)ItemType.Pudding);
                DataController.Instance.SaveData(false);
            }
            gameScene = "Restaurant1_4";
        }
        if (LevelDataController.Instance.currentLevel.chapter == 3
            && LevelDataController.Instance.currentLevel.id == 1
            && DataController.Instance.HasRestaurantTutorial(111131))
            gameScene = "Restaurant13_1";
        if (LevelDataController.Instance.currentLevel.chapter == 4
           && LevelDataController.Instance.currentLevel.id == 1
           && DataController.Instance.HasRestaurantTutorial(111141))
            gameScene = "Restaurant14_1";
        if (LevelDataController.Instance.currentLevel.chapter == 5
           && LevelDataController.Instance.currentLevel.id == 1
           && DataController.Instance.HasRestaurantTutorial(111151))
            gameScene = "Restaurant7_1";
        if (LevelDataController.Instance.currentLevel.chapter == 6
           && LevelDataController.Instance.currentLevel.id == 1
           && DataController.Instance.HasRestaurantTutorial(111161))
            gameScene = "Restaurant8_1";
        if (LevelDataController.Instance.currentLevel.chapter == 7
          && LevelDataController.Instance.currentLevel.id == 1
          && DataController.Instance.HasRestaurantTutorial(111171))
            gameScene = "Restaurant9_1";
        if (LevelDataController.Instance.currentLevel.chapter == 8
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(111181))
            gameScene = "Restaurant10_1";
        if (LevelDataController.Instance.currentLevel.chapter == 9
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(111191))
            gameScene = "Restaurant2_1";
        if (LevelDataController.Instance.currentLevel.chapter == 10
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(1111101))
            gameScene = "Restaurant3_1";
        if (LevelDataController.Instance.currentLevel.chapter == 11
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(1111111))
            gameScene = "Restaurant4_1";
        if (LevelDataController.Instance.currentLevel.chapter == 12
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(1111121))
            gameScene = "Restaurant5_1";
        if (LevelDataController.Instance.currentLevel.chapter == 13
         && LevelDataController.Instance.currentLevel.id == 1
         && DataController.Instance.HasRestaurantTutorial(1111131))
            gameScene = "Restaurant6_1";

        if (LevelDataController.Instance.currentLevel.chapter == 14
        && LevelDataController.Instance.currentLevel.id == 1
        && DataController.Instance.HasRestaurantTutorial(1111141))
            gameScene = "Restaurant19_1";

        if (LevelDataController.Instance.currentLevel.chapter == 15
        && LevelDataController.Instance.currentLevel.id == 1
        && DataController.Instance.HasRestaurantTutorial(1111151))
            gameScene = "Restaurant16_1";
        if (LevelDataController.Instance.currentLevel.chapter == 15
       && LevelDataController.Instance.currentLevel.id == 5
       && DataController.Instance.HasRestaurantTutorial(1111155))
            gameScene = "Restaurant16_2";

        if (LevelDataController.Instance.currentLevel.chapter == 16
        && LevelDataController.Instance.currentLevel.id == 1
        && DataController.Instance.HasRestaurantTutorial(1111161))
            gameScene = "Restaurant17_1";

        if (LevelDataController.Instance.currentLevel.chapter == 17
        && LevelDataController.Instance.currentLevel.id == 1
        && DataController.Instance.HasRestaurantTutorial(1111171))
            gameScene = "Restaurant18_1";

        if (DataController.Instance.IsUnlimitedEnergy())
        {
            foreach (int itemId in LevelDataController.Instance.items)
                DataController.Instance.UseItem(itemId, 1);
            DataController.Instance.SaveData(false);
            SceneController.Instance.LoadScene(gameScene);
        }
        else if (DataController.Instance.Energy > 0)
        {
            DataController.Instance.Energy--;
            foreach (int itemId in LevelDataController.Instance.items)
                DataController.Instance.UseItem(itemId, 1);
            DataController.Instance.SaveData(false);
            SceneController.Instance.LoadScene(gameScene);
        }
        else
        {
            FindObjectOfType<MainMenuController>().setIndexTab(5);
            FindObjectOfType<EnergyController>().OpenBuyEnergyPanel();
        }
        isLockUI = false;
    }
    public LevelBtnController GetLevelBtn(int chapter, int id)
    {
        List<LevelBtnController> levelBtns = new List<LevelBtnController>();
        foreach (var levelContainer in activeLvlContainer)
            levelBtns.AddRange(levelContainer.GetComponent<LevelBtnContainer>().levelBtns);
        foreach (var levelBtn in levelBtns)
            if (levelBtn.chapter == chapter && levelBtn.index == id)
                return levelBtn;
        return null;
    }
}
