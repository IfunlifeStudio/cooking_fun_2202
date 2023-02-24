using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BpElementController : MonoBehaviour
{
    [SerializeField] private BpItemController freeBp;
    [SerializeField] private BpVipItemController vipBp;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject lockLevelGo, startPointGo;

    public int Level;
    public void InItBpElement(BattlePassElementData bpElementData, bool activeLockBar)
    {
        startPointGo.SetActive(bpElementData.Level == 30);
        levelText.gameObject.SetActive(bpElementData.Level != 30);
        this.Level = bpElementData.Level;
        levelText.text = Level.ToString();
        lockLevelGo.SetActive(activeLockBar);
        var bpLevelState = BattlePassDataController.Instance.GetBattlePassLevelState(Level);
        freeBp.SetupUi(bpElementData.FreeItem, Level, BattlePassDataController.Instance.CurrentBpLevel >= Level, bpLevelState.hasClaimedFree);
        vipBp.SetupUi(bpElementData.VipItem, Level, BattlePassDataController.Instance.CurrentBpLevel >= Level, bpLevelState.hasClaimedVip);
    }
    public void OnLevelUp(int level)
    {

        bool activeLockGo = this.Level + 1 == level;
        lockLevelGo.SetActive(activeLockGo);
    }
    public void SetLockBarActive(bool status)
    {
        lockLevelGo.SetActive(status);
    }

}

