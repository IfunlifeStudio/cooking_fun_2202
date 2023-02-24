using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoelUIDetailController : MonoBehaviour
{
    [SerializeField] private GameObject[] sockIcon;
    // Start is called before the first frame update
    void Start()
    {
        if (sockIcon.Length > 0)
        {
            for (int i = 0; i < sockIcon.Length; i++)
            {
                sockIcon[i].SetActive(DataController.Instance.IsShowNoel == 1);
            }
        }
    }

}
