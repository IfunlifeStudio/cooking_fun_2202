using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
public class MultiWaterMachine : MonoBehaviour
{
    [SerializeField] private GameObject[] machineBodies;
    [SerializeField] private Machine[] machines;
    [SerializeField] private GameObject[] machineColliders;
    [SerializeField] private bool isEvent = false;
    private int machineLevel;
    private Vector3 originScale;
    public void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        this.machineLevel = isEvent ? EventDataController.Instance.GetMachineLevel(machines[0].id) : DataController.Instance.GetMachineLevel(machines[0].id);
        bool isMachineUnlock = isEvent ? EventDataController.Instance.IsMachineUnlocked(machines[0].id, currentLevel) : DataController.Instance.IsMachineUnlocked(machines[0].id, currentLevel);
        int machineCount = isEvent ? EventDataController.Instance.GetMachineCount(machines[0].id) : DataController.Instance.GetMachineCount(machines[0].id);
        for (int i = 0; i < machines.Length; i++)
            machines[i].gameObject.SetActive(i < machineCount && isMachineUnlock);
        for (int i = 0; i < machineBodies.Length; i++)
            machineBodies[i].gameObject.SetActive(i == machineLevel - 1 && isMachineUnlock);
        for (int i = 0; i < machineColliders.Length; i++)
            machineColliders[i].gameObject.SetActive(i == machineLevel - 1 && isMachineUnlock);
        HasCompletedUpgradeProcessing();
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(machines[0].id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(machines[0].id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                for (int i = 0; i < machines.Length; i++)
                {
                    machines[i].ShowUpgradeProcessAgain();
                }
                DataController.Instance.RemoveMachineUpgradeProcess(machines[0].id);
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {

                DataController.Instance.CompletedMachineUpgrade(machines[0].id, false, true);
                for (int i = 0; i < machines.Length; i++)
                {
                    machines[i].ShowUpgradeProcess();
                }
                DataController.Instance.HasCompletedMachineUpgradeInGame(machines[0].id);
                return true;
            }
        }
        return false;
    }
    public void ServeFood()
    {
        for (int i = 0; i < machineLevel; i++)
            if (machines[i].ServeFood())
                break;
    }
    public void OnSelect(LeanFinger finger)
    {
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
                   {
                       transform.localScale = t.CurrentValue;
                   };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.06f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.12f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
        ServeFood();
    }
}
