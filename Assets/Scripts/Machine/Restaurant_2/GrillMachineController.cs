using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillMachineController : MonoBehaviour
{
    [SerializeField] private Machine[] sausageMachines, steakMachines;
    [SerializeField] private GameObject[] sausageLocks, steakLocks;
    private bool isMachineWorking;
    private float timeStamp;
    private void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        if (sausageMachines.Length > 0 && sausageMachines != null)
        {
            int machineCount = DataController.Instance.GetMachineCount(sausageMachines[0].id);
            bool isMachineUnlock = DataController.Instance.IsMachineUnlocked(sausageMachines[0].id, currentLevel);
            for (int i = 0; i < sausageMachines.Length; i++)
                sausageLocks[i].SetActive(!isMachineUnlock || i >= machineCount);
        }
        if (steakMachines.Length > 0 && steakMachines != null)
        {
            int machineCount = DataController.Instance.GetMachineCount(steakMachines[0].id);
            bool isMachineUnlock = DataController.Instance.IsMachineUnlocked(steakMachines[0].id, currentLevel);
            for (int i = 0; i < steakMachines.Length; i++)
                steakLocks[i].SetActive(!isMachineUnlock || i >= machineCount);
        }
    }
}
