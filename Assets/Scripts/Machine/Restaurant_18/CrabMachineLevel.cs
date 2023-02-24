using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMachineLevel : MonoBehaviour
{
    [SerializeField] private CrabMachineLevelController[] machines;
    public int machineId = 1;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineUnlock = DataController.Instance.IsMachineUnlocked(machineId, currentLevel);
        int machineCount = DataController.Instance.GetMachineCount(machineId);
        for (int i = 0; i < machines.Length; i++)
        {
            bool canUnlock = isMachineUnlock && i < machineCount;
            machines[i].isUnlock = canUnlock;
            machines[i].locked.gameObject.SetActive(!canUnlock);
            machines[i].capsuleCollider2D.enabled = canUnlock;
        }
        if (isMachineUnlock)
            if (isMachineUnlock && !HasCompletedUpgradeProcessing())
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
                    if (machines[i].isUnlock)
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
                    if (machines[i].isUnlock)
                        machines[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(machines[0].id);
                return true;
            }
        }
        return false;
    }
}
