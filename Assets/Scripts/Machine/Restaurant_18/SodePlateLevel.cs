using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodePlateLevel : MonoBehaviour
{
    [SerializeField] private int machineId = 200;
    [SerializeField] private ServePlateController[] plates;
    // Start is called before the first frame update
    [SerializeField] private bool isEvent = false;
    void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        bool isMachineActive = DataController.Instance.IsMachineUnlocked(machineId, currentLevel);
        int plateLevel = DataController.Instance.GetMachineCount(machineId);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineActive && i < plateLevel);
    }
}
