using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;
using Lean.Touch;
using UnityEngine;

public class SodaMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;
    [SerializeField] private SpriteRenderer machineBody;
    [SerializeField] private SpriteRenderer machineBodyCoc;
    [SerializeField] private Sprite[] machineBodiesSprites;
    [SerializeField] private Sprite[] machineBodiesCoc;
    [SerializeField] private AudioClip foodServeSfx;
    private bool isMachineUnlock;
    int machineLevel = 1;
    private IEnumerator Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        machineLevel = DataController.Instance.GetMachineLevel(id);
        if (!HasCompletedUpgradeProcessing())
        {
            SetupSkin();
        }
        int ingredientLevel = DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(ingredientLevel.ToString());
        yield return null;
        FillIngredent();
    }
    void Update()
    {
        if (Time.timeScale == 0)
            if (audioSource != null)
                audioSource.Pause();
        else
            if (audioSource != null) audioSource.UnPause();
        if (!isPlaying)
        {
            //animationController.Pause();
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (!plates[0].gameObject.activeInHierarchy || plates[0].activeFood == null)
                machineBodyCoc.enabled = true;
            if (timeRemain <= 0)
            {
                machineBodyCoc.enabled = false;
                animationController.Pause();
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
                for (int i = 0; i < plates.Length; i++)
                {
                    if (!plates[i].gameObject.activeInHierarchy) continue;
                    plates[i].StoreCookedFood(foodId);
                }
                state = MachineStates.Standby;
            }
        }
        else if (state == MachineStates.Standby)
        {
            bool canRun = false;
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].gameObject.activeInHierarchy) continue;
                if (plates[i].activeFood == null) canRun = true;
            }
            if (canRun) FillIngredent();
        }
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0, -1f), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            state = MachineStates.Working;
            PlayAudio(processSfx);
            animationController.Resume();
            animationController.PlayAnimation("doing", true);

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

    public override void BackToWorkingStatus()
    {
    }
    public void OnSelect(LeanFinger finger)
    {
        if (CustomerFactory.Instance.CheckFood(foodId))
        {
            for (int i = 0; i < plates.Length; i++)
            {
                if (plates[i].activeFood != null)
                {
                    bool foodHasDispose = plates[i].activeFood.OnClick();
                    if (foodHasDispose)
                    {
                        PlayAudio(foodServeSfx);
                        plates[i].activeFood = null;
                    }
                    state = MachineStates.Standby;
                    FillIngredent();
                    return;
                }
            }
        }
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
    public override void SetupSkin()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        machineBodyCoc.sprite = machineBodiesCoc[machineLevel - 1];
        machineBodyCoc.enabled = false;
    }

    public override void ShowUpgradeProcessAgain()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 2];
        machineBodyCoc.sprite = machineBodiesCoc[machineLevel - 2];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel));
    }

    public override void ShowUpgradeProcess()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        machineBodyCoc.sprite = machineBodiesCoc[machineLevel - 1];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel + 1));
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(4f);
        if (level > 3) level = 3;
        animationController.SetMachineSkin(level.ToString());
        ScaleMachine();
        GameObject go = Instantiate(upgradeEffect, transform.position, Quaternion.identity);
        Destroy(go, 1);
    }
    public void ScaleMachine()
    {
        Vector3 originScale = transform.localScale;
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
    }
}
