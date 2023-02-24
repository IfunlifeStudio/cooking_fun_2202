using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheesePlateController : MonoBehaviour
{
    [SerializeField] private int machineId = 200;

    [SerializeField] private ServePlateController[] servePlates;
    // Start is called before the first frame update
    [SerializeField] private bool isEvent = false;
    void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineActive = isEvent ? EventDataController.Instance.IsMachineUnlocked(machineId, currentLevel) : DataController.Instance.IsMachineUnlocked(machineId, currentLevel);


        if (servePlates.Length > 0)
        {
            int plateLevel = isEvent ? EventDataController.Instance.GetMachineCount(servePlates[0].id) : DataController.Instance.GetMachineCount(servePlates[0].id);
            for (int i = 0; i < servePlates.Length; i++)
                servePlates[i].gameObject.SetActive(isMachineActive && i < plateLevel);
        }

    }
}
