using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PacksLogicController : MonoBehaviour
{
    [SerializeField] private Button[] packIcons;
    [SerializeField] private GameObject[] originPacks, salePacksLevel1, salePacksLevel2;
    [SerializeField] private string[] originPacksId, saleLevel1PacksId, saleLevel2PacksId;
    [SerializeField] private Transform cameraCanvas;
    [SerializeField] private GameObject saleBtn;
    public Transform shopBtn;
    private string activePack;
    private int activePackIndex;
    private Animator activeAnim;
    private TextMeshProUGUI countDownText = null;
    private float packLifeTime, packTimeStamp, timeStamp = 0;
    private void Start()
    {
        var endDay = new DateTime(2021, 1, 5);
        var today = DateTime.Today;
        int leftTime = DateTime.Compare(today, endDay);
        if (leftTime > 0)
        {
            packIcons[0].gameObject.SetActive(false);
            packIcons[1].gameObject.SetActive(false);
            int packIndex = (PlayerClassifyController.Instance.GetPlayerClassifyLevel() - 1) * 2;
            if (packIndex >= 0)//enable correspond pack icon and start the count down
            {
                StartCoroutine(RandomPackAnimation());
                float currentTimeStamp = (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow);
                packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + originPacksId[packIndex], 0);
                packLifeTime = IAPPackHelper.GetPackLifeTime(originPacksId[packIndex]);
                if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                {
                    PlayerPrefs.SetFloat("time_stamp_" + originPacksId[packIndex], currentTimeStamp);
                    packTimeStamp = currentTimeStamp;
                }
                if (currentTimeStamp - packTimeStamp < packLifeTime)//origin pack 1
                {
                    activePack = originPacksId[packIndex];
                    float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                    packIcons[0].gameObject.SetActive(true);
                    countDownText = packIcons[0].GetComponentInChildren<TextMeshProUGUI>();
                    activePackIndex = 0;
                    return;
                }

                if (PlayerPrefs.GetInt(originPacksId[packIndex], 0) == 0
                && PlayerPrefs.GetInt(saleLevel1PacksId[packIndex], 0) == 0
                && PlayerPrefs.GetInt(saleLevel2PacksId[packIndex], 0) == 0)//user haven't bought any packs
                {
                    packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + saleLevel1PacksId[packIndex], 0);
                    if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                    {
                        PlayerPrefs.SetFloat("time_stamp_" + saleLevel1PacksId[packIndex], currentTimeStamp);
                        packTimeStamp = currentTimeStamp;
                    }
                    packLifeTime = IAPPackHelper.GetPackLifeTime(saleLevel1PacksId[packIndex]);
                    if (currentTimeStamp - packTimeStamp < packLifeTime)//sale pack 1.1
                    {
                        activePack = saleLevel1PacksId[packIndex];
                        float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                        packIcons[1].gameObject.SetActive(true);
                        countDownText = packIcons[1].GetComponentInChildren<TextMeshProUGUI>();
                        activePackIndex = 1;
                        return;
                    }

                    packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + saleLevel2PacksId[packIndex], 0);
                    if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                    {
                        PlayerPrefs.SetFloat("time_stamp_" + saleLevel2PacksId[packIndex], currentTimeStamp);
                        packTimeStamp = currentTimeStamp;
                    }
                    packLifeTime = IAPPackHelper.GetPackLifeTime(saleLevel2PacksId[packIndex]);
                    if (currentTimeStamp - packTimeStamp < packLifeTime)//sale pack 1.2
                    {
                        activePack = saleLevel2PacksId[packIndex];
                        float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                        packIcons[1].gameObject.SetActive(true);
                        countDownText = packIcons[1].GetComponentInChildren<TextMeshProUGUI>();
                        activePackIndex = 2;
                        return;
                    }
                }

                packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + originPacksId[packIndex + 1], 0);
                if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                {
                    PlayerPrefs.SetFloat("time_stamp_" + originPacksId[packIndex + 1], currentTimeStamp);
                    packTimeStamp = currentTimeStamp;
                }
                packLifeTime = IAPPackHelper.GetPackLifeTime(originPacksId[packIndex + 1]);
                if (currentTimeStamp - packTimeStamp < packLifeTime)//origin pack 2
                {
                    activePack = originPacksId[packIndex + 1];
                    float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                    packIcons[0].gameObject.SetActive(true);
                    countDownText = packIcons[0].GetComponentInChildren<TextMeshProUGUI>();
                    activePackIndex = 3;
                    return;
                }

                if (PlayerPrefs.GetInt(originPacksId[packIndex + 1], 0) == 0
                && PlayerPrefs.GetInt(saleLevel1PacksId[packIndex + 1], 0) == 0
                && PlayerPrefs.GetInt(saleLevel2PacksId[packIndex + 1], 0) == 0)//user haven't bought any packs
                {
                    packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + saleLevel1PacksId[packIndex + 1], 0);
                    if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                    {
                        PlayerPrefs.SetFloat("time_stamp_" + saleLevel1PacksId[packIndex + 1], currentTimeStamp);
                        packTimeStamp = currentTimeStamp;
                    }
                    packLifeTime = IAPPackHelper.GetPackLifeTime(saleLevel1PacksId[packIndex + 1]);
                    if (currentTimeStamp - packTimeStamp < packLifeTime)//sale pack 2.1
                    {
                        activePack = saleLevel1PacksId[packIndex + 1];
                        float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                        packIcons[1].gameObject.SetActive(true);
                        countDownText = packIcons[1].GetComponentInChildren<TextMeshProUGUI>();
                        activePackIndex = 4;
                        return;
                    }

                    packTimeStamp = PlayerPrefs.GetFloat("time_stamp_" + saleLevel2PacksId[packIndex + 1], 0);
                    if (packTimeStamp == 0 || currentTimeStamp - packTimeStamp > GetPackCycleTime())
                    {
                        PlayerPrefs.SetFloat("time_stamp_" + saleLevel2PacksId[packIndex + 1], currentTimeStamp);
                        packTimeStamp = currentTimeStamp;
                    }
                    packLifeTime = IAPPackHelper.GetPackLifeTime(saleLevel2PacksId[packIndex + 1]);
                    if (currentTimeStamp - packTimeStamp < packLifeTime)//sale pack 2.2
                    {
                        activePack = saleLevel2PacksId[packIndex + 1];
                        float deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                        packIcons[1].gameObject.SetActive(true);
                        countDownText = packIcons[1].GetComponentInChildren<TextMeshProUGUI>();
                        activePackIndex = 5;
                        return;
                    }
                }
                float _deltaTime = (int)(packLifeTime + packTimeStamp - currentTimeStamp);
                countDownText.gameObject.SetActive(_deltaTime > 0);

            }
        }
        else
        {
            saleBtn.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (Time.time - timeStamp > 1 && countDownText != null)
        {
            if (countDownText.gameObject.activeInHierarchy)
            {
                timeStamp = Time.time;
                float _deltaTime = (float)(packTimeStamp + packLifeTime - DataController.ConvertToUnixTime(System.DateTime.UtcNow));
                int hours = DataController.GetHours(_deltaTime);
                if (hours > 0)
                    countDownText.text = DataController.GetHours(_deltaTime) + "h " + DataController.GetMinutes(_deltaTime) + "m " + DataController.GetSeconds(_deltaTime) + "s";
                else
                    countDownText.text = DataController.GetMinutes(_deltaTime) + "m " + DataController.GetSeconds(_deltaTime) + "s";
            }       
        }
    }
    private IEnumerator RandomPackAnimation()
    {
        yield return new WaitForSeconds(1);
        if (packIcons[1].gameObject.activeInHierarchy)
            activeAnim = packIcons[1].GetComponentInParent<Animator>();
        else
            activeAnim = packIcons[0].GetComponentInParent<Animator>();
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 12f));
            activeAnim.Play("Effect");
        }
    }
    private GameObject GetActiveSalePrefab()
    {
        int packOffset = activePackIndex / 3;
        int packIndex = (PlayerClassifyController.Instance.GetPlayerClassifyLevel() - 1) * 2;
        GameObject activePackPrefab = originPacks[0];
        switch (activePackIndex % 3)
        {
            case 0:
                activePackPrefab = originPacks[packIndex + packOffset];
                break;
            case 1:
                activePackPrefab = salePacksLevel1[packIndex + packOffset];
                break;
            case 2:
                activePackPrefab = salePacksLevel2[packIndex + packOffset];
                break;
        }
        return activePackPrefab;
    }
    public bool CanOpenPackPanel()
    {
        bool canOpenPack = FindObjectOfType<StarterPackController>().CanDisplayPack() && DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_OPEN_TIMESTAMP, 0) > 6 * 3600;
        canOpenPack &= (CheckWinLoseOpenCondition()
                || DataController.Instance.Energy < 1
                || DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_OPEN_TIMESTAMP, 0) > 12 * 3600);
        canOpenPack &= (PlayerClassifyController.Instance.GetPlayerClassifyLevel() > 0
        && DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat("pack_timestamp", 0) > 6 * 3600)
        && (CheckWinLoseOpenCondition()
                || DataController.Instance.Energy < 1
                || DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat("pack_timestamp", 0) > 12 * 3600);
        return canOpenPack;
    }
    public void CheckAndOpenPackPanel()
    {
        if (FindObjectOfType<StarterPackController>().CanDisplayPack()
        && DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_OPEN_TIMESTAMP, 0) > 6 * 3600)
        {
            if (CheckWinLoseOpenCondition()
                || DataController.Instance.Energy < 1
                || DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_OPEN_TIMESTAMP, 0) > 12 * 3600)
                FindObjectOfType<StarterPackController>().OnClickOpenPanel();
        }
        else
        {
            bool shouldOpenPack = false;
            if (PlayerClassifyController.Instance.GetPlayerClassifyLevel() > 0
            && DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat("pack_timestamp", 0) > 6 * 3600)
            {
                if (CheckWinLoseOpenCondition()
                || DataController.Instance.Energy < 1
                || DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat("pack_timestamp", 0) > 12 * 3600)
                    shouldOpenPack = true;
            }
            if (shouldOpenPack)
                OnClickOpenPackPanel();
        }
    }
    private bool CheckWinLoseOpenCondition()
    {
        if (PlayerClassifyController.Instance.winCount >= 3 || PlayerClassifyController.Instance.loseCount >= 3)
            return true;
        return false;
    }
    public void OnClickOpenPackPanel()//detect the current pack prefabs and instance
    {
        if (PlayerClassifyController.Instance.GetPlayerClassifyLevel() < 1) return;
        PlayerPrefs.SetFloat("pack_timestamp", (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        var salePack= Instantiate(GetActiveSalePrefab(), cameraCanvas);
        FindObjectOfType<MainMenuController>()?.setIndexTab(5);
    }
    private float GetPackCycleTime()
    {
        int packIndex = (PlayerClassifyController.Instance.GetPlayerClassifyLevel() - 1) * 2;
        float totalCycleTime = IAPPackHelper.GetPackLifeTime(originPacksId[packIndex]) + IAPPackHelper.GetPackLifeTime(originPacksId[packIndex + 1]);
        if (PlayerPrefs.GetInt(originPacksId[packIndex], 0) == 0
        && PlayerPrefs.GetInt(saleLevel1PacksId[packIndex], 0) == 0
        && PlayerPrefs.GetInt(saleLevel2PacksId[packIndex], 0) == 0)
            totalCycleTime = totalCycleTime + IAPPackHelper.GetPackLifeTime(saleLevel1PacksId[packIndex]) + IAPPackHelper.GetPackLifeTime(saleLevel2PacksId[packIndex]);
        if (PlayerPrefs.GetInt(originPacksId[packIndex + 1], 0) == 0
        && PlayerPrefs.GetInt(saleLevel1PacksId[packIndex + 1], 0) == 0
        && PlayerPrefs.GetInt(saleLevel2PacksId[packIndex + 1], 0) == 0)
            totalCycleTime = totalCycleTime + IAPPackHelper.GetPackLifeTime(saleLevel1PacksId[packIndex + 1]) + IAPPackHelper.GetPackLifeTime(saleLevel2PacksId[packIndex + 1]);
        return totalCycleTime;
    }
}
