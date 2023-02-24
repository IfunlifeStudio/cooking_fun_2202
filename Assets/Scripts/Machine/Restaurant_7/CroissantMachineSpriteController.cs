using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroissantMachineSpriteController : MonoBehaviour
{
    [SerializeField] private Machine[] croissantMachines;
    [SerializeField] private GameObject[] crepeLockSprites;
    // Start is called before the first frame update
    void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        int machineCount = DataController.Instance.GetMachineCount(croissantMachines[0].id);
        bool isMachineUnlock = DataController.Instance.IsMachineUnlocked(croissantMachines[0].id, currentLevel);
        for (int i = 0; i < croissantMachines.Length; i++)
            croissantMachines[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        for (int i = 0; i < croissantMachines.Length; i++)
            crepeLockSprites[i].SetActive(!isMachineUnlock || i >= machineCount);
        HasCompletedUpgradeProcessing();
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(croissantMachines[0].id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(croissantMachines[0].id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                for (int i = 0; i < croissantMachines.Length; i++)
                {
                    croissantMachines[i].ShowUpgradeProcessAgain();
                }
                DataController.Instance.RemoveMachineUpgradeProcess(croissantMachines[0].id);
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {

                DataController.Instance.CompletedMachineUpgrade(croissantMachines[0].id, false, true);
                for (int i = 0; i < croissantMachines.Length; i++)
                {
                    croissantMachines[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(croissantMachines[0].id);
                return true;
            }
        }
        return false;
    }
}
