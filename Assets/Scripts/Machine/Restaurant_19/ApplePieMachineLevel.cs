using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplePieMachineLevel : MonoBehaviour
{
    public ApplePieMachineController[] machine;
    public int id;
    public GameObject []bodyMachine;
    bool isMachineUnlock = false;
    int machineCount=-1;
    [HideInInspector] public int machineIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);         
        machineCount = DataController.Instance.GetMachineCount(id);
        for(int i=0;i<machine.Length;i++)
        {
            machine[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
            //Locked[i].SetActive(!isMachineUnlock && i > machineCount);
        }
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        for (int j=0;j<bodyMachine.Length;j++)
        {
            if (j == machineLevel - 1)
                bodyMachine[j].SetActive(true);
            else bodyMachine[j].SetActive(false);
        }    
        
    }

   
}
