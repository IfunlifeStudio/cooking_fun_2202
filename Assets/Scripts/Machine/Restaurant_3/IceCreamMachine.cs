using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
public class IceCreamMachine : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, upgradeEffect;
    [SerializeField] private SpriteRenderer machineBody;
    [SerializeField] private Sprite[] machineBodiesSprites;
    private Vector3 originScale;
    private bool isMachineUnlock;
    int machineLevel = 1;
    private void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
        int machineCount = DataController.Instance.GetMachineCount(id);
        for (int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(isMachineUnlock && i < machineCount);
        machineLevel = DataController.Instance.GetMachineLevel(id);
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        int ingredientLevel = DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(ingredientLevel.ToString());
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
                animationController.PlayAnimation("lv4", false);
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
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
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
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
    }

    public override void ShowUpgradeProcessAgain()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 2];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel));
    }

    public override void ShowUpgradeProcess()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel + 1));
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(5f);
        if (level > 3) level = 3;
        machineBody.sprite = machineBodiesSprites[level - 1];
        ScaleMachine();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
    public void ScaleMachine()
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
