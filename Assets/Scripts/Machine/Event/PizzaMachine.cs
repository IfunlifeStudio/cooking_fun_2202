using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
public class PizzaMachine : Machine
{
    [SerializeField] private Transform clockPos, foodPos;
    [SerializeField] private MachineClockController clockPrefab;
    [SerializeField] private GameObject instantCookEffect, antiOverCookEffect;
    [SerializeField] private GameObject autoServeEffect;
    private MachineClockController activeClock;
    [SerializeField] private int[] cookableFoodIds;
    public int activeFoodId;
    private Vector3 originScale;
    private void Start()
    {
        originScale = transform.localScale;
        workDuration = EventDataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        int machineLevel = EventDataController.Instance.GetMachineLevel(id);
        animationController.SetMachineSkin(machineLevel.ToString());
    }
    private void OnDestroy()
    {
        if (activeClock != null)
            activeClock.Dispose();
    }
    void Update()
    {
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
                animationController.PlayAnimation("lv2", true);
                timeRemain = burnTime = 15;
                if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
                {
                    timeRemain = burnTime = 23;
                }
                else
                if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
                {
                    timeRemain = burnTime = 12;
                }
                AudioController.Instance.PlaySfx(completeSfx);
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                    activeClock.HideClock();
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && ServeFood())
                {
                    GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                    Destroy(autoEffect, 2);
                }
            }
        }
        else if (state == MachineStates.Completed &&
        !LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))//check no-burn item
        {
            timeRemain -= Time.deltaTime;//simulate burn product
            activeClock.SetFillAmount(1 - timeRemain / burnTime, false);
            if (timeRemain <= 0)
            {
                state = MachineStates.Burned;
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position + new Vector3(0, 1, 0));
                else
                    MessageManager.Instance.SendMessage(
                        new Message(CookingMessageType.OnFoodBurned,
                        new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position + new Vector3(0, 1, 0), this }));
                FindObjectOfType<GameController>().OnFoodBurned(foodId);
                activeClock.HideClock();
                animationController.PlayAnimation("lv3", true);
                AudioController.Instance.PlaySfx(burnSfx);
            }
        }
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            int _foodIdIndex = -1;
            for (int i = 0; i < cookableFoodIds.Length; i++)
                if (cookableFoodIds[i] == activeFoodId)
                    _foodIdIndex = i;
            if (_foodIdIndex == -1) return false;
            animationController.SetMachineSkin((_foodIdIndex + 1).ToString());
            state = MachineStates.Working;
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                antiOverCookEffect.SetActive(true);
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position, instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            animationController.PlayAnimation("lv1", true);
            AudioController.Instance.PlaySfx(processSfx);
            return true;
        }
        return false;
    }
    public override bool ServeFood()
    {
        bool result = false;
        if (state == MachineStates.Completed)
        {
            if (CustomerFactory.Instance.CheckFood(activeFoodId))
            {
                Food food = FoodFactory.Instance.SpawnFoodById(activeFoodId, foodPos);
                food.gameObject.SetActive(false);
                CustomerFactory.Instance.ServeFood(food);
                food.DisposeFood();
                state = MachineStates.Standby;
                animationController.PlayAnimation("idle", true);
                activeClock.HideClock();
                antiOverCookEffect.SetActive(false);
                activeFoodId = 0;
                result = true;
            }
        }
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            TossFood();
            result = true;
        }
        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
        state = MachineStates.Standby;
        animationController.PlayAnimation("idle", true);//simulate toss food to trash
        activeClock.HideClock();
        FindObjectOfType<TrashBinController>().TossTrash(activeFoodId);
    }
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
                   {
                       transform.localScale = t.CurrentValue;
                   };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
        ServeFood();
    }
    public void OnSelectUp(LeanFinger finger)
    {
    }

    public override void BackToWorkingStatus()
    {
        state = MachineStates.Completed;
        animationController.PlayAnimation("lv2", true);
        timeRemain = burnTime;
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
