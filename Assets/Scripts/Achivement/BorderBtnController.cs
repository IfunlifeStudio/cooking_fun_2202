using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderBtnController : MonoBehaviour
{
    [SerializeField] int borderId;
    [SerializeField] GameObject lockGo;
    bool hasUnlocked;
    public bool HasBorderUnlocked()
    {
        if (borderId < 4)
        {
            hasUnlocked = true;
            return true;
        }
        else
        {
            hasUnlocked = DataController.Instance.GetGameData().borderCollection.Contains(borderId);
            if (lockGo != null)
                lockGo.SetActive(!hasUnlocked);
            return hasUnlocked;
        }

    }
}
