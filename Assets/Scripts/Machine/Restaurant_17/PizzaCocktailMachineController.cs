using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaCocktailMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private AudioClip foodServeSfx;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;
    private bool isMachineUnlock;

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
                animationController.PlayAnimation("idle", false);
                for (int i = 0; i < plates.Length; i++)
                {
                    if (!plates[i].gameObject.activeInHierarchy) continue;
                    plates[i].StoreCookedFood(foodId);
                }
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && ServeFood())
                {
                    GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                    Destroy(autoEffect, 2);
                    FillIngredent();
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
        timeRemain = workDuration;
        if (state == MachineStates.Standby)
        {

            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0, -0.5f), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            state = MachineStates.Working;
            PlayAudio(processSfx);
            animationController.PlayAnimation("doing", false);
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
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
