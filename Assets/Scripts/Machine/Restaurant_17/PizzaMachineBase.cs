using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaMachineBase : Machine
{
    public int countMachine = 0;
    public SpriteRenderer machineBody;
    public SpriteRenderer locked;
    public Sprite[] machineBodiesSprites;
    public bool isMachineUnlock;
    public BoxCollider2D boxCollider2D;
    public Food activeFood = null;
    public virtual void Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        int machineCount = DataController.Instance.GetMachineCount(id);
        if (isMachineUnlock && countMachine < machineCount)
        {
            machineBody.gameObject.SetActive(false);
            locked.gameObject.SetActive(false);
            boxCollider2D.isTrigger = false;
            machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        }
        else
        {
            machineBody.gameObject.SetActive(true);
            locked.gameObject.SetActive(true);
            boxCollider2D.isTrigger = true;
        }
    }
    public override void BackToWorkingStatus()
    {
        throw new System.NotImplementedException();
    }

    public override bool FillIngredent()
    {
        throw new System.NotImplementedException();
    }

    public override bool ServeFood()
    {
        throw new System.NotImplementedException();
    }

    public override void TossFood()
    {
        throw new System.NotImplementedException();
    }

    public override void SetupSkin()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowUpgradeProcessAgain()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowUpgradeProcess()
    {
        throw new System.NotImplementedException();
    }
}
