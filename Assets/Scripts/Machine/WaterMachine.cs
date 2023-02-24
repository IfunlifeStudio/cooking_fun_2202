using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaterMachine : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private Transform coffeeSpawnPos;
    [SerializeField] private bool isEvent = false;
    [SerializeField] private AudioClip foodServeSfx;
    private IEnumerator Start()
    {
        workDuration = isEvent ? EventDataController.Instance.GetMachineWorkTime(id) : DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int ingredientLevel = isEvent ? EventDataController.Instance.GetIngredientLevel(foodId) : DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(ingredientLevel.ToString());
        yield return null;
        FillIngredent();
    }
    // Update is called once per frame
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
                activeClock.HideClock();
                state = MachineStates.Completed;
                animationController.PlayAnimation("lv4", false);
                AudioController.Instance.PlaySfx(completeSfx);
            }
        }
        else
        if (state == MachineStates.Completed)
        {
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && ServeFood())
            {
                GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                Destroy(autoEffect, 2);
            }
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
            return true;
        }
        return false;
    }
    public override bool ServeFood()
    {
        if (!isPlaying) return false;
        bool result = false;
        if (state == MachineStates.Completed)
        {
            if (CustomerFactory.Instance.CheckFood(foodId))
            {
                Food coffee = FoodFactory.Instance.SpawnFoodById(foodId, coffeeSpawnPos);
                state = MachineStates.Standby;
                CustomerFactory.Instance.ServeFood(coffee);
                coffee.DisposeFood();
                PlayAudio(foodServeSfx);
                FillIngredent();
                result = true;
            }
        }
        return result;
    }
    public override void TossFood()
    {
    }
    public override void BackToWorkingStatus()
    {
    }

    public override void SetupSkin()
    {
        throw new System.NotImplementedException();
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
