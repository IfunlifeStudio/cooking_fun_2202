using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMachineLevelController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;

    [SerializeField] protected GameObject instantCookEffect, antiOverCookEffect, upgradeEffect;
    //public SpriteRenderer machineBody;
    public SpriteRenderer locked;
    [SerializeField] protected ServePlateController[] plates;
    public Food activeFood = null;
    public bool isUnlock = false;
    public CapsuleCollider2D capsuleCollider2D;
    protected virtual void Start()
    {
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        int foodLevel = DataController.Instance.GetIngredientLevel(foodId);
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        animationController.SetMachineSkin(foodLevel.ToString());
        state = MachineStates.Standby;
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
            //animationController.Pause();
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (timeRemain <= 0)
            {
                capsuleCollider2D.enabled = true;
                state = MachineStates.Completed;
                timeRemain = burnTime = 15;
                PlayAudio(completeSfx);
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                    activeClock.HideClock();
                animationController.PlayAnimation("lv2", true);
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
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position);
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
    public bool IsMachineStandby()
    {
        return state == MachineStates.Standby;
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
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0, -1f), instantCookEffect.transform.rotation);
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
        bool result = false;
        if (state == MachineStates.Completed)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].gameObject.activeInHierarchy) continue;
                if (plates[i].StoreCookedFood(foodId))
                {
                    state = MachineStates.Standby;
                    //transform.GetChild(0).gameObject.SetActive(false);
                    activeClock.HideClock();
                    antiOverCookEffect.SetActive(false);
                    animationController.PlayAnimation("idle", true);
                    result = true;
                    break;
                }
            }
        }
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            TossFood();
            result = true;
        }
        if (result)
        {

            capsuleCollider2D.enabled = false;
        }

        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
        state = MachineStates.Standby;
        animationController.PlayAnimation("idle", true);
        activeClock.HideClock();
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
    }
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
        ServeFood();
    }

    public override void BackToWorkingStatus()
    {
        capsuleCollider2D.enabled = true;
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
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
