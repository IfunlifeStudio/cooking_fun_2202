using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
public class MultiMachineController : MonoBehaviour
{
    [SerializeField] private Machine[] machines;
    [SerializeField] private GameObject[] machineColliders;
    private int machineLevel;
    [SerializeField] private bool isEvent = false;
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineUnlock = isEvent ? EventDataController.Instance.IsMachineUnlocked(machines[0].id, currentLevel) : DataController.Instance.IsMachineUnlocked(machines[0].id, currentLevel);
        this.machineLevel = isEvent ? EventDataController.Instance.GetMachineLevel(machines[0].id) : DataController.Instance.GetMachineLevel(machines[0].id);
        int machineCount = isEvent ? EventDataController.Instance.GetMachineCount(machines[0].id) : DataController.Instance.GetMachineCount(machines[0].id);
        for (int i = 0; i < machines.Length; i++)
            machines[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        if (machineColliders != null && machineColliders.Length > 0)
            machineColliders[machineLevel - 1].SetActive(true);
        if (isMachineUnlock)
            if (!HasCompletedUpgradeProcessing())
            {
                for (int i = 0; i < machines.Length; i++)
                    machines[i].SetupSkin();
            }
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(machines[0].id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(machines[0].id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                for (int i = 0; i < machines.Length; i++)
                {
                    machines[i].ShowUpgradeProcessAgain();
                }
                DataController.Instance.RemoveMachineUpgradeProcess(machines[0].id);
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {

                DataController.Instance.CompletedMachineUpgrade(machines[0].id, false, true);
                for (int i = 0; i < machines.Length; i++)
                {
                    machines[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(machines[0].id);
                return true;
            }
        }
        return false;
    }
    public void OnSelect(LeanFinger finger)
    {
        for (int i = 0; i < machineLevel; i++)
            if (machines[i].ServeFood())
                break;
    }
}
