using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KouchaMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private AudioClip foodServeSfx;
    //[SerializeField] private Transform[] platePos;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;

    private bool isMachineUnlock;
    int plateIndex = 9;
    Vector3 defaultKouchaPos;
    int machineLevel = 1;
    private IEnumerator Start()
    {
        defaultKouchaPos = transform.position;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        machineLevel = DataController.Instance.GetMachineLevel(id);
        if (!HasCompletedUpgradeProcessing())
            SetupSkin();
        workDuration = DataController.Instance.GetMachineWorkTime(id) / machineLevel;
        yield return null;
        FillIngredent();
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeMachineProcessing(id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetMachineUpgradeTime(id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                DataController.Instance.RemoveMachineUpgradeProcess(id);
                ShowUpgradeProcessAgain();
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedMachineUpgrade(id, false, true);
                ShowUpgradeProcess();
                return true;
            }
        }
        return false;
    }
    void Update()
    {
        if (Time.timeScale == 0)
            if (audioSource != null)
                audioSource.Pause();
        else
            if (audioSource != null)
                audioSource.UnPause();
        if (!isPlaying)
        {
            animationController.Pause();
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (timeRemain <= 0)
            {
                animationController.PlayAnimation("lv1", false);
                state = MachineStates.Completed;
                activeClock.HideClock();
                timeRemain = 0.1f;
            }
        }
        else if (state == MachineStates.Completed)
        {
            timeRemain -= Time.deltaTime;
            if (timeRemain <= 0)
            {
                PlayAudio(completeSfx);
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && ServeFood())
                {
                    GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                    Destroy(autoEffect, 2);
                    FillIngredent();
                }
                plates[plateIndex].StoreCookedFood(foodId);
                animationController.PlayAnimation("idle", false);
                state = MachineStates.Standby;
            }
        }
        else if (state == MachineStates.Standby)
        {
            gameObject.transform.position = defaultKouchaPos;
            int indexTemp = GetReadyMachine();
            if (indexTemp != 9) FillIngredent();
        }
    }
    public override bool FillIngredent()
    {
        int indexTemp = GetReadyMachine();
        if (plateIndex != 9)
            if (state == MachineStates.Standby)
            {

                gameObject.transform.position = plates[indexTemp].gameObject.transform.position;
                timeRemain = workDuration;
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
                {
                    timeRemain = workDuration = 0.06f;
                    GameObject instantEffect = Instantiate(instantCookEffect, transform.position, instantCookEffect.transform.rotation);
                    Destroy(instantEffect, 2);
                }
                state = MachineStates.Working;
                PlayAudio(processSfx);
                animationController.PlayAnimation("lv1", true);
                return true;
            }
        return false;
    }
    public override bool ServeFood()
    {
        return false;
    }

    public override void TossFood()
    {
    }
    public int GetReadyMachine()
    {
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].activeFood == null)
            {
                plateIndex = i;
                return plateIndex;
            }
        }
        return 9;
    }
    public void OnSelect(LeanFinger finger)
    {
        for (int i = 0; i < plates.Length; i++)
        {
            if (plates[i].activeFood != null)
            {
                bool foodHasDispose = plates[i].activeFood.OnClick();
                if (foodHasDispose)
                {
                    plates[i].ScalePlate();
                    PlayAudio(foodServeSfx);
                    plates[i].activeFood = null;
                    state = MachineStates.Standby;
                    FillIngredent();
                }
                return;
            }
        }
    }

    public override void BackToWorkingStatus()
    {

    }

    public override void SetupSkin()
    {
        animationController.SetMachineSkin(machineLevel.ToString());
    }

    public override void ShowUpgradeProcessAgain()
    {
        animationController.SetMachineSkin((machineLevel - 1).ToString());
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel));
    }

    public override void ShowUpgradeProcess()
    {
        animationController.SetMachineSkin(machineLevel.ToString());
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel + 1));
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(4f);
        if (level > 3) level = 3;
        animationController.SetMachineSkin(level.ToString());
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
