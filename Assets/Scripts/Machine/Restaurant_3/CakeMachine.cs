using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
public class CakeMachine : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect, antiOverCookEffect, upgradeEffect;
    private MachineClockController activeClock;
    private BoxCollider2D boxCollider2D;
    private Vector3 originScale;
    private void Start()
    {
        originScale = transform.localScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        animationController.SetMachineSkin(machineLevel.ToString());
    }
    private void OnDestroy()
    {
        if (activeClock != null)
            activeClock.Dispose();
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
                boxCollider2D.enabled = true;
                state = MachineStates.Completed;
                animationController.PlayAnimation("lv2", true);
                timeRemain = burnTime = 15;
                if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
                {
                    timeRemain = burnTime = 23;
                }
                else if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
                {
                    timeRemain = burnTime = 12;
                }
                PlayAudio(completeSfx);
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                    activeClock.HideClock();
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
                PlayAudio(burnSfx);
            }
        }
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            state = MachineStates.Working;
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                antiOverCookEffect.SetActive(true);
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0.45f, 0), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            animationController.PlayAnimation("lv1", true);
            PlayAudio(processSfx);
            return true;
        }
        return false;
    }
    public override bool ServeFood()
    {
        if (state == MachineStates.Completed)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].gameObject.activeInHierarchy) continue;
                if (plates[i].StoreCookedFood(foodId))
                {
                    state = MachineStates.Standby;
                    animationController.PlayAnimation("idle", true);
                    activeClock.HideClock();
                    boxCollider2D.enabled = false;
                    antiOverCookEffect.SetActive(false);
                    return true;
                }
            }
        }
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            TossFood();
            boxCollider2D.enabled = false;
            return true;
        }

        if (state == MachineStates.Completed)
        {
            plates[0].Pulse();
        }
        clickTimeStamp = Time.time;
        return false;
    }
    public override void TossFood()
    {
        state = MachineStates.Standby;
        animationController.PlayAnimation("idle", true);//simulate toss food to trash
        activeClock.HideClock();
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
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
        boxCollider2D.enabled = true;
        state = MachineStates.Completed;
        animationController.PlayAnimation("lv2", true);
        timeRemain = burnTime;
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
