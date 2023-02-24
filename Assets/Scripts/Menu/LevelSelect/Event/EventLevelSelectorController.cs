using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
public class EventLevelSelectorController : MonoBehaviour
{
    public int eventResId = 51;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private SubLevelBtnController[] subLevelBtns;
    [SerializeField] private Image objectiveIcon, limitIcon, conditionIcon;
    [SerializeField] private TextMeshProUGUI objectiveText, restaurantText;
    [SerializeField] private Sprite[] objectiveSprites, limitSprites, conditionSprites;
    [SerializeField] private Transform levelContainer, playBtn;
    [Header("Prefab")]
    [SerializeField] private EventLevelBtnContainer levelContainerPrefab;
    [SerializeField] private GameObject tapParticle;
    private EventLevelBtnController currentLevelBtn;
    private List<GameObject> activeLvlContainer = new List<GameObject>();
    private bool isScrollDirty, isLockUI;
    private int targetScrollValue, focusLevelIndex;
    private RectTransform levelContainerRect;
    private IEnumerator Start()
    {
        levelContainerRect = levelContainer.GetComponent<RectTransform>();
        yield return new WaitForSeconds(0.25f);
        isScrollDirty = false;
        if (EventDataController.Instance.currentChapter > 0 && LevelDataController.Instance.currentLevel.chapter == eventResId)
            Init(eventResId);
    }
    private void Update()
    {
        if (isScrollDirty)
        {
            float deltaPos = targetScrollValue - levelContainerRect.anchoredPosition.y;
            float sign = deltaPos / Mathf.Abs(deltaPos);
            float baseSpeed = Mathf.Max(500f, 2 * deltaPos * sign);
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
    public int GetFocusLevelIndex()
    {
        return focusLevelIndex;
    }
    public void OnClickOpenLevelBox()
    {
        int chapter = EventDataController.Instance.currentChapter;//grant player reward and unlock new level
        levelSelectPanel.SetActive(true);
        int[] levelsIndex = null;
        int[] levelProgressUnlock = EventDataController.Instance.GetLevelProgressUnlock(chapter);//get the level progress unlock 
        int lastestLevel = EventDataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        int totalLevels;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        switch (lastestLevel)
        {
            case 5:
                FindObjectOfType<RewardPanelController>().Init(0, 0, 60, new int[1] { (int)ItemType.Second_Chance }, new int[1] { 3 }, false);
                break;
            case 10:
                FindObjectOfType<RewardPanelController>().Init(0, 50, 120, new int[7] {  (int)ItemType.Anti_OverCook,(int)ItemType.Double_Coin,(int)ItemType.Instant_Cook,(int)ItemType.Add_Customers,(int)ItemType.Add_Seconds,(int)ItemType.Second_Chance,(int)ItemType.Pudding
                }, new int[7] { 1, 1, 1, 1, 1, 1, 1 }, false);//reward all boost x1
                break;
            case 15:
                FindObjectOfType<RewardPanelController>().Init(0, 50, 120, new int[7] {  (int)ItemType.Anti_OverCook,(int)ItemType.Double_Coin,(int)ItemType.Instant_Cook,(int)ItemType.Add_Customers,(int)ItemType.Add_Seconds,(int)ItemType.Second_Chance,(int)ItemType.Pudding
                }, new int[7] { 2, 2, 2, 2, 2, 2, 2 }, false);//reward all boost x2
                break;
        }
        EventDataController.Instance.SetRestaurantRewarded(chapter, (lastestLevel - 1) / 5);
        if (index == levelProgressUnlock.Length) return;
        totalLevels = levelProgressUnlock[index];//increase the level progress unlock threshold
        int activeContainerCount = activeLvlContainer.Count;
        bool isLastContainer = false;
        int totalKeys = 0;
        LevelData levelData = null;
        for (int i = 1; i < totalLevels + 1; i++)//check if box has been click and init the level
        {
            levelData = LevelDataController.Instance.GetEventLevelData(chapter, i);
            if (EventDataController.Instance.GetLevelState(chapter, i) >= levelData.key)//check if key is granted
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
                activeLvlContainer[i / 3].GetComponent<EventLevelBtnContainer>().Init(chapter, levelsIndex, isLastContainer);
        }
        if (!isLastContainer && totalKeys != totalLevels)
        {
            lvlContainer = levelContainerPrefab.Spawn(chapter, new int[0], levelContainer, true);
            activeLvlContainer.Add(lvlContainer);
        }
        focusLevelIndex = EventDataController.Instance.GetNextLevel(chapter);
        targetScrollValue = GetContentScrollPos(focusLevelIndex);
        isScrollDirty = true;
    }
    public void Init(int chapter)
    {
        for (int i = activeLvlContainer.Count - 1; i > -1; i--)
            Destroy(activeLvlContainer[i]);
        activeLvlContainer.Clear();
        EventDataController.Instance.currentChapter = chapter;
        restaurantText.text = EventDataController.Instance.GetRestaurantById(chapter).restaurantName;
        levelSelectPanel.SetActive(true);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        int[] levelsIndex = null;
        GameObject lvlContainer;
        int[] levelProgressUnlock = EventDataController.Instance.GetLevelProgressUnlock(chapter);//get the level progress unlock 
        int lastestLevel = EventDataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        int totalLevels;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        int totalKeys = 0;
        LevelData levelData = null;
        for (int i = 1; i < totalLevels + 1; i++)//check if box has been click and init the level
        {
            levelData = LevelDataController.Instance.GetEventLevelData(chapter, i);
            if (EventDataController.Instance.GetLevelState(chapter, i) >= levelData.key)//check if key is granted
                totalKeys++;
        }
        if (!EventDataController.Instance.HasRestaurantRewarded(chapter, (lastestLevel - 1) / 5))
            totalLevels = levelProgressUnlock[index - 1];
        else
            totalLevels = levelProgressUnlock[Mathf.Min(index, levelProgressUnlock.Length - 1)];
        focusLevelIndex = EventDataController.Instance.GetNextLevel(chapter);
        int focusKeyIndex = EventDataController.Instance.GetFocusKeyLevel(chapter);
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
                isLastContainer = totalKeys != totalLevels || !EventDataController.Instance.HasRestaurantRewarded(chapter, (totalLevels - 1) / 5);
                levelsIndex = new int[totalLevels + 1 - i];
                for (int j = 0; j < totalLevels + 1 - i; j++)
                    levelsIndex[j] = i + j;
            }
            lvlContainer = levelContainerPrefab.Spawn(chapter, levelsIndex, levelContainer, isLastContainer);
            activeLvlContainer.Add(lvlContainer);
        }
        int focusLevel = -1;
        if (!isLastContainer && (totalKeys != totalLevels || !EventDataController.Instance.HasRestaurantRewarded(chapter, (lastestLevel - 1) / 5)))
        {
            lvlContainer = levelContainerPrefab.Spawn(chapter, new int[0], levelContainer, true);
            activeLvlContainer.Add(lvlContainer);
        }
        if (totalKeys == totalLevels && !EventDataController.Instance.HasRestaurantRewarded(chapter, (lastestLevel - 1) / 5))
            focusLevel = totalLevels;
        else
            if (lastestPassedLevel != null && lastestPassedLevel.chapter == eventResId)
            focusLevel = lastestPassedLevel.id;
        if (focusLevel > -1)
        {
            targetScrollValue = GetContentScrollPos(focusLevel);
            isScrollDirty = true;
        }
    }
    public void HidePanel()
    {
        StartCoroutine(DelayHidePanel());
    }
    private IEnumerator DelayHidePanel()
    {
        levelSelectPanel.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        for (int i = activeLvlContainer.Count - 1; i > -1; i--)
            Destroy(activeLvlContainer[i]);
        activeLvlContainer.Clear();
        levelSelectPanel.SetActive(false);
    }
    public void OnSelectLevel(EventLevelBtnController levelBtn)//load all level information and display in the level detail panel
    {
        LevelData levelData = LevelDataController.Instance.currentLevel;
        int subLevel = EventDataController.Instance.GetLevelState(levelData.chapter, levelData.id);
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
        if (EventDataController.Instance.GetLevelState(levelData.chapter, levelData.id) < 1)
        {
            playBtn.gameObject.SetActive(true);
            System.Action<ITween<Vector3>> updatePlayBtnScale = (t) =>
                       {
                           playBtn.localScale = t.CurrentValue;
                       };
            TweenFactory.Tween("PlayBtn" + Time.time, playBtn.localScale, new Vector3(1.9f, 1.9f, 1), 0.1f, TweenScaleFunctions.QuinticEaseOut, updatePlayBtnScale)
            .ContinueWith(new Vector3Tween().Setup(new Vector3(1.9f, 1.9f, 1), new Vector3(1.7f, 1.7f, 1), 0.1f, TweenScaleFunctions.QuinticEaseInOut, updatePlayBtnScale))
            .ContinueWith(new Vector3Tween().Setup(new Vector3(1.7f, 1.7f, 1), new Vector3(1.77f, 1.77f, 1f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updatePlayBtnScale))
            ;
        }
        else
            playBtn.gameObject.SetActive(false);
        if (!isScrollDirty)
        {
            targetScrollValue = GetContentScrollPos(levelBtn.index);
            isScrollDirty = true;
        }
        MessageManager.Instance.SendMessage(
            new Message(CookingMessageType.OnOpenLevelSelect, new object[] { levelData.chapter, levelData.id }));
    }
    public void OnClickPlay()
    {
        if (isScrollDirty || isLockUI) return;
        isLockUI = true;
        System.Action<ITween<Vector3>> updatePlayBtnScale = (t) =>
                   {
                       playBtn.localScale = t.CurrentValue;
                   };
        TweenFactory.Tween("PlayBtn" + Time.time, playBtn.localScale, new Vector3(1.9f, 1.9f, 1), 0.1f, TweenScaleFunctions.QuinticEaseOut, updatePlayBtnScale)
            .ContinueWith(new Vector3Tween().Setup(new Vector3(1.9f, 1.9f, 1), new Vector3(1.7f, 1.7f, 1), 0.1f, TweenScaleFunctions.QuinticEaseInOut, updatePlayBtnScale))
            .ContinueWith(new Vector3Tween().Setup(new Vector3(1.7f, 1.7f, 1), new Vector3(1.77f, 1.77f, 1f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updatePlayBtnScale))
            ;
        GameObject go = Instantiate(tapParticle, playBtn.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
        var level = LevelDataController.Instance.currentLevel;
        int currentLevel = (level.chapter - 1) * 100 + level.id;
        if (PlayerPrefs.GetInt("current_level", 0) < currentLevel)
        {
            PlayerPrefs.SetInt("current_level", currentLevel);
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("current_level", level.chapter + "_" + level.id);
        }
        StartCoroutine(DelayLoadLevel());
    }
    private IEnumerator DelayLoadLevel()
    {
        yield return new WaitForSeconds(1);
        LevelDataController.Instance.lastestPassedLevel = null;
        LevelDataController.Instance.collectedGold = 0;
        string gameScene = EventDataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
        if (DataController.Instance.IsUnlimitedEnergy())
            SceneController.Instance.LoadScene(gameScene);
        else if (DataController.Instance.Energy > 0)
        {
            DataController.Instance.Energy--;
            foreach (int itemId in LevelDataController.Instance.items)
                DataController.Instance.UseItem(itemId, 1);
            SceneController.Instance.LoadScene(gameScene);
        }
        else
            FindObjectOfType<EnergyController>().OpenBuyEnergyPanel();
        isLockUI = false;
    }
}
