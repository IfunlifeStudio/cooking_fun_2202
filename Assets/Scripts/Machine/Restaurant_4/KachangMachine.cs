using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KachangMachine : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;
    [SerializeField] private MachineAnimationController machineBody;
    private bool isMachineUnlock;
    private IEnumerator Start()
    {
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        state = MachineStates.Standby;
        if(isMachineUnlock)
            activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        machineBody.SetMachineSkin(machineLevel.ToString());
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
            animationController.Pause();
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (timeRemain <= 0)
            {
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
                for (int i = 0; i < plates.Length; i++)
                {
                    if (!plates[i].gameObject.activeInHierarchy) continue;
                    plates[i].StoreCookedFood(foodId);
                }
                animationController.PlayAnimation("idle", false);
                machineBody.PlayAnimation("idle", false);
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
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position, instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            state = MachineStates.Working;
            PlayAudio(processSfx);
            animationController.PlayAnimation("lv1", true);
            machineBody.PlayAnimation("lv1", false);
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

    public override void SetupSkin()
    {
    }

    public override void ShowUpgradeProcessAgain()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade());
    }

    public override void ShowUpgradeProcess()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade());
    }
    IEnumerator DelayUpgrade()
    {
        yield return new WaitForSeconds(4f);
        ScaleMachine();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
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
