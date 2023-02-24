using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XmasPassController : MonoBehaviour
{
    [SerializeField] private Transform cameraCanvas;
    [SerializeField] private GameObject lockXmasPassBtn, unlockXmasPassBtn;
    [SerializeField] private GameObject xmasPassPanelPrefab, battlePassPanelPrefab, xmasUnlockPosterPrefab, xmasLockPosterPrefab;
    [SerializeField] private TextMeshProUGUI countdownTxt;
    // Start is called before the first frame update
    void Start()
    {
        if (DataController.Instance.IsShowNoel == 1)
            CheckUnlockEvent();
    }
    private void CheckUnlockEvent()
    {
        TimeSpan timeSpan = BattlePassDataController.EndTime - DateTime.UtcNow;
        if (timeSpan.TotalSeconds > 0)
        {
            if (BattlePassDataController.Instance.HasUnlockEvent)
            {
                unlockXmasPassBtn.SetActive(true);
                StartCoroutine(CoundowntBattlePassTime());
            }
            else if (BattlePassDataController.Instance.HasUnlockPoster)
            {
                unlockXmasPassBtn.SetActive(false);
                lockXmasPassBtn.SetActive(true);
                StartCoroutine(CoundowntBattlePassTime());
            }
            else
            {
                unlockXmasPassBtn.transform.parent.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            unlockXmasPassBtn.transform.parent.parent.gameObject.SetActive(false);
        }
    }
    IEnumerator CoundowntBattlePassTime()
    {
        DateTime endTime = BattlePassDataController.EndTime;
        var delay = new WaitForSeconds(60f);
        TimeSpan timeSpan = endTime - DateTime.UtcNow;
        while (timeSpan.TotalSeconds > 0)
        {
            timeSpan = endTime - DateTime.UtcNow;
            countdownTxt.text = String.Format("{0}d {1}h {2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            yield return delay;
        }
    }
    public void OnClickXmasBtn()
    {
        if (BattlePassDataController.Instance.HasUnlockEvent)
        {
            if (PlayerPrefs.GetInt("first_open_bp", 0) == 0)
            {
                PlayerPrefs.SetInt("first_open_bp", 1);
                OnShowUnlockPoster();
            }
            else
            {
                OnOpenBattlePass();
            }
        }
        else if (BattlePassDataController.Instance.HasUnlockPoster)
        {
            OnShowLockPoster();
        }
    }
    public bool CanShowPoster()
    {
        if (BattlePassDataController.Instance.IsFirstOpenPoster && BattlePassDataController.Instance.HasUnlockEvent)
        {
            BattlePassDataController.Instance.IsFirstOpenPoster = false;
            OnShowUnlockPoster();
            return true;
        }
        return false;
    }
    public void OnClickOpenXmasPass()
    {
        Instantiate(xmasPassPanelPrefab, cameraCanvas);
    }
    public void OnOpenBattlePass()
    {
        Instantiate(battlePassPanelPrefab, cameraCanvas);
    }
    public void OnShowUnlockPoster()
    {
        Instantiate(xmasUnlockPosterPrefab, cameraCanvas);
    }
    public void OnShowLockPoster()
    {
        Instantiate(xmasLockPosterPrefab, cameraCanvas);
    }
}
