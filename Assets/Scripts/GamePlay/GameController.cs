using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class GameController : MonoBehaviour
{
    [SerializeField] private AudioClip winAudio, loseAudio, backgroundMusic, objectiveUpdateAudio;
    [Header("UI")]
    [SerializeField] private PausePanelController pausePanel;
    [SerializeField] private WinPanelController winPanel;
    [SerializeField] private LosePanelController losePanel;
    [SerializeField] private EndGameItemPanel itemPanel;
    public Image objectiveIcon, objectiveProgress, limitIcon, conditionIcon;
    [SerializeField] private TextMeshProUGUI objectiveProgressText, limitText;
    [SerializeField] private Button btnCheat;
    [SerializeField] private Animator limitedTextAnimator, closeBoardAnim;
    [SerializeField] private Sprite[] objectiveSprites, limitSprites, conditionSprites;
    [SerializeField] private List<BoosterItem> boosterItems = new List<BoosterItem>();
    private GameObject progressSkeleton;
    [HideInInspector] public InstructionController instructionController;
    [HideInInspector] public Transform canvasWorld;
    [SerializeField] GameObject instructionPrefab;
    private Animator ObjecttiveIconAnim;
    ComboBarController comboBar;
    private float currentFillAmount, targetFillAmount, currentScale, timeStamp;
    private int tmpGain = 0, objectiveGain;
    private List<int> servedFoods = new List<int>(), throwFoods = new List<int>(), burnedFoods = new List<int>();
    private int totalUnservedCustomer = 0;//this is total customer leave without served
    private int totalServerCustomer = 0;
    private int totalCustomerVisited = 0;//this is total customer visited the restaurant
    private int totalGold/*total gold collected in game*/, totalCombo5 = 0, comboGold = 0, totalHeart, bonusCustomer = 0, bonusTime = 0/*total time bonus this level*/, itemUsed = 0/*number of bonus used*/;
    public float playTime = 0;
    [HideInInspector] public LevelData levelData;
    [HideInInspector] public int levelState;
    private bool waiting, isShowAds;
    private float instructionTime = 0;
    private List<int> activeItems = new List<int>();
    private ServePlateController[] plates;
    private ToppingIngredient[] toppings;
    private Machine[] machines;
    private IngredientHolder[] lstHolder;
    public bool isCheat, isPuddingActive = false, isTut = false, isEvent = false, isPlaying, isEndGame, isFlicker, isSentMessage, isGameWin;
    int loseType = 0;

    void Start()
    {
        Input.multiTouchEnabled = true;
        isSentMessage = false;
        UIController.Instance.ClearStack();
        comboBar = FindObjectOfType<ComboBarController>();
        canvasWorld = GameObject.Find("CanvasWorld").transform;
        instructionTime = Time.time;
        instructionController = Instantiate(instructionPrefab, transform).GetComponent<InstructionController>();
        AudioController.Instance.PlayMusic(backgroundMusic, true);
        levelData = LevelDataController.Instance.currentLevel;
        levelState = DataController.Instance.GetLevelState(levelData.chapter, levelData.id);
        objectiveIcon.sprite = objectiveSprites[levelData.ObjectiveType() - 1];
        ObjecttiveIconAnim = objectiveIcon.GetComponent<Animator>();
        objectiveProgress.fillAmount = 0f;
        objectiveProgressText.text = "0/" + levelData.Objective();
        progressSkeleton = objectiveProgress.transform.GetChild(0).gameObject;
        limitIcon.sprite = limitSprites[levelData.LimitType() - 1];
        limitText.text = levelData.Limit().ToString();
        //limitedTextAnimator = limitText.GetComponent<Animator>();
        if (levelData.ConditionType() > 0)
        {
            conditionIcon.gameObject.SetActive(true);
            conditionIcon.sprite = conditionSprites[levelData.ConditionType() - 1];
        }
        if (!LevelDataController.Instance.IsExtraJob)
            AchivementController.Instance.OnGameStart(levelData.chapter);
        var profileDatas = DataController.Instance.GetGameData().profileDatas;
        profileDatas.TotalPlayedGame += 1;
        DisplayActiveIcon();
        var level = levelData.chapter + "_" + levelData.id + "_" + levelState;
        DOVirtual.DelayedCall(1, () =>
        {
            APIController.Instance.LogEventLevelStart(level);
            if (!(Application.internetReachability == NetworkReachability.NotReachable))
            {
                LevelTracking eventTracking = new LevelTracking(level, "start");
                string request = JsonUtility.ToJson(eventTracking);
                APIController.Instance.PostData(request, Url.LevelTracking);
            }
        });
        StartCoroutine(DelayFindObject());
        if (!isTut)
            StartCoroutine(GameLoop());
        else
            UIController.Instance.CanBack = false;
        if (btnCheat != null)
            btnCheat.gameObject.SetActive(DataController.Instance.IsTestUser);
        int chapter_up = PlayerPrefs.GetInt("user_property_level", 1);
        if (chapter_up < levelData.chapter)
        {
            PlayerPrefs.GetInt("user_property_level", levelData.chapter);
            APIController.Instance.SetProperty("Chapter", levelData.chapter.ToString());
        }
        APIController.Instance.SetProperty("Level", level);
#if UNITY_EDITOR
        btnCheat.gameObject.SetActive(true);
#endif
        DataController.Instance.SaveData(false);
    }
    void Update()
    {
        float deltaTime = Time.time - timeStamp;
        if (deltaTime < 0.1f)
        {
            objectiveProgress.fillAmount = Mathf.Lerp(currentFillAmount, Mathf.Min(targetFillAmount, 1), 3.34f * Mathf.Sqrt(deltaTime));
            if (objectiveProgress.fillAmount == 1)
                progressSkeleton.SetActive(true);
            int currentObjective = (int)Mathf.Lerp(tmpGain, objectiveGain, 3.34f * Mathf.Sqrt(deltaTime));
            objectiveProgressText.text = currentObjective + "/" + levelData.Objective();
            currentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            transform.localScale = new Vector3(currentScale, currentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
            {
                objectiveProgress.fillAmount = Mathf.Min(targetFillAmount, 1);
                if (objectiveProgress.fillAmount == 1)
                    progressSkeleton.SetActive(true);
                objectiveProgressText.text = objectiveGain + "/" + levelData.Objective();
                transform.localScale = Vector3.one;
            }
        }
        if (!isTut && Time.time - instructionTime >= 8 && instructionController.handState == HandState.TurnOff /*&& ABtesting == "1"*/)
        {
            CanEnableInstructionHand();
        }
        if (Input.GetMouseButtonDown(0) /*&& ABtesting == "1"*/)
        {
            instructionTime = Time.time;
            if (instructionController.handState == HandState.TurnOn)
            {
                instructionController.TurnOff();
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnHidePanel();
        }
    }

    public void UpdateObjectiveGain(int value)
    {
        tmpGain = objectiveGain;
        objectiveGain = value;
        currentFillAmount = objectiveProgress.fillAmount;
        targetFillAmount = objectiveGain * 1f / levelData.Objective();
        timeStamp = Time.time;
        AudioController.Instance.PlaySfx(objectiveUpdateAudio);
    }
    public void ManualStartGameLoop()
    {
        UIController.Instance.CanBack = true;
        StartCoroutine(GameLoop());
    }
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStart());
        while (itemUsed < 4)
        {
            yield return StartCoroutine(GamePlay());
            if (itemUsed == 3)
            {
                Time.timeScale = 1;
                break;
            }
            isGameWin = (CheckGameCondition() || isCheat) && CheckObjective();
            if (isGameWin)
            {
                Time.timeScale = 1;
                break;
            }
            else
            {
                int itemId = 0;
                if (!CheckGameCondition())//player lose due to violate a level condition
                    itemId = 100300;//second chance
                else
                {
                    if (levelData.LimitType() == 1)//time limit
                        itemId = 100200;//+30 seconds
                    else//customer limit
                        itemId = 100100;// +3 customers
                }
                //yield return new WaitForSecondsRealtime(2f);
                itemPanel.Init(itemId);
                int currentItemUsed = itemUsed;
                waiting = true;
                while (waiting)
                {
                    yield return new WaitForSecondsRealtime(0.1f);//waiting for player to use the boost
                }
                Time.timeScale = 1;
                if (currentItemUsed == itemUsed)
                    break;//user didnt use any booster
            }
        }
        yield return StartCoroutine(GameEnd());
    }
    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0.5f);
        playTime = 0;
        CustomerFactory.Instance.SpawnWave();
    }
    private IEnumerator GamePlay()
    {
        isPlaying = true;
        while (isPlaying)
        {
            yield return null;
            if (!isPuddingActive)
                playTime += Time.deltaTime;
            if (CanSpawnNewWave())
                CustomerFactory.Instance.SpawnWave();
            DisplayGameLimit();
            if (!CheckGameLimit() || !CheckGameCondition())
            {
                if (levelData.LimitType() == 2 && !CheckGameLimit())
                    yield return new WaitForSeconds(1);
                isPlaying = false;
                yield return new WaitForSeconds(0.2f);
                Time.timeScale = 0;
            }
        }
    }
    private IEnumerator GameEnd()
    {
        isEndGame = true;
        closeBoardAnim.Play("ClosePanel");
        CustomerFactory.Instance.StopSpawn();
        activeItems.AddRange(LevelDataController.Instance.items);
        LevelDataController.Instance.ConsumeItems();
        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnGameEnd));
        int objectiveType = levelData.ObjectiveType();
        int conditionType = levelData.ConditionType() - 1;
        bool isConditionViolate = !CheckGameCondition();
        bool isTargetViolate = !CheckObjective();
        int objectiveGain = 0;
        switch (levelData.ObjectiveType())
        {
            case 1:
                objectiveGain = totalGold;
                break;
            case 2:
                objectiveGain = totalHeart;
                break;
            case 3:
                objectiveGain = servedFoods.Count;
                break;
        }
        //if (levelData.LimitType() == 1)
        //    yield return new WaitForSecondsRealtime(0.1f);//wait for customer to leave
        //else
        //{
        //    while (totalCustomerVisited < totalServerCustomer)
        //        yield return null;
        //}
        isGameWin = (CheckGameCondition() || isCheat) && CheckObjective();
        if (!isEvent)
            PlayerClassifyController.Instance.SaveGameResult(isGameWin, itemUsed > 0);
        yield return new WaitForSecondsRealtime(0.25f);
        Time.timeScale = 1;
        string strlevelState = levelData.chapter + "." + levelData.id + (levelState + 1);
        var level = levelData.chapter + "_" + levelData.id + "_" + levelState;
        yield return new WaitForSecondsRealtime(0.15f);
        IsShowAds = true;
        if (isGameWin)
        {
            PlayerPrefs.SetInt("CanShowUpgrade", 0);
            int goldReward = GetGoldsReward();
            DataController.Instance.IncreaseOneEnergy();
            LevelDataController.Instance.playedLevelData = new PlayedLevelData(levelData.chapter, levelData.id, 1, loseType, activeItems);
            MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnWinGame, new object[] { levelState, burnedFoods.Count, throwFoods.Count, totalUnservedCustomer }));
            winPanel.Init(goldReward, totalCombo5, totalHeart, servedFoods.Count);
            if (!isEvent)
            {
                LevelDataController.Instance.lastestPassedLevel = levelData;
                LevelDataController.Instance.collectedGold = goldReward;
            }
            //Set user property
            APIController.Instance.LogEventWinLevel(level);
            AudioController.Instance.PlayMusic(winAudio, false);
        }
        else
        {
            if (LevelDataController.Instance.lastestLoseLevel != null)
                //if (!isEvent) AchivementController.Instance.AddAchivementProgress(AchivementID.Achivement_2, 0);
                if (LevelDataController.Instance.lastestLoseLevel.chapter == levelData.chapter
                    && LevelDataController.Instance.lastestLoseLevel.id == levelData.id)
                    PlayerPrefs.SetInt("CanShowUpgrade", 1);
            SetIsFirstWinState(strlevelState);
            CheckGameloseCondition();
            CheckLoseObjective();
            LevelDataController.Instance.lastestLoseLevel = levelData;

            string objectiveProgress = "";
            if (objectiveGain < levelData.Objective())
                objectiveProgress = "<color=#FF0000>" + objectiveGain + "</color>" + "/" + levelData.Objective();
            else
                objectiveProgress = objectiveGain + "/" + levelData.Objective();
            LevelDataController.Instance.playedLevelData = new PlayedLevelData(levelData.chapter, levelData.id, 1, loseType, activeItems);
            losePanel.Init(objectiveType, objectiveProgress, isTargetViolate, isConditionViolate, levelData.ConditionType(), totalCombo5, totalHeart, servedFoods.Count);
            AudioController.Instance.PlayMusic(loseAudio, false);
        }
        if (!(Application.internetReachability == NetworkReachability.NotReachable))
        {
            LevelTracking eventTracking = new LevelTracking(level, isGameWin ? "win" : "lose");
            string request = JsonUtility.ToJson(eventTracking);
            APIController.Instance.PostData(request, Url.LevelTracking);
        }
    }
    public void LogFirstWinGSM(string gold, bool isDouble)
    {
        if (levelState < 3 && !(Application.internetReachability == NetworkReachability.NotReachable))
        {
            string strlevelState = levelData.chapter + "." + levelData.id + (levelState + 1);
            string level = levelData.chapter + "_" + levelData.id + "_" + levelState;
            int firstWinStage_count = PlayerPrefs.GetInt(strlevelState, 0);
            firstWinStage_count++;
            string x2gold = isDouble ? gold : "0";
            string numstr = firstWinStage_count + "_" + gold + "_" + x2gold;
            FirstWinTracking eventTracking = new FirstWinTracking("fw2_" + level, "firstwinv2", numstr);
            string request = JsonUtility.ToJson(eventTracking);
            APIController.Instance.PostData(request, Url.LevelTracking);
            if (PlayerPrefs.HasKey(strlevelState))
                PlayerPrefs.DeleteKey(strlevelState);
        }
    }
    private bool CanSpawnNewWave()
    {
        return !CustomerFactory.Instance.isWorking && levelData.LimitType() == 1 && CheckGameLimit();
    }
    #region Game Condition Checking
    private bool CheckGameCondition()
    {
        bool result = true;
        switch (levelData.ConditionType())
        {
            case 0:
                break;
            case 1:
                result = (burnedFoods.Count == 0);
                loseType = 100000;
                break;
            case 2:
                result = (throwFoods.Count == 0);
                loseType = 130000;
                break;
            case 3:
                result = (totalUnservedCustomer == 0);
                loseType = 100300;
                break;
        }
        return result;
    }
    private void CheckLoseObjective()
    {
        int objectiveGain = 0;
        switch (levelData.ObjectiveType())
        {
            case 1:
                objectiveGain = totalGold;
                break;
            case 2:
                objectiveGain = totalHeart;
                break;
            case 3:
                objectiveGain = servedFoods.Count;
                break;
        }
        if (objectiveGain < levelData.Objective())
            switch (levelData.ObjectiveType())
            {
                case 1:
                    if (loseType == 0)
                        loseType = 110000;
                    break;
                case 2:
                    if (loseType == 0)
                        loseType = 100300;
                    break;
                case 3:
                    if (loseType == 0)
                        loseType = 100300;
                    break;
            }
    }
    private void CheckGameloseCondition()
    {
        switch (levelData.ConditionType())
        {
            case 0:
                break;
            case 1:
                if (burnedFoods.Count != 0)
                    loseType = 100000;
                break;
            case 2:
                if (throwFoods.Count != 0)
                    loseType = 130000;
                break;
            case 3:
                if (totalUnservedCustomer != 0)
                    loseType = 100300;
                break;
        }
    }
    #endregion
    #region  Game Limit
    private bool CheckGameLimit()
    {
        bool result = true;
        switch (levelData.LimitType())
        {
            case 1://handle limit play time
                if (playTime >= levelData.Limit() + bonusTime)
                    result = false;
                break;
            case 2://handle limit customer 
                result = totalServerCustomer < levelData.Limit() + bonusCustomer;
                break;
        }
        return result;
    }
    private void DisplayGameLimit()
    {
        int result = 0;
        switch (levelData.LimitType())
        {
            case 1://handle limit play time
                result = Mathf.Max(0, levelData.Limit() + bonusTime - (int)playTime);
                loseType = 3;
                break;
            case 2://handle limit customer 
                result = levelData.Limit() + bonusCustomer - totalServerCustomer;
                loseType = 4;
                break;
        }
        if (!isFlicker)
            if (levelData.LimitType() == 1 && result <= 10)
            {
                isFlicker = true;
                limitedTextAnimator.Play("Flicker");
            }
        limitText.text = result.ToString();
    }
    #endregion
    private bool CheckObjective()
    {
        int objectiveGain = 0;
        switch (levelData.ObjectiveType())
        {
            case 1:
                objectiveGain = totalGold;
                break;
            case 2:
                objectiveGain = totalHeart;
                break;
            case 3:
                objectiveGain = servedFoods.Count;
                break;
        }
        return objectiveGain >= levelData.Objective();
    }
    private void UpdateObjectiveGain()
    {
        int _objectiveGain = 0;
        switch (levelData.ObjectiveType())
        {
            case 1:
                _objectiveGain = totalGold;
                break;
            case 2:
                _objectiveGain = totalHeart;
                break;
            case 3:
                _objectiveGain = servedFoods.Count;
                break;
        }
        tmpGain = objectiveGain;
        objectiveGain = _objectiveGain;
        currentFillAmount = objectiveProgress.fillAmount;
        targetFillAmount = objectiveGain * 1f / levelData.Objective();
        timeStamp = Time.time;
        AudioController.Instance.PlaySfx(objectiveUpdateAudio);
    }
    public void ScaleObjecttiveIcon()
    {
        UpdateObjectiveGain();
        ObjecttiveIconAnim.Play("ScaleObject");
    }
    private int GetGoldsReward()
    {
        int rewardGolds = 0;
        switch (levelData.ObjectiveType())
        {
            case 1:
                rewardGolds = totalGold;
                break;
            case 2:
                rewardGolds = totalHeart * 10 + comboGold;
                break;
            case 3:
                rewardGolds = servedFoods.Count * 5 + comboGold;
                break;
        }
        return rewardGolds;
    }
    #region Level Event Callback
    public void OnPlayerThrowFood(int id)//called when player double tap to throw a food
    {
        throwFoods.Add(id);
    }
    public void OnCustomerUnserved()//called when a customer is leaving because of time over
    {
        totalUnservedCustomer++;
    }
    public void OnCustomerServed()
    {
        totalServerCustomer++;
    }
    public void OnCustomerLeave()//called when a customer leave the camera view
    {
        totalCustomerVisited++;
    }
    public void OnCustomerGiveHeart()//called when a customer give a heart
    {
        totalHeart++;
        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnHeartReceived));
    }
    public void IncreaseGold(int value, bool isCombo)//called when customer pay for a food or get a combo
    {
        totalGold += value;
        if (isCombo)
            comboGold += value;
    }
    public void OnDoCombo5()
    {
        totalCombo5++;
    }
    public void OnFoodBurned(int foodId)//called when a food is burn
    {
        burnedFoods.Add(foodId);
    }
    public void OnFoodServed(int foodId)//called when a food is served to a customer
    {
        servedFoods.Add(foodId);
        comboBar.Combo++;
    }
    #endregion
    #region UI function
    public void OnClickPause()
    {
        pausePanel.Init();
        ShowInterstialAds();
    }
    private void ShowInterstialAds()
    {
        StartCoroutine(DelayShowInterstialAds());
    }
    private IEnumerator DelayShowInterstialAds()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        if (AdsController.Instance.CanShowIntersAds(LevelDataController.Instance.currentLevel.chapter, LevelDataController.Instance.currentLevel.id, 0))
            AdsController.Instance.ShowInterstitial(null);
    }
    public void OnClickCompleteLevel()
    {
        isCheat = true;
        switch (levelData.ObjectiveType())
        {
            case 1:
                totalGold = levelData.Objective();
                break;
            case 2:
                totalHeart = levelData.Objective();
                break;
            case 3:
                for (int i = 0; i < levelData.Objective(); i++)
                    servedFoods.Add(1);
                break;
        }
        switch (levelData.LimitType())
        {
            case 1://handle limit play time
                playTime = levelData.Limit() + 1;
                break;
            case 2://handle limit customer 
                totalServerCustomer = totalCustomerVisited = levelData.Limit();
                break;
        }
    }
    #endregion
    #region Item Functions
    public void OnClickAddSeconds(int seconds)
    {
        bonusTime += seconds;
        itemUsed += 1;
        waiting = false;//continue the game
        activeItems.Add((int)ItemType.Add_Seconds);
    }
    public void OnClickAddCustomer(int customers)
    {
        itemUsed += 1;
        waiting = false;
        bonusCustomer += customers;//add 3 customers        
        CustomerFactory.Instance.SpawnAdditionWave(3);//create 3 customers then spawns
        activeItems.Add((int)ItemType.Add_Customers);
    }
    public void OnClickAdd1Customer(int customers)
    {
        itemUsed += 1;
        waiting = false;
        bonusCustomer += customers;//add 1 customers        
        CustomerFactory.Instance.SpawnAdditionWave(1);//create 1 customers then spawns
        activeItems.Add((int)ItemType.Add_Customers);
    }
    public void SendMessageBurnedViolate(Vector3 machinePosition)
    {
        if (!isSentMessage)
        {
            isSentMessage = true;
            AudioController.Instance.Vibrate();
            MessageManager.Instance.SendMessage(
            new Message(CookingMessageType.OnConditionViolate,
            new object[] { levelData.ConditionType(), machinePosition }));
        }
    }
    public void OnClickSecondChance()
    {
        if (levelData.ConditionType() == 1)
        {
            Machine[] machines = FindObjectsOfType<Machine>();
            foreach (Machine machine in machines)
            {
                if (machine.state == MachineStates.Burned || (machine.timeRemain <= 1f && machine.state == MachineStates.Completed))
                {
                    machine.BackToWorkingStatus();
                }
            }
            burnedFoods.Clear();
        }
        else if (levelData.ConditionType() == 3)
        {
            foreach (var custom in CustomerFactory.Instance.activeCustomers)
            {
                custom.OnUseSecondChance();
            }
            CustomerFactory.Instance.isViolate = false;
            totalUnservedCustomer = 0;
        }
        throwFoods.Clear();
        itemUsed += 1;
        //CustomerFactory.Instance.OnPuddingActive();
        //isPuddingActive = true;
        waiting = false;//continue the game
        isSentMessage = false;
    }
    public void OnPlayerCancelItemPanel()
    {
        waiting = false;//continue the game
    }
    public void DisplayActiveIcon()
    {
        int index = 0;
        for (int i = 0; i < boosterItems.Count; i++)
        {
            if (LevelDataController.Instance.CheckItemActive(boosterItems[i].id))
            {
                boosterItems[i].booster.SetActive(true);
                float posY = boosterItems[i].booster.transform.position.y;

                if (Screen.width * 1f / Screen.height < 1.7f)
                {
                    boosterItems[i].booster.transform.position = new Vector2((4.4f + (0.8f * index)), posY);
                    boosterItems[i].booster.transform.localScale = new Vector2(0.8f, 0.8f);
                }
                else
                {
                    boosterItems[i].booster.transform.position = new Vector2((4.4f + index), posY);
                }
                if (boosterItems[i].id == 120000)
                    //AchivementController.Instance.AddAchivementProgress(levelData.chapter, AchivementID.Achivement_6);
                    DataController.Instance.GetGameData().GetCertificateDataAtRes(levelData.chapter).FlashHand++;
                index++;
                string para_leveldata = levelData.chapter + "_" + levelData.id + "_" + levelState;
                string itemName = DataController.Instance.GetItemName(boosterItems[i].id);
                APIController.Instance.LogEventUseItem(itemName, para_leveldata);
            }
        }
    }

    private Food GetFoddOnPlate()
    {
        for (int i = 0; i < plates.Length; i++)
        {
            Food activeFood = plates[i].activeFood;
            if (activeFood != null)
                return activeFood;
        }
        return null;
    }
    private bool CanEnableInstructionHand()
    {
        bool canShowHand = DataController.Instance.GetLevelState(1, 2) > 0;
        if (!canShowHand) return false;
        CustomerController[] customerPool = CustomerFactory.Instance.activeCustomers.ToArray();
        for (int i = 0; i < plates.Length; i++) //check food vs shortest time's customer
        {
            Food activeFood = plates[i].activeFood;
            if (activeFood != null)
                if (CustomerFactory.Instance.CheckFood(activeFood.FoodId))
                {
                    instructionController.Init(plates[i].transform.position, "click");
                    return true;
                }
        }
        List<int> holderID = new List<int>();
        foreach (IngredientHolder holder in lstHolder)
        {
            holderID.Add(holder.id);
        }
        for (int i = 0; i < machines.Length; i++)
        {
            if (machines[i].state == MachineStates.Completed && !holderID.Contains(machines[i].foodId) && CustomerFactory.Instance.CheckFood(machines[i].foodId))
            {
                instructionController.Init(machines[i].transform.position, "click");
                return true;
            }
        }
        for (int x = 0; x < customerPool.Length; x++)
        {
            foreach (Product product in customerPool[x]?.activeProducts)
            {
                if (product != null)
                {
                    int ingredientMajorId = product.foodBase.ingredientIds[0];
                    for (int j = 0; j < plates.Length; j++)
                    {
                        Food foodinPlates = plates[j].activeFood;
                        if (foodinPlates != null)
                        {
                            if (ingredientMajorId == foodinPlates.foodBase.ingredientIds[0])
                            {
                                for (int k = 0; k < toppings.Length; k++)
                                {
                                    int id = foodinPlates.FoodId + toppings[k].foodId;
                                    if (id == product.FoodId)
                                    {
                                        instructionController.Init(toppings[k].transform.position, "click");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    for (int j = 0; j < plates.Length; j++)
                    {
                        Food foodinPlates = plates[j].activeFood;
                        if (foodinPlates == null)
                        {
                            for (int i = 0; i < machines.Length; i++)
                            {
                                if (machines[i].state == MachineStates.Completed && ingredientMajorId == machines[i].foodId)
                                {
                                    instructionController.Init(machines[i].transform.position, "click");
                                    return true;
                                }
                            }
                            for (int i = 0; i < machines.Length; i++)
                            {
                                if (ingredientMajorId == machines[i].foodId && machines[i].state == MachineStates.Standby)
                                {
                                    for (int k = 0; k < lstHolder.Length; k++)
                                    {
                                        if (lstHolder[k].id == ingredientMajorId && lstHolder[k].gameObject.activeSelf)
                                        {

                                            instructionController.Init(lstHolder[k].transform.position, "click");
                                            return true;
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < machines.Length; i++)
                            {
                                if (ingredientMajorId == machines[i].foodId && machines[i].state == MachineStates.Burned)
                                {
                                    instructionController.Init(machines[i].transform.position, "click_2");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
    IEnumerator DelayFindObject()
    {
        yield return new WaitForSeconds(3);
        plates = FindObjectsOfType<ServePlateController>();
        toppings = FindObjectsOfType<ToppingIngredient>();
        machines = FindObjectsOfType<Machine>();
        lstHolder = FindObjectsOfType<IngredientHolder>();
    }
    #endregion
    public void SetIsFirstWinState(string levelState)
    {
        int firstWinStage_count = PlayerPrefs.GetInt(levelState, 0);
        firstWinStage_count++;
        PlayerPrefs.SetInt(levelState, firstWinStage_count);
    }
    public void OnHidePanel()
    {
        //var currentUi = CurrentUIView(UIController.Instance.GetCurrentUi());
        Debug.Log("Stack Lenght :" + UIController.Instance.StackLenght);
        if (UIController.Instance.CanBack)
        {
            if (UIController.Instance.StackLenght > 0)
                UIController.Instance.GetCurrentUi()?.OnHide();
            else
                pausePanel.Init();
        }
    }
    public bool IsShowAds
    {
        get { return isShowAds; }
        set { isShowAds = value; }
    }
    private void OnApplicationPause(bool pause)
    {
        if (Time.timeScale != 0 && !IsShowAds)
        {
            Time.timeScale = 0;
            pausePanel.Init();
        }
    }
}
