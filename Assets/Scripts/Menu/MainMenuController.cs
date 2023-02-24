using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.Events;
using Facebook.Unity;

public class MainMenuController : MonoBehaviour
{
    public const string FIRSRT_RUN_TIMESTAMP_HASH = "first_run_timestamp", ZONE_INDEX = "zone_index";

    public Transform overlayCanvas, cameraCanvas, zoneCanvas;
    [Header("UI")]
    private ScrollRect resBackground;
    [SerializeField] private LevelSelectorController levelSelectorController;
    [SerializeField] private GameObject /*noAdsPanelPrefab,*/ unlockZonePanelPrefab;
    [SerializeField] private GameObject nextZonePanelPrefab;
    [SerializeField] private GameObject restaurantUnlockPrefab, restaurantCommingSoonPrefab;
    [SerializeField] private IceCreamController iceCreamTruck;
    [SerializeField] private LevelSelectHelper levelSelectHelper;
    [SerializeField] private GameObject iceCreamTruckTutPrefab;
    [SerializeField] private GameObject unlockRestaurantPrefab, dailyReward, loginPanelPrefab, claimLoginRewardPrefab;
    [SerializeField] DailyOfferController dailyOffer;
    [SerializeField] private RateUsController UIRateUsController;
    [SerializeField] private RewardPanelController UiRewardPanel;
    [HideInInspector] public DetailPanelController detailPanel;
    [SerializeField] private QuitPanelController uiQuitPanel;
    [SerializeField] private ItemMessagePanel uiMessagePanel;
    [SerializeField] private UpgradeWithRubyPanel uiUpgradeWithRubyPanel;
    [SerializeField] private SandwichPanelController sandwichPanel;
    [SerializeField] private ProfilePanelController profilePanel;
    [SerializeField] private UnlockNewRestaurantController UnlockRestaurantPanel;
    [SerializeField] private XmasPassController xmasPassController;
    private TextMeshProUGUI keyBanner;
    [Header("Top Bar")]
    [SerializeField] private Image xpProgressBar;
    public Transform coinIcon, rubyIcon, shopIcon, xmasBtn;
    [SerializeField] private TextMeshProUGUI goldText, rubyText;
    [SerializeField] private GameObject coinProp, gemProp, itemProp, sockProp;
    [SerializeField] private AudioClip coinIncreaseAudio, rewardAudio, backgroundAudio;
    [SerializeField] private RestaurantCollection[] restaurantCollections;
    private RestaurantCollection activeRestaurantCollection;
    private float goldTimeStamp, gemTimeStamp, clickTimeStamp;
    private int tmpGold, targetGold = 0, tmpGem, targetGem;
    private bool isLockUpdateData = false;
    private float instructionTime = 0, tabIndex = 0;
    private bool canInstruction, hideUI;
    private string ABtesting = "";
    private int maxUnlockedResInZone = 1;
    public static bool isFirstOpenDailyOffer = true;
    [SerializeField] private InstructionCanvasController instructionController;
    [SerializeField] private RectTransform levelSelectTabBtn;
    [SerializeField] Animator[] barAnimator;
    [Header("Noel")]
    [SerializeField] private GameObject packNoelBtn;
    [SerializeField] private GameObject xmasPassBtn;
    private IEnumerator Start()
    {
        Input.multiTouchEnabled = false;
        yield return new WaitForSeconds(0.1f);
        UIController.Instance.ClearStack();
        instructionTime = Time.time;
        if (packNoelBtn != null)
            packNoelBtn.SetActive(DataController.Instance.IsShowNoel == 1);
        if (xmasPassBtn != null)
            xmasPassBtn.SetActive(DataController.Instance.IsShowNoel == 1);
        canInstruction = true;
        ABtesting = PlayerPrefs.GetString("ABtesting_data", "");
        Time.timeScale = 1;
        goldText.text = DataController.Instance.Gold.ToString();
        rubyText.text = DataController.Instance.Ruby.ToString();
        tmpGold = DataController.Instance.Gold;
        AudioController.Instance.PlayMusic(backgroundAudio, true);
        DataController.Instance.onDataChange.AddListener(OnDataChange);
        int nextResId = 1;//Get the next unlock restaurant
        do
        {
            nextResId++;
            if (DataController.Instance.GetRestaurantById(nextResId) == null)
            {
                nextResId--;
                break;
            }
        }
        while (DataController.Instance.IsRestaurantUnlocked(nextResId));
        int zoneIndex = PlayerPrefs.GetInt(ZONE_INDEX, -1);
        if (zoneIndex == -1)
        {
            if (nextResId <= 5)
            {
                zoneIndex = 0;
                PlayerPrefs.SetInt(ZONE_INDEX, zoneIndex);
            }
            else if (nextResId <= 9)
            {
                zoneIndex = 1;
                PlayerPrefs.SetInt(ZONE_INDEX, zoneIndex);
            }
            else if (nextResId <= 13)
            {
                zoneIndex = 2;
                PlayerPrefs.SetInt(ZONE_INDEX, zoneIndex);
            }
            else
            {
                zoneIndex = 3;
                PlayerPrefs.SetInt(ZONE_INDEX, zoneIndex);
            }
        }
        if (nextResId == 10 || nextResId == 5 || nextResId == 14)
            maxUnlockedResInZone = nextResId;
        else
            maxUnlockedResInZone = nextResId - 1;
        var resButtonArray = restaurantCollections[zoneIndex].restaurantButtons;
        int resButtonArrayLenght = resButtonArray.Length;
        maxUnlockedResInZone = Mathf.Min(maxUnlockedResInZone, resButtonArray[resButtonArrayLenght - 1].resIndex);
        activeRestaurantCollection = Instantiate(restaurantCollections[zoneIndex].gameObject, zoneCanvas).GetComponent<RestaurantCollection>();
        activeRestaurantCollection.InitiateResBtns();
        resBackground = activeRestaurantCollection.resBackground;
        resBackground.onValueChanged.AddListener(OnScrollValueChange);
        if (DataController.Instance.currentChapter < 0)
        {
            if (DataController.Instance.GetRestaurantById(nextResId) != null)
            {
                resBackground.content.anchoredPosition = new Vector2(925 - (nextResId - 2) % 5 * 135, 0);
            }
            else
            {
                resBackground.content.anchoredPosition = new Vector2(925 - (nextResId - 1) % 5 * 135, 0);
            }
        }
        else
            resBackground.content.anchoredPosition = new Vector2(925 - ((DataController.Instance.currentChapter - 1) % 5) * 100, 0);
        keyBanner = activeRestaurantCollection.keyBanner;
        var _restaurant = DataController.Instance.GetRestaurantById(nextResId);
        int keyGranted = DataController.Instance.GetTotalKeyGranted();
        keyBanner.text = keyGranted + "/" + _restaurant.keyRequired;
        DataController.Instance.GetGameData().profileDatas.KeyGranted = keyGranted;
        var nextRes = activeRestaurantCollection.GetRestaurantById(nextResId);

        if (nextRes != null)
            keyBanner.transform.parent.SetParent(nextRes);//handle index out of bound when update remote config
        keyBanner.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(107, 27);
        //nextRes?.gameObject.GetComponent<RestaurantButton>().SetActiveUserPanelPosition(keyBanner.transform.parent.localPosition + new Vector3(8, 48, 0));
        if (DataController.Instance.IsRestaurantUnlocked(nextResId))
            keyBanner.transform.parent.gameObject.SetActive(false);
        else
            keyBanner.transform.parent.gameObject.SetActive(nextRes != null);
        if (DataController.Instance.GetGameData().userIapValue <= 0)
        {
            if (!PlayerPrefs.HasKey("paying_type"))
            {
                PlayerPrefs.SetString("paying_type", "f1");
                APIController.Instance.SetProperty("paying_type", "f1");
            }
            else if (keyGranted == PlayerClassifyController.Instance.playerClassifyData.keyThresholds[1])
            {
                PlayerPrefs.SetString("paying_type", "f3");
                APIController.Instance.SetProperty("paying_type", "f3");
            }
            else if (keyGranted == PlayerClassifyController.Instance.playerClassifyData.keyThresholds[0])
            {
                PlayerPrefs.SetString("paying_type", "f2");
                APIController.Instance.SetProperty("paying_type", "f2");
            }
        }
        if (!PlayerPrefs.HasKey(FIRSRT_RUN_TIMESTAMP_HASH))
            PlayerPrefs.SetFloat(FIRSRT_RUN_TIMESTAMP_HASH, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        PlayerPrefs.SetInt("hlw_user_type", 1);
        StartCoroutine(CollectLevelRewards());
        yield return new WaitForSeconds(0.2f);
        if (PlayerPrefs.GetInt(DatabaseController.LOGIN_TYPE) != 2
                    && DataController.Instance.GetGameData().hasClaimLoginReward == 0
                    && DataController.Instance.GetLevelState(1, 2) > 0)
        {
            Instantiate(claimLoginRewardPrefab, overlayCanvas).GetComponent<LoginPanelController>();
            tabIndex = 5;
        }
    }
    void OnDestroy()
    {
        DataController.Instance.onDataChange.RemoveListener(OnDataChange);
    }
    private void Update()
    {
        if (Time.time - instructionTime >= 6 && canInstruction && ABtesting == "1")
        {
            canInstruction = false;
            Vector2 playBtn = levelSelectorController.playBtn.position;
            switch (tabIndex)
            {
                case 2:
                    if (levelSelectHelper.CanUpgradeIngredients()
                        && DataController.Instance.GetIngredientLevel(detailPanel.ActiveIngredientUpgrade.currentIngredient.ingredientId) < 3)
                    {
                        instructionController.InitUI(playBtn, cameraCanvas);
                    }
                    else
                    {
                        instructionController.InitUI(levelSelectTabBtn.position, cameraCanvas);
                    }
                    break;
                case 3:
                    if (levelSelectHelper.CanUpgradeMachine()
                        && DataController.Instance.GetMachineLevel(detailPanel.ActiveMachineUpgrade.currentMachine.machineId) < 3)
                    {
                        instructionController.InitUI(playBtn, cameraCanvas);
                    }
                    else
                    {
                        instructionController.InitUI(levelSelectTabBtn.position, cameraCanvas);
                    }
                    break;
            }
        }
        float deltaTime = Time.time - goldTimeStamp;//update top bar coin progress
        if (deltaTime < 0.1f)
        {
            int currentGold = (int)Mathf.Lerp(tmpGold, targetGold, 3.34f * Mathf.Sqrt(deltaTime));
            goldText.text = currentGold.ToString();
            float curentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            coinIcon.localScale = new Vector3(curentScale, curentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
            {
                goldText.text = DataController.Instance.Gold.ToString();
                coinIcon.localScale = Vector3.one;
            }
        }
        deltaTime = Time.time - gemTimeStamp;//update top bar gem
        if (deltaTime < 0.1f)
        {
            int currentGem = (int)Mathf.Lerp(tmpGem, targetGem, 3.34f * Mathf.Sqrt(deltaTime));
            rubyText.text = currentGem.ToString();
            float curentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            rubyIcon.localScale = new Vector3(curentScale, curentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
            {
                rubyText.text = DataController.Instance.Ruby.ToString();
                rubyIcon.localScale = Vector3.one;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            instructionTime = Time.time;
            if (!canInstruction)
            {
                canInstruction = true;
                instructionController.TurnOff();
            }
            if (UIController.Instance.IsShowSandwichBtn)
            {
                UIController.Instance.IsShowSandwichBtn = false;
                sandwichPanel.OnClickSandwichBtn();
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnHidePanel();
        }
    }
    private IEnumerator CollectLevelRewards()
    {
        if (DataController.Instance.currentChapter != -1)
        {
            resBackground.content.anchoredPosition = new Vector2(925 - (DataController.Instance.currentChapter - 1) % 4 * 140, 0);
        }
        yield return new WaitForSeconds(0.75f);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        if (lastestPassedLevel != null)
        {
            if (lastestPassedLevel.chapter > 50)//dont reward event level
            {
                yield break;
            }
            AudioController.Instance.PlaySfx(rewardAudio);
            int totalGold = LevelDataController.Instance.collectedGold;
            //int _currentLevel = DataController.Instance.GetLevelState(lastestPassedLevel.chapter, lastestPassedLevel.id);//current star of this level

            iceCreamTruck.InitIceCreamTruck();
            Transform restaurant = activeRestaurantCollection.GetRestaurantById(lastestPassedLevel.chapter);
            if (restaurant != null && totalGold != 0)
            {
                IncreaseCoin(restaurant.position, totalGold);
            }
           /* if (LevelDataController.Instance.IsSockLevel)
            {
                LevelDataController.Instance.IsSockLevel = false;
                if (restaurant != null)

                    IncreaseSock(restaurant.position);
            }*/
            LevelDataController.Instance.collectedGold = 0;
            DataController.Instance.Gold += totalGold;//collect gold and xp after spawn the coin and exp animation to prevent exp progress bar bug
            DataController.Instance.SaveData();
            // tracking earn gold
            if (totalGold > 0)
            {
                APIController.Instance.LogEventEarnGold(totalGold, "win_level");
            }
            yield return new WaitForSeconds(1.5f);
            int totalStageInRes = DataController.Instance.GetTotalLevelsPerRestaurant(lastestPassedLevel.chapter) * 3;
            int totalStageGranted = DataController.Instance.GetTotalStageGranted(lastestPassedLevel.chapter);
            if (PlayerPrefs.GetInt("rateus", 0) == 0 && (lastestPassedLevel.chapter == 1 && lastestPassedLevel.id == 7)
                && DataController.Instance.GetLevelState(1, 7) == 1)
            {
                Instantiate(UIRateUsController, cameraCanvas).OnShow();
                tabIndex = 5;
            }

            else
            if ((lastestPassedLevel.chapter == 2 && lastestPassedLevel.id == 6) && DataController.Instance.GetGameData().hasClaimLoginReward == 0 && !FB.IsLoggedIn)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    Instantiate(loginPanelPrefab, overlayCanvas).GetComponent<LoginPanelController>();
                }
                LevelDataController.Instance.lastestPassedLevel = null;
                tabIndex = 5;
            }
            if ((!DataController.Instance.GetTutorialData().Contains(209) && (lastestPassedLevel.chapter == 1 && lastestPassedLevel.id == 6)))
            {
                var piggyController = FindObjectOfType<PiggyBankBtnController>();
                DataController.Instance.GetTutorialData().Add(209);
                DataController.Instance.SaveData(false);
                piggyController.ShowIntroTutorial();
            }
            else
                DisplayMenuPanel();           
        }
        else
        {
            if (DataController.Instance.currentChapter > 0)
            {
                if (LevelDataController.Instance.currentLevel.chapter > 50)
                {
                    yield break;//dont open level select panel when lose event game
                }
                if (PlayerPrefs.GetInt("CanShowUpgrade", 0) == 1)
                {
                    int panelIndex = GetDetailPanelIndex();
                    detailPanel.OpenDetailPanel(panelIndex);
                }
                else
                {
                    detailPanel.OpenDetailPanel(1);
                }

                //currentChapter = DataController.Instance.currentChapter;

                //int id = DataController.Instance.GetRecommendUpgradeId();//detect which upgrade panel should open base on recommend upgrade
                //if (DataController.Instance.GetRestaurantById(currentChapter).IsMachineId(id))
                //    panelIndex = 3;
                //if (lastLevel == DataController.Instance.Level)
                //{
                //    detailPanel.OpenDetailPanel(panelIndex);
                //}
            }
            else
            {
                ShowEarlyOffer();
            }
        }
        int nextResId = 1;//Get the next unlock restaurant
        do
        {
            nextResId++;
            if (DataController.Instance.GetRestaurantById(nextResId) == null)
            {
                nextResId--;
                break;
            }
        }
        while (DataController.Instance.IsRestaurantUnlocked(nextResId));
        var _restaurant = DataController.Instance.GetRestaurantById(nextResId);
        keyBanner.text = DataController.Instance.GetTotalKeyGranted() + "/" + _restaurant.keyRequired;
        var nextRes = activeRestaurantCollection.GetRestaurantById(nextResId);
        if (nextRes != null)
            keyBanner.transform.parent.SetParent(nextRes);
        keyBanner.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(107, 27);
        //nextRes?.gameObject.GetComponent<RestaurantButton>().SetActiveUserPanelPosition(keyBanner.transform.parent.localPosition + new Vector3(8, 48, 0));
        if (DataController.Instance.IsRestaurantUnlocked(nextResId))
            keyBanner.transform.parent.gameObject.SetActive(false);
        else
            keyBanner.transform.parent.gameObject.SetActive(nextRes != null);
    }
    public void ShowEarlyOffer()
    {
        if (dailyOffer.CanDisplayDailyOffer() && MainMenuController.isFirstOpenDailyOffer)
        {
            DisplayDailyOffer();
        }
        else
        {
            DislayDailyReward();
        }
    }
    public void DisplayMenuPanel()
    {
        StartCoroutine(_DisplayMenuPanel());
    }


    public void DisplayDailyOffer()
    {
        dailyOffer.OnClickShowDailyOfferPanel();
        tabIndex = 5;
    }

    public void DislayDailyReward()
    {
        if (DailyRewardController.CanClaimReward() && DataController.Instance.GetLevelState(1, 6) >= 1)
        {
            var dailyRewardObject = Instantiate(dailyReward, cameraCanvas);
            tabIndex = 5;
        }
    }

    public void HideUI()
    {
        if (!hideUI)
        {
            hideUI = true;
            for (int i = 0; i < barAnimator.Length; i++)
            {
                barAnimator[i].Play("Disappear");
            }
        }
    }
    public void ShowUI()
    {
        if (hideUI)
        {
            hideUI = false;
            for (int i = 0; i < barAnimator.Length; i++)
            {
                barAnimator[i].Play("Appear");
            }
        }
    }
    public void ShowProfilePanel()
    {
        HideUI();
        var profile = Instantiate(profilePanel, cameraCanvas);
        profile.Init(DataController.Instance.GetGameData().profileDatas, ShowUI);
    }
    private IEnumerator _DisplayMenuPanel()
    {
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;

        if (!DataController.Instance.GetTutorialData().Contains(201) && DataController.Instance.GetLevelState(1, 2) == 0)
        {
            UIController.Instance.CanBack = false;
        }
        if (FindObjectOfType<PiggyBankBtnController>().CanShowFullPiggyPanel())
            FindObjectOfType<PiggyBankBtnController>().ShowFullPiggyPanel();
        else if (lastestPassedLevel !=null && lastestPassedLevel.chapter == 2 && lastestPassedLevel.id == 1 && DataController.Instance.GetLevelState(2, 1) == 1 && DataController.Instance.IsShowNoel == 1)
        {
            FindObjectOfType<XmasPassController>().OnShowUnlockPoster();
        }
        else if (lastestPassedLevel != null && lastestPassedLevel.chapter > 1 && lastestPassedLevel.id == 2 && DataController.Instance.GetLevelState(lastestPassedLevel.chapter, lastestPassedLevel.id) == 1)
        {
            FindObjectOfType<ZoneOfferController>().OnClickZoneOfferBtn();
        }
        else if (!DataController.Instance.GetTutorialData().Contains(202))//instance tutorial here
        {
            DataController.Instance.GetTutorialData().Add(202);
            var RestaurantPos = activeRestaurantCollection.GetRestaurantById(maxUnlockedResInZone);
            canInstruction = false;
            if (RestaurantPos != null)
                instructionController.InitUI(RestaurantPos.position, zoneCanvas.transform.GetChild(0));
            tabIndex = 5;
        }
        else if ((lastestPassedLevel != null && lastestPassedLevel.chapter == 1 && lastestPassedLevel.id == 12) && PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0) == 0 && DataController.Instance.GetGameData().userIapValue == 0)
        {
            FindObjectOfType<StarterPackController>().OpenPackPanel();
            PlayerPrefs.SetFloat(StarterPackController.STARTER_PACK_HASH, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        }
        else if (FindObjectOfType<FlashSaleController>().CanShowFlashSale())
        {
            tabIndex = 5;
        }
        else if (lastestPassedLevel != null && lastestPassedLevel.chapter == 1 && lastestPassedLevel.id == 9)
        {
            if (!DataController.Instance.GetTutorialData().Contains(102000))
            {
                DataController.Instance.GetTutorialData().Add(102000);
                FindObjectOfType<IceCreamController>().iceCreamCar.gameObject.SetActive(true);
                resBackground.content.anchoredPosition = new Vector2(855, 0);
                GameObject go = Instantiate(iceCreamTruckTutPrefab, zoneCanvas);
                tabIndex = 5;
                UIController.Instance.CanBack = false;
                go.GetComponentInChildren<Button>().onClick.AddListener(
                    () =>
                    {
                        FindObjectOfType<IceCreamController>().OnBtnIceCreamCarClick();
                        UIController.Instance.CanBack = true;
                        Destroy(go);
                    });
            }
        }
        else
        {
            tabIndex = 1;
            detailPanel.OpenDetailPanel(1);
            yield return new WaitForSeconds(2);
        }
    }
    public void OnDataChange()
    {
        if (isLockUpdateData) return;
        if (targetGold != DataController.Instance.Gold)
        {
            targetGold = DataController.Instance.Gold;
            goldTimeStamp = Time.time;
        }
        if (targetGem != DataController.Instance.Ruby)
        {
            targetGem = DataController.Instance.Ruby;
            gemTimeStamp = Time.time;
        }
    }
    public void IncreaseCoin(Vector3 spawnPos, int totalGold)
    {
        StartCoroutine(_IncreaseCoin(spawnPos, totalGold));
    }
    public IEnumerator _IncreaseCoin(Vector3 spawnPos, int totalGold)
    {
        isLockUpdateData = true;
        targetGold = DataController.Instance.Gold;
        tmpGold = targetGold;
        int[] golds = new int[5];//devide gold to 5 pack and instance to coin
        int averangeGold = totalGold / 5;
        for (int i = 0; i < 4; i++)
            golds[i] = averangeGold;
        golds[4] = totalGold - averangeGold * 4;
        for (int i = 0; i < 5; i++)
        {
            GameObject coin = Instantiate(coinProp, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
            Vector3 target = coin.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updateCoinPos = (t) =>
                {
                    coin.transform.position = t.CurrentValue;
                };
            int rewardGold = golds[i];
            System.Action<ITween<Vector3>> completedCoinMovement = (t) =>
            {
                AudioController.Instance.PlaySfx(coinIncreaseAudio);
                targetGold += rewardGold;
                goldTimeStamp = Time.time;
                Destroy(coin);
            };
            TweenFactory.Tween("coin" + i + Time.time, coin.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updateCoinPos).ContinueWith(new Vector3Tween().Setup(target, new Vector3(coinIcon.position.x, coinIcon.position.y, spawnPos.z - 0.1f), 0.25f, TweenScaleFunctions.QuadraticEaseIn, updateCoinPos, completedCoinMovement));
            yield return new WaitForSeconds(0.06f);
        }
        isLockUpdateData = false;
    }
    public void IncreaseGem(Vector3 spawnPos, int totalGem)
    {
        StartCoroutine(_IncreaseGem(spawnPos, totalGem));
    }
    private IEnumerator _IncreaseGem(Vector3 spawnPos, int totalGem)
    {
        isLockUpdateData = true;
        tmpGem = DataController.Instance.Ruby;
        targetGem = tmpGem;
        int totalPack = Mathf.Min(10, totalGem);
        int gemPerPack = (int)totalGem / totalPack;
        int[] gems = new int[totalPack];//split xp to 3 pack
        for (int i = 0; i < totalPack - 1; i++)
            gems[i] = gemPerPack;
        gems[totalPack - 1] = totalGem - gemPerPack * (totalPack - 1);
        for (int i = 0; i < totalPack; i++)
        {
            GameObject gem = Instantiate(gemProp, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
            Vector3 target = gem.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updateGemPos = (t) =>
            {
                gem.transform.position = t.CurrentValue;
            };
            int rewardGem = gems[i];
            System.Action<ITween<Vector3>> completedGemMovement = (t) =>
            {
                AudioController.Instance.PlaySfx(coinIncreaseAudio);
                targetGem += rewardGem;
                gemTimeStamp = Time.time;
                Destroy(gem);
            };
            TweenFactory.Tween("gem" + i + Time.time, gem.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updateGemPos).ContinueWith(new Vector3Tween().Setup(target, rubyIcon.position, 0.45f, TweenScaleFunctions.QuadraticEaseIn, updateGemPos, completedGemMovement));
            yield return null;
        }
        isLockUpdateData = false;
    }
    public void IncreaseItem(Vector3 spawnPos)
    {
        StartCoroutine(_IncreaseItem(spawnPos));
    }
    public void IncreaseEnergy(Vector3 spawnPos, int amount)
    {
        StartCoroutine(_IncreaseEnergy(spawnPos, amount));
    }
    public IEnumerator _IncreaseEnergy(Vector3 spawnPos, int amount)
    {
        isLockUpdateData = true;
        for (int i = 0; i < amount; i++)
        {
            GameObject gem = Instantiate(gemProp, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
            Vector3 target = gem.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updateGemPos = (t) =>
            {
                gem.transform.position = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> completedGemMovement = (t) =>
            {
                gemTimeStamp = Time.time;
                Destroy(gem);
            };
            TweenFactory.Tween("gem" + i + Time.time, gem.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updateGemPos).ContinueWith(new Vector3Tween().Setup(target, rubyIcon.position, 0.45f, TweenScaleFunctions.QuadraticEaseIn, updateGemPos, completedGemMovement));
            yield return null;
            DataController.Instance.IncreaseOneEnergy();
        }
        isLockUpdateData = false;
    }
    /*public void IncreaseSock(Vector3 spawnPos)
    {
        StartCoroutine(_IncreaseSock(spawnPos));
    }
    public IEnumerator _IncreaseSock(Vector3 spawnPos)
    {
        isLockUpdateData = true;
        for (int i = 0; i < 5; i++)
        {
            GameObject sock = Instantiate(sockProp, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
            Vector3 target = sock.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updateGemPos = (t) =>
            {
                sock.transform.position = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> completedGemMovement = (t) =>
            {
                Destroy(sock);
            };
            TweenFactory.Tween("sock" + i + Time.time, sock.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updateGemPos).ContinueWith(new Vector3Tween().Setup(target, xmasBtn.position, 0.45f, TweenScaleFunctions.QuadraticEaseIn, updateGemPos, completedGemMovement));
            yield return null;
        }
        isLockUpdateData = false;
    }*/
    private IEnumerator _IncreaseItem(Vector3 spawnPos)
    {
        isLockUpdateData = true;
        for (int i = 0; i < 5; i++)
        {
            GameObject item = Instantiate(itemProp, spawnPos + new Vector3(0, 0, -0.1f), Quaternion.identity);
            Vector3 target = item.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//generate a random pos
            System.Action<ITween<Vector3>> updateGemPos = (t) =>
            {
                item.transform.position = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> completedGemMovement = (t) =>
            {
                AudioController.Instance.PlaySfx(coinIncreaseAudio);
                Destroy(item);
            };
            TweenFactory.Tween("item" + i + Time.time, item.transform.position, target, 0.25f, TweenScaleFunctions.QuinticEaseOut, updateGemPos).ContinueWith(new Vector3Tween().Setup(target, shopIcon.position, 0.45f, TweenScaleFunctions.QuadraticEaseIn, updateGemPos, completedGemMovement));
            yield return null;
        }
        isLockUpdateData = false;
    }
    public void OnSelectRestaurant(int chapter)
    {
        if (Time.time - clickTimeStamp < 0.5f) return;
        else
            clickTimeStamp = Time.time;
        if (DataController.Instance.IsRestaurantUnlocked(chapter))
        {
            if (DataController.Instance.GetGameData().restaunrantUnlockReward == 0)
            {
                DataController.Instance.GetGameData().restaunrantUnlockReward = DataController.Instance.GetHighestRestaurant();
            }
            if (DataController.Instance.GetGameData().restaunrantUnlockReward < chapter && chapter <= DataController.Instance.HighestRestaurant && chapter != 1)
            {
                DataController.Instance.GetGameData().restaunrantUnlockReward = DataController.Instance.HighestRestaurant;
                var unl_Res_Panel = Instantiate(UnlockRestaurantPanel, cameraCanvas);
                unl_Res_Panel.Init(chapter - 1);
            }
            else
            {
                DataController.Instance.currentChapter = chapter;
                tabIndex = 1;
                detailPanel.OpenDetailPanel(1);
            }
        }
        else
            StartCoroutine(DelayOpenResUnlockPanel(chapter));
    }

    private IEnumerator DelayOpenResUnlockPanel(int chapter)
    {
        yield return new WaitForSeconds(0.25f);
        if (chapter >= 16)
        {
            var uiRestaurantUnlockPanel = Instantiate(restaurantCommingSoonPrefab, overlayCanvas).GetComponent<RestaurantUnlockPanel>();
            uiRestaurantUnlockPanel.Init(18);
        }
        else if (DataController.Instance.GetRestaurantById(chapter) != null)
        {
            var uiRestaurantUnlockPanel = Instantiate(restaurantUnlockPrefab, overlayCanvas).GetComponent<RestaurantUnlockPanel>();
            uiRestaurantUnlockPanel.Init(chapter);
        }
        else
        {
            var uiRestaurantUnlockPanel = Instantiate(restaurantCommingSoonPrefab, overlayCanvas).GetComponent<RestaurantUnlockPanel>();
            uiRestaurantUnlockPanel.Init(18);
        }
    }
    public void OnClickZoneBtn()
    {
        SceneController.Instance.LoadScene("Zones");
    }
    public void OnClickSwitchZone()
    {
        int zoneIndex = PlayerPrefs.GetInt(ZONE_INDEX, -1);
        int totalKey = DataController.Instance.GetTotalKeyGranted();
        tabIndex = 5;
        if (totalKey < 125)
        {
            Instantiate(unlockZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
        }
        else if (totalKey < 265)
        {
            if (zoneIndex == 0)
            {
                ZoneUnlockPanelController nextZone = Instantiate(nextZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
                nextZone.Init(1);
            }
            else
            {
                ZoneUnlockPanelController nextZone = Instantiate(unlockZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
            }
        }
        else if (totalKey < 435)
        {
            if (zoneIndex <= 1)
            {
                ZoneUnlockPanelController nextZone1 = Instantiate(nextZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
                nextZone1.Init(2);
            }
            else
            {
                ZoneUnlockPanelController nextZone1 = Instantiate(unlockZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
            }
        }
        else if (totalKey >= 435)
        {
            //Instantiate(nextZonePanelPrefab, overlayCanvas);
            ZoneUnlockPanelController nextZone = Instantiate(nextZonePanelPrefab, overlayCanvas).GetComponent<ZoneUnlockPanelController>();
            nextZone.Init(3);
        }
    }
    public void OnChooseZone(int zoneIndex)
    {
        DataController.Instance.currentChapter = 0;
        LevelDataController.Instance.lastestPassedLevel = null;
        PlayerPrefs.SetInt(ZONE_INDEX, zoneIndex);
        SceneController.Instance.LoadScene("MainMenu");
    }


    public void UnlockResAndCollectReward()
    {
        StartCoroutine(DelayUnlockAndCollectReward());
    }
    private IEnumerator DelayUnlockAndCollectReward()
    {
        ShowUI();
        yield return new WaitForSecondsRealtime(0.2f);
        DataController.Instance.SetRestaurantRewarded(DataController.Instance.currentChapter);
        var nextRestaurant = activeRestaurantCollection.GetRestaurantById(DataController.Instance.currentChapter + 1);
        DataController.Instance.HighestRestaurant = DataController.Instance.currentChapter + 1;
        yield return new WaitForSecondsRealtime(0.2f);
        activeRestaurantCollection.InitiateResBtns();
        yield return new WaitForSecondsRealtime(0.2f);
        if (DataController.Instance.currentChapter == 4 || DataController.Instance.currentChapter == 8)
        {
            //if (DataController.Instance.currentChapter == 4)
            resBackground.content.anchoredPosition = new Vector2(390, 0);
            // else
            //    resBackground.content.anchoredPosition = new Vector2(1020, 0);
            UiRewardPanel.Init(0, 10, 30, new int[0], new int[0], true, "default",
                        () => { FindObjectOfType<RestaurantCollection>().ShowSwitchZoneTutorial(); });
            tabIndex = 5;
        }
        else
        {
            resBackground.content.anchoredPosition = new Vector2(925 - (DataController.Instance.currentChapter % 5) * 100, 0);
            GameObject go = Instantiate(unlockRestaurantPrefab, nextRestaurant.position, Quaternion.identity, nextRestaurant);
            Destroy(go, 2);
            yield return new WaitForSecondsRealtime(1f);
            if (PlayerPrefs.GetInt("rateus", 0) == 0)
            {
                UiRewardPanel.Init(0, 40, 30, new int[0], new int[0], true, "default",
            () => { Instantiate(UIRateUsController, cameraCanvas).OnShow(); });
                tabIndex = 5;
            }
        }
    }
    public void OpenLastestRestaurant()
    {
        int chapterIndex = 1;
        while (DataController.Instance.IsRestaurantUnlocked(chapterIndex + 1) && chapterIndex <= DataController.MAX_RESTAURANT)
            chapterIndex++;
        if (chapterIndex > DataController.MAX_RESTAURANT) chapterIndex = DataController.MAX_RESTAURANT;
        FindObjectOfType<MainMenuController>().OnSelectRestaurant(chapterIndex);//open the level which is not collected all keys
    }
    public void OnOpenMaxResInZone()
    {
        OnSelectRestaurant(DataController.Instance.GetHighestRestaurant());
    }
    public void OnScrollValueChange(Vector2 value)
    {
        if (Screen.width * 1f / Screen.height < 1.7f)
        {
            if (Screen.width * 1f / Screen.height < 1.5f)
            {
                if (value.x < -0.08f)
                    resBackground.content.anchoredPosition = new Vector2(1070, 0);
                else if (value.x > 1.08f)
                    resBackground.content.anchoredPosition = new Vector2(150, 0);
            }
            else
            {
                if (value.x < -0.08f)
                    resBackground.content.anchoredPosition = new Vector2(1070, 0);
                else if (value.x > 1.08f)
                    resBackground.content.anchoredPosition = new Vector2(300, 0);
            }
        }
        else
           if (Screen.width * 1f / Screen.height < 1.95f)
        {
            if (value.x < -0.08f)
                resBackground.content.anchoredPosition = new Vector2(1070, 0);
            else if (value.x > 1.08f)
                resBackground.content.anchoredPosition = new Vector2(330, 0);
        }
        else if (Screen.width * 1f / Screen.height >= 1.95f)
        {
            if (value.x < -0.08f)
                resBackground.content.anchoredPosition = new Vector2(1070, 0);
            else if (value.x > 1.08f)
                resBackground.content.anchoredPosition = new Vector2(460, 0);
        }
    }
    public void setIndexTab(int index)
    {
        tabIndex = index;
    }
    //public void ShowAdsPanel()
    //{
    //    Instantiate(noAdsPanelPrefab, overlayCanvas);
    //    tabIndex = 5;
    //    isRewarding = false;
    //}
    int GetDetailPanelIndex()
    {
        int totalMoney = DataController.Instance.Gold + DataController.Instance.Ruby * 20;
        List<int> ingredientIds = new List<int>();
        List<int> machineIds = new List<int>();
        int chapterId = DataController.Instance.currentChapter;
        var ingredientDatas = DataController.Instance.GetRestaurantById(chapterId).ingredients;
        var machineDatas = DataController.Instance.GetRestaurantById(chapterId).machines;
        foreach (var machineData in machineDatas)
        {
            if (DataController.Instance.GetMachineUpgradeCost(machineData.id) < totalMoney && DataController.Instance.IsMachineUnlocked(machineData.id) && DataController.Instance.GetMachineLevel(machineData.id) < 3)
                return 3;
        }
        foreach (var ingredientData in ingredientDatas)
        {
            if (DataController.Instance.GetIngredientUpgradeCost(ingredientData.id) < totalMoney && DataController.Instance.IsIngredientUnlocked(ingredientData.id) && DataController.Instance.GetIngredientLevel(ingredientData.id) < 3)
                return 2;
        }
        return 1;
    }
    #region Handle UI  
    public void OnHidePanel()
    {
        //var currentUi = CurrentUIView(UIController.Instance.GetCurrentUi());
        Debug.Log("Stack Lenght :" + UIController.Instance.StackLenght);
        if (UIController.Instance.CanBack)
        {
            if (UIController.Instance.StackLenght > 0)
                UIController.Instance.GetCurrentUi()?.OnHide();
            else
                Instantiate(uiQuitPanel, overlayCanvas);
        }
    }
    public void ShowMessagePanel(string message)
    {
        Instantiate(uiMessagePanel, overlayCanvas).Show(message);
    }
    public void ShowUpgradeWithRubyPanel(int upgradeGold, UnityAction callback, string _upgradeDes)
    {
        Instantiate(uiUpgradeWithRubyPanel, overlayCanvas).Init(upgradeGold, callback, _upgradeDes);
    }
    #endregion
}
