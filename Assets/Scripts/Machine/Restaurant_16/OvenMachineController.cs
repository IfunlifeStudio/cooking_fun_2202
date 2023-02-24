﻿using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect, upgradeEffect;
    private Vector3 originScale;
    private bool isMachineUnlock;
    bool isMachineStandby = false;
    int machineLevel = 1;
    private IEnumerator Start()

    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        machineLevel = DataController.Instance.GetMachineLevel(id);
        //animationController.SetMachineSkin(machineLevel.ToString());
        yield return null;
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
                activeClock.HideClock();
                animationController.PlayAnimation("idle", false, workDuration / timeRemain);
                PlayAudio(completeSfx);
                for (int i = 0; i < plates.Length; i++)
                {
                    if (!plates[i].gameObject.activeInHierarchy) continue;
                    plates[i].StoreCookedFood(foodId);
                }
                state = MachineStates.Standby;
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
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(-0.6f, 0, 0), instantCookEffect.transform.rotation);
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
    public void OnSelect(LeanFinger finger)
    {
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].activeFood == null)
            {
                isMachineStandby = true;
                break;
            }
            else isMachineStandby = false;
        }
        if (!isPlaying) return;
        //if (!isMachineStandby) return;
        if (isMachineUnlock)
        {
            ScaleMachine();
            FillIngredent();
        }
    }
    public void OnSelectUp(LeanFinger finger)
    {
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
        ScaleMachine();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
    private void ScaleMachine()
    {
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
    }
}
