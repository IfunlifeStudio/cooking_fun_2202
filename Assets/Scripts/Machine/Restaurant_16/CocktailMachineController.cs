using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CocktailMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private CocktailPlateController[] plates;
    [SerializeField] private AudioClip foodServeSfx;
    //[SerializeField] private Transform[] platePos;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;

    private bool isMachineUnlock;
    int plateIndex = 9;
    public Vector3 defaultKouchaPos = new Vector3(4.569f, -0.484f, 0.001f);
    private IEnumerator Start()
    {
        //defaultKouchaPos = transform.position;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int ingredient = DataController.Instance.GetMachineLevel(id);
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < ingredient);
        int ingredientLevel = DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(ingredientLevel.ToString());
        workDuration = DataController.Instance.GetMachineWorkTime(id) / ingredient;
        yield return null;
        FillIngredent();
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
            //animationController.PlayAnimation("doing", false);
            if (timeRemain <= 0)
            {

                state = MachineStates.Completed;
                activeClock.HideClock();
                gameObject.transform.localPosition = defaultKouchaPos - new Vector3(0.13f, 0, 0);
                animationController.PlayAnimation("idle_binh", true, workDuration / timeRemain);
                timeRemain = 0.2f;
            }
        }
        else if (state == MachineStates.Completed)
        {
            timeRemain -= Time.deltaTime;

            if (timeRemain <= 0)
            {
                plates[plateIndex].StoreCookedFood(foodId);
                PlayAudio(completeSfx);
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

                gameObject.transform.position = plates[indexTemp].gameObject.transform.position - new Vector3(-0.05f, 0.42f, 0);
                timeRemain = workDuration;
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
                {
                    timeRemain = workDuration = 0.06f;
                    GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0, -1f), instantCookEffect.transform.rotation);
                    Destroy(instantEffect, 2);
                }
                state = MachineStates.Working;
                PlayAudio(processSfx);
                animationController.PlayAnimation("doing", false, workDuration / timeRemain);
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
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }

}
