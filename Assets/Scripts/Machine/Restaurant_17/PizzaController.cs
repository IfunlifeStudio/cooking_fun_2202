using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaController : MonoBehaviour
{
    [SerializeField] private int machineId = 200;
    [SerializeField] private PizzaPlateController[] plates;
    // Start is called before the first frame update
    [SerializeField] private bool isEvent = false;
    void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineActive = isEvent ? EventDataController.Instance.IsMachineUnlocked(machineId, currentLevel) : DataController.Instance.IsMachineUnlocked(machineId, currentLevel);
        int plateLevel = isEvent ? EventDataController.Instance.GetMachineCount(plates[0].id) : DataController.Instance.GetMachineCount(plates[0].id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineActive && i < plateLevel);
    }
}
