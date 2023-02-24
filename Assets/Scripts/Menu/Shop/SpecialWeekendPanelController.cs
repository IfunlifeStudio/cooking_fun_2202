using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialWeekendPanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] Transform packGroup;
    [SerializeField] Animator CurtainResetAnim;
    [SerializeField] GameObject saleLayer, unBoxLayer;
    [SerializeField] TextMeshProUGUI timeLeft, resetAmountTxt;
    [SerializeField] GameObject[] SalePacks1, SalePacks2, SalePacks3;
    int maxSaleOffValue = 30;
    int[] ratios = new int[3];
    List<GameObject> packList;
    double duration;
    int resetAmount;
    // Start is called before the first frame update
    void Start()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime tomorrow = now.AddDays(1).Date;
        duration = (tomorrow - now).TotalSeconds;
        StartCoroutine(CoundownTime());
        if (DataController.ConvertToUnixTime(System.DateTime.Now) - DataController.Instance.SpecialWeekendTimeStamp > 86400)
            ResetData();
        else
            GetSaleOffRatioData();
        packList = new List<GameObject>();
        animator.Play("Appear");
        resetAmount = PlayerPrefs.GetInt("reset_pack_amount", 1);
        bool hasUnbox = (PlayerPrefs.GetInt("unbox_special_weekend", 0) == 1);
        saleLayer.SetActive(hasUnbox);
        unBoxLayer.SetActive(!hasUnbox);
        bool canEnableResetBtn = PlayerPrefs.GetInt("has_bought_sw_0", 0) == 1 || PlayerPrefs.GetInt("has_bought_sw_1", 0) == 1 || PlayerPrefs.GetInt("has_bought_sw_2", 0) == 1;
        resetAmountTxt.transform.parent.gameObject.SetActive(!canEnableResetBtn);
        resetAmountTxt.text = (resetAmount * 5).ToString();
        DataController.Instance.SaveData();
        MainMenuController.isFirstOpenDailyOffer = false;
        if (hasUnbox)
            InitSalePackage();
    }
    private void GetSaleOffRatioData()
    {
        maxSaleOffValue = 0;
        for (int i = 0; i < 3; i++)
        {
            ratios[i] = PlayerPrefs.GetInt("sw_ratio_" + i, 0);
            if (ratios[i] >= maxSaleOffValue)
                maxSaleOffValue = ratios[i];
        }

    }
    public void ResetData()
    {
        DataController.Instance.SpecialWeekendTimeStamp = DataController.ConvertToUnixTime(System.DateTime.Now);
        PlayerPrefs.SetInt("unbox_special_weekend", 0);
        PlayerPrefs.SetInt("reset_pack_amount", 1);
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("sw_ratio_" + i, 0);
            PlayerPrefs.SetInt("has_bought_sw_" + i, 0);
        }
    }
    IEnumerator CoundownTime()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (true)
        {
            if (duration > 0)
            {
                duration -= 1;
                if (duration < 0) duration = 86400;
                timeLeft.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", (int)duration / 3600, (int)(duration / 60) % 60, (int)duration % 60);
            }
            yield return delay;
        }
    }
    public void OnClickUnLockPackage()
    {
        if(PlayerPrefs.GetInt("unbox_special_weekend", 0) == 0)
        {
            PlayerPrefs.SetInt("unbox_special_weekend", 1);
            for (int i = 0; i < 3; i++)
            {
                GetRandomSaleOffRatio(i);
            }
            StartCoroutine(DelayUnbox());
        }
    }
    IEnumerator DelayUnbox()
    {
        var sg = unBoxLayer.GetComponentsInChildren<SkeletonGraphic>();
        for (int i = 0; i < sg.Length; i++)
        {
            sg[i].AnimationState.SetAnimation(0, "click_open", false);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.5f);
        unBoxLayer.SetActive(false);
        saleLayer.SetActive(true);
        InitSalePackage();
    }
    public void OnClickReset()
    {
        resetAmount = PlayerPrefs.GetInt("reset_pack_amount", 1);
        if (DataController.Instance.Ruby >= resetAmount * 5)
        {
            DataController.Instance.Ruby -= resetAmount * 5;
            for (int i = 0; i < 3; i++)
            {
                GetRandomSaleOffRatio(i);
                Destroy(packList[i]);
            }
            resetAmount++;
            resetAmount = Mathf.Clamp(resetAmount, 1, 4);
            PlayerPrefs.SetInt("reset_pack_amount", resetAmount);
            resetAmountTxt.text = (resetAmount * 5).ToString();
            packList.Clear();
            InitSalePackage();
            DataController.Instance.SaveData();
        }
        else
            FindObjectOfType<ShopController>().OpenShopPanel(3);
    }
    public void InitSalePackage()
    {
        GetSaleOffRatioData();
        int userRank = DataController.Instance.Rank;
        switch (userRank)
        {
            case 0:
            case 1:
                SpawnPackage(SalePacks1);
                break;
            case 2:
            case 3:
                SpawnPackage(SalePacks2);
                break;
            case 4:
            case 5:
                SpawnPackage(SalePacks3);
                break;
        }
    }
    public void SpawnPackage(GameObject[] salePacks)
    {
        for (int i = 0; i < 3; i++)
        {
            string sw_ratio_data = "sw_ratio_" + i;
            string has_bought_sw_data = "has_bought_sw_" + i;
            var pack1 = Instantiate(salePacks[ratios[i] / 10 - 3 + (i * 4)], packGroup);
            packList.Add(pack1);
            pack1.GetComponent<SpecialWeekendItemController>().Init(i, PlayerPrefs.GetInt(has_bought_sw_data) == 1, PlayerPrefs.GetInt(sw_ratio_data) == maxSaleOffValue);
            pack1.transform.SetAsFirstSibling();
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        FindObjectOfType<DailyOfferController>().CanDisplayDailyOffer();
        animator.Play("Disappear");
        FindObjectOfType<MainMenuController>().DislayDailyReward();
        Destroy(gameObject, 0.3f);
    }
    public void GetRandomSaleOffRatio(int Index)
    {
        string sw_ratio = "sw_ratio_" + Index;
        ratios[Index] = Random.Range(0, 100);
        if (ratios[Index] <= 40) ratios[Index] = 30;
        else if (ratios[Index] <= 80) ratios[Index] = 40;
        else if (ratios[Index] <= 90) ratios[Index] = 50;
        else ratios[Index] = 60;
        PlayerPrefs.SetInt(sw_ratio, ratios[Index]);
    }
}
