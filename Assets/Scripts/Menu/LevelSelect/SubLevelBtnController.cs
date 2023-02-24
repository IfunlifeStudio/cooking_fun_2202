using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SubLevelBtnController : MonoBehaviour
{
    //[SerializeField] private Image[] subStageBackground;
    [SerializeField] private GameObject key, subStageBackground;
    public void Init(bool isActive, bool isKey)
    {
        subStageBackground.SetActive(isActive);
        key.SetActive(isKey);
    }
}
