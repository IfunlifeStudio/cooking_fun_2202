using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrepeMachineSpriteController : MonoBehaviour
{
    [SerializeField] private Machine[] crepeMachines;
    [SerializeField] private SpriteRenderer[] crepeSprites;
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        int machineCount = DataController.Instance.GetMachineCount(crepeMachines[0].id);
        bool isMachineUnlock = DataController.Instance.IsMachineUnlocked(crepeMachines[0].id, currentLevel);
        for (int i = 0; i < crepeMachines.Length; i++)
            crepeMachines[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        for (int i = 0; i < crepeMachines.Length; i++)
            crepeSprites[i].gameObject.SetActive(isMachineUnlock && i == machineCount - 1);
        CheckCompletedUpgradeProcessing();

    }
    public void CheckCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(crepeMachines[0].id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(crepeMachines[0].id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                for (int i = 0; i < crepeMachines.Length; i++)
                {
                    crepeMachines[i].ShowUpgradeProcessAgain();
                }
                DataController.Instance.RemoveMachineUpgradeProcess(crepeMachines[0].id);
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {

                DataController.Instance.CompletedMachineUpgrade(crepeMachines[0].id, false, true);
                for (int i = 0; i < crepeMachines.Length; i++)
                {
                    crepeMachines[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(crepeMachines[0].id);
            }
        }
    }
}
