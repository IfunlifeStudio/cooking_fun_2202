using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using Random = UnityEngine.Random;
using DigitalRuby.Tween;
public class IceCreamController : UIView
{
    private const int ICESCREAM_REPLENISH_TIME = 10800;
    public int[] ICECREAM_AMOUNT_MILESTONE = { 5, 8, 11, 14, 17, 20, 23 };
    public int[] ICECREAM_STORAGE_MILESTONE = { 20, 30, 40, 50, 60, 70, 80 };
    public int[] ICECREAM_WAITTIME_MILESTONE = { 14400, 13500, 12600, 11700, 10800, 9900, 9000 };
    public int[] ICECREAM_UPGRADECOST_MILESTONE = { 150, 200, 250, 300, 350, 400, 450 };
    [SerializeField] private float timeStamp;
    //   [SerializeField] private Animator Panel0, Panel1;//
    [SerializeField] private GameObject IceCreamUpgradePanel;
    [SerializeField] private Animator animator;
    public GameObject BtnCountDown, BtnClaim, btnUpgrade, btnMax;
    [SerializeField] private TextMeshProUGUI quatityIceCream, upgradeCost, timeUpgrade, crr_claimIce, crr_storgeIce, crr_timeupgrade, next_claimIce, next_storgeIce, next_timeUpgrade, claimTxt;
    [SerializeField] private GameObject[] progressBarElement;
    [SerializeField] private GameObject[] progressLevel;
    public GameObject iceCreamIcon;
    public SkeletonGraphic machineSkeleton;
    [SerializeField] IceCreamCarController carPanelPrefab;
    [HideInInspector] public SkeletonGraphic iceCreamCar, iceCreamHuman;
    private GameObject notification;
    [Header("Tutorials")]
    [SerializeField] private GameObject iceCreamClaimPrefab;
    [SerializeField] private GameObject iceCreamUpgradePrefab;
    [SerializeField]
    private AudioClip upgradeSfx;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.25f);
        iceCreamCar = FindObjectOfType<RestaurantCollection>().iceCreamCar;
        iceCreamHuman = FindObjectOfType<RestaurantCollection>().icecreamHuman;
        notification = FindObjectOfType<RestaurantCollection>().iceCreamNoti;
        //DataController.Instance.IceCreamDuration -= CalculateTimePassUnlimitedIceCream();
        BtnClaim.SetActive(DataController.Instance.IceCreamDuration <= 0);
        InitIceCreamTruck();
    }
    public void InitIceCreamTruck()
    {
        Onmaxlevel();
        bool canDisplayIceCreamTruck = DataController.Instance.IsItemUnlocked((int)ItemType.IceCream);
        iceCreamCar.gameObject.SetActive(canDisplayIceCreamTruck);
        notification.SetActive(canDisplayIceCreamTruck && DataController.Instance.IceCreamDuration <= 0);
        if (canDisplayIceCreamTruck)
        {
            SetTextIceCream();
            SetProgressMileStone();
            iceCreamCar.Skeleton.SetSkin((DataController.Instance.IceCreamTimeLevel + 1).ToString());
        }
    }
    public void Onmaxlevel()
    {
        int level = DataController.Instance.IceCreamTimeLevel;
        btnUpgrade.SetActive(level < 6);
        btnMax.SetActive(level >= 6);
    }
    private IEnumerator RandoomIceCreamHumanAnimation()
    {
        yield return new WaitForSecondsRealtime(Random.Range(4f, 7f));
        iceCreamHuman.AnimationState.SetAnimation(0, Random.Range(0, 2) == 0 ? "idle3" : "idle2", true);
        yield return new WaitForSecondsRealtime(Random.Range(4f, 7f));
        iceCreamHuman.AnimationState.SetAnimation(0, "idle", true);
        StartCoroutine(RandoomIceCreamHumanAnimation());
    }
    private IEnumerator ReplenishIceCream()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (true)
        {
            if (DataController.Instance.IceCreamDuration > 0)
            {
                DataController.Instance.IceCreamDuration -= CalculateTimePassUnlimitedIceCream();
                if (DataController.Instance.IceCreamDuration < 0) DataController.Instance.IceCreamDuration = 0;
                timeUpgrade.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", DataController.Instance.IceCreamDuration / 3600, (DataController.Instance.IceCreamDuration / 60) % 60, DataController.Instance.IceCreamDuration % 60);
                BtnCountDown.SetActive(true);
                BtnClaim.SetActive(false);
                DataController.Instance.IceCreamTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
                notification.SetActive(false);
                iceCreamIcon.SetActive(false);
                machineSkeleton.AnimationState.SetAnimation(0, "idle", true);
            }
            else
            {
                notification.SetActive(true);
                DataController.Instance.IceCreamDuration = 0;
                BtnCountDown.SetActive(false);
                iceCreamIcon.SetActive(true);
                int currentNumber = DataController.Instance.GetItemQuantity((int)ItemType.IceCream);
                if (currentNumber < ICECREAM_STORAGE_MILESTONE[DataController.Instance.IceCreamTimeLevel])
                {
                    BtnClaim.SetActive(true);
                }
                else
                {
                    timeUpgrade.text = Lean.Localization.LeanLocalization.GetTranslationText("Full");
                    BtnClaim.SetActive(false);
                }
                machineSkeleton.AnimationState.SetAnimation(0, "full", true);
            }
            yield return delay;
        }
    }
    public void OnBtnUpgradeClick()
    {
        if (DataController.Instance.Ruby >= ICECREAM_UPGRADECOST_MILESTONE[DataController.Instance.IceCreamTimeLevel])
        {
            int cost = ICECREAM_UPGRADECOST_MILESTONE[DataController.Instance.IceCreamTimeLevel];
            DataController.Instance.Ruby -= cost;
            APIController.Instance.LogEventSpentRuby(cost, "upgrade_icecream");
            DataController.Instance.IceCreamTimeLevel++;
            DataController.Instance.IceCreamDuration = ICECREAM_WAITTIME_MILESTONE[DataController.Instance.IceCreamTimeLevel];
            SetTextIceCream();
            AudioController.Instance.PlaySfx(upgradeSfx);
            SetProgressMileStone();
            StopAllCoroutines();
            StartCoroutine((ReplenishIceCream()));
            InitUpgradePanel();
            DataController.Instance.SaveData();
        }
        else
            FindObjectOfType<ShopController>().OnClickRubyPanel();
    }
    private void InitUpgradePanel()
    {
        Onmaxlevel();
        var go = Instantiate(carPanelPrefab, IceCreamUpgradePanel.transform);
        int level = DataController.Instance.IceCreamTimeLevel;
        go.Init(level);

    }
    public void SetProgressMileStone()
    {
        int level = DataController.Instance.IceCreamTimeLevel;
        for (int i = 0; i < ICECREAM_AMOUNT_MILESTONE.Length; i++)
        {
            progressBarElement[i].SetActive(i <= level);
            progressLevel[i].SetActive(i == level);
        }
    }
    public void SetTextIceCream()
    {
        int level = DataController.Instance.IceCreamTimeLevel;
        if (level < 6)
            upgradeCost.text = ICECREAM_UPGRADECOST_MILESTONE[level + 1].ToString();
        else
            upgradeCost.text = ICECREAM_UPGRADECOST_MILESTONE[6].ToString();
        quatityIceCream.text = DataController.Instance.GetItemQuantity((int)ItemType.IceCream) + "/" + ICECREAM_STORAGE_MILESTONE[DataController.Instance.IceCreamTimeLevel].ToString();
        if (ICECREAM_AMOUNT_MILESTONE[level] < 23)
        {
            String crr_time = String.Format("{0:D2}h{1:D2}'", ICECREAM_WAITTIME_MILESTONE[level] / 3600, (ICECREAM_WAITTIME_MILESTONE[level] / 60) % 60);
            String next_time = String.Format("{0:D2}h{1:D2}'", ICECREAM_WAITTIME_MILESTONE[level + 1] / 3600, (ICECREAM_WAITTIME_MILESTONE[level + 1] / 60) % 60);
            crr_claimIce.text = ICECREAM_AMOUNT_MILESTONE[level].ToString();
            crr_storgeIce.text = ICECREAM_STORAGE_MILESTONE[level].ToString();
            crr_timeupgrade.text = crr_time;
            next_claimIce.text = ICECREAM_AMOUNT_MILESTONE[level + 1].ToString();
            next_storgeIce.text = ICECREAM_STORAGE_MILESTONE[level + 1].ToString();
            next_timeUpgrade.text = next_time;
        }
        else
        {
            crr_claimIce.text = "max";
            crr_storgeIce.text = "max";
            crr_timeupgrade.text = "max";
            next_claimIce.text = "max";
            next_storgeIce.text = "max";
            next_timeUpgrade.text = "max";
        }

        //txtIceCream1.text = DataController.Instance.GetItemQuantity((int)ItemType.IceCream) + "/15";
        //txtIceCream2.text = DataController.Instance.GetItemQuantity((int)ItemType.IceCream) + "/15";
        //txtIceCream3.text = DataController.Instance.GetItemQuantity((int)ItemType.IceCream) + "/15";
    }
    private int CalculateTimePassUnlimitedIceCream()
    {
        double deltaTime = DataController.ConvertToUnixTime(DateTime.UtcNow) - DataController.Instance.IceCreamTimeStamp;
        return Mathf.Max(0, (int)deltaTime);
    }
    public void OnBtnClaimClick()
    {
        int currentNumber = DataController.Instance.GetItemQuantity((int)ItemType.IceCream);
        if (currentNumber + ICECREAM_AMOUNT_MILESTONE[DataController.Instance.IceCreamTimeLevel] > ICECREAM_STORAGE_MILESTONE[DataController.Instance.IceCreamTimeLevel])
            DataController.Instance.AddItem((int)ItemType.IceCream, (ICECREAM_STORAGE_MILESTONE[DataController.Instance.IceCreamTimeLevel] - currentNumber));
        else
            DataController.Instance.AddItem((int)ItemType.IceCream, ICECREAM_AMOUNT_MILESTONE[DataController.Instance.IceCreamTimeLevel]);
        if (DataController.Instance.IceCreamDuration > ICECREAM_WAITTIME_MILESTONE[DataController.Instance.IceCreamTimeLevel] || DataController.Instance.IceCreamDuration <= 0)
        {
            DataController.Instance.IceCreamDuration = ICECREAM_WAITTIME_MILESTONE[DataController.Instance.IceCreamTimeLevel];
            DataController.Instance.IceCreamTimeStamp = DataController.ConvertToUnixTime(DateTime.UtcNow);
        }
        timeUpgrade.text = ToolHelper.GetTextTime(DataController.Instance.IceCreamDuration);
        BtnCountDown.SetActive(true);
        BtnClaim.SetActive(false);
        SetTextIceCream();
        StopAllCoroutines();
        StartCoroutine(ReplenishIceCream());
        if (PlayerPrefs.GetInt("has_display_icecream_upgrade_tut", 0) == 1)//handle old data
        {
            if (!DataController.Instance.GetTutorialData().Contains(102002))
                DataController.Instance.GetTutorialData().Add(102002);
        }
        if (!DataController.Instance.GetTutorialData().Contains(102002))
        {
            DataController.Instance.GetTutorialData().Add(102002);
            GameObject go = Instantiate(iceCreamUpgradePrefab, IceCreamUpgradePanel.transform);
            go.GetComponentInChildren<Button>().onClick.AddListener(
                () =>
                {
                    Destroy(go);
                });
        }
        DataController.Instance.SaveData(false);
    }
    public void OnBtnIceCreamCarClick()
    {
        OnSHowPanel();

    }
    public void OnSHowPanel()
    {
        if (DataController.Instance.IsItemUnlocked((int)ItemType.IceCream))
        {
            IceCreamUpgradePanel.gameObject.SetActive(true);
            animator.Play("Appear");
            UIController.Instance.PushUitoStack(this);
            if (IceCreamUpgradePanel.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(ReplenishIceCream());
                StartCoroutine(RandoomIceCreamHumanAnimation());
            }
            SetTextIceCream();
            StartCoroutine(DelayShowTutorial());
            claimTxt.text = Lean.Localization.LeanLocalization.GetTranslationText("common_claim", "Claim") + " " + ICECREAM_AMOUNT_MILESTONE[DataController.Instance.IceCreamTimeLevel];
            if (DataController.Instance.IceCreamDuration > 0)
            {
                machineSkeleton.AnimationState.SetAnimation(0, "idle", true);
            }
            else
                machineSkeleton.AnimationState.SetAnimation(0, "full", true);
        }
        else
        {
            FindObjectOfType<MainMenuController>().ShowMessagePanel(DataController.Instance.GetItemUnlockMessage((int)ItemType.IceCream));
        }
    }
    private IEnumerator DelayShowTutorial()
    {
        yield return new WaitForSeconds(0.25f);
        if (!DataController.Instance.GetTutorialData().Contains(102001))
        {
            DataController.Instance.GetTutorialData().Add(102001);
            GameObject go = Instantiate(iceCreamClaimPrefab, IceCreamUpgradePanel.transform);
            go.GetComponentInChildren<Button>().onClick.AddListener(
                () =>
                {
                    OnBtnClaimClick();
                    Destroy(go);
                });
        }

    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        StartCoroutine(IEHidePanel());
    }
    IEnumerator IEHidePanel()
    {
        UIController.Instance.PopUiOutStack();
        yield return new WaitForSeconds(0.3f);
        IceCreamUpgradePanel.gameObject.SetActive(false);
        StopAllCoroutines();
    }
}
