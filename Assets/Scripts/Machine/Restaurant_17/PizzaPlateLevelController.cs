using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaPlateLevelController : MonoBehaviour
{
    [SerializeField] private int machineId = 200;
    [SerializeField] private PizzaPlateController[] plates;
    [SerializeField] private ServePlateController[] servePlates;
    // Start is called before the first frame update
    [SerializeField] private bool isEvent = false;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineActive = isEvent ? EventDataController.Instance.IsMachineUnlocked(machineId, currentLevel) : DataController.Instance.IsMachineUnlocked(machineId, currentLevel);
        int plateLevel = DataController.Instance.GetMachineCount(plates[0].id);
        if (plates.Length > 0)
        {
            for (int i = 0; i < plates.Length; i++)
                plates[i].gameObject.SetActive(isMachineActive && i < plateLevel);
            if (!HasCompletedUpgradeProcessing())
                for (int i = 0; i < plates.Length; i++)
                    plates[i].SetupSkin();
        }

        if (servePlates.Length > 0)
        {
            for (int i = 0; i < servePlates.Length; i++)
                servePlates[i].gameObject.SetActive(isMachineActive && i < plateLevel);
        }

    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(plates[0].id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(plates[0].id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                for (int i = 0; i < plates.Length; i++)
                {
                    plates[i].ShowUpgradeProcessAgain();
                }
                DataController.Instance.RemoveMachineUpgradeProcess(plates[0].id);
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {

                DataController.Instance.CompletedMachineUpgrade(plates[0].id, false, true);
                for (int i = 0; i < plates.Length; i++)
                {
                    plates[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(plates[0].id);
                return true;
            }
        }
        return false;
    }
}
