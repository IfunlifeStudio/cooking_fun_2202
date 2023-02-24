using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AlignPlatePosition : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Vector3[] positionList;
    [SerializeField] ServePlateController[] servePlates;
    // Start is called before the first frame update
    void Start()
    {
        int levelMachine = DataController.Instance.GetMachineLevel(id);
        if(levelMachine == 1)
            servePlates[0].transform.position = positionList[0];
        if (levelMachine == 2)
        {
            servePlates[0].transform.position = positionList[1];
            servePlates[1].transform.position = positionList[2];
        }
    }
}
