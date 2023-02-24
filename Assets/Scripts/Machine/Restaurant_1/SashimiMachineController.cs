using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiMachineController : Machine
{
    [SerializeField] private Sprite[] machineSprites;
    [SerializeField] private SpriteRenderer machineSprite;
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect;
    [SerializeField] GameObject upgradeEffect;
    [SerializeField] private bool isEvent = false;
    private Vector3 originScale;
    private MachineClockController activeClock;
    private BoxCollider2D boxCollider2D;
    int machineLevel = 1;
    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        originScale = transform.localScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        machineLevel = DataController.Instance.GetMachineLevel(id);
        int foodLevel = DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(foodLevel.ToString());
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(5f);
        if (level > 3) level = 3;
        machineSprite.sprite = machineSprites[level - 1];
        ScaleMachine();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
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
                state = MachineStates.Completed;
                animationController.PlayAnimation("lv2", true);
                boxCollider2D.enabled = true;
                PlayAudio(completeSfx);
                activeClock.HideClock();
            }
        }
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            state = MachineStates.Working;
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0.5f, 0), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            animationController.PlayAnimation("lv1", true, 8 / workDuration);
            PlayAudio(processSfx);
            boxCollider2D.enabled = false;
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
                    animationController.PlayAnimation("idle", true);
                    activeClock.HideClock();
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
        if (!result && state == MachineStates.Completed)
        {
            plates[0].Pulse();
        }
        clickTimeStamp = Time.time;
        if (result) boxCollider2D.enabled = false;
        return result;
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
        ScaleMachine();
        ServeFood();
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
    public void OnSelectUp(LeanFinger finger)
    {
    }

    public override void BackToWorkingStatus()
    {

    }

    public override void ShowUpgradeProcessAgain()
    {
        machineSprite.sprite = machineSprites[machineLevel - 2];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel));
    }

    public override void ShowUpgradeProcess()
    {
        machineSprite.sprite = machineSprites[machineLevel - 1];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(machineLevel + 1));
    }

    public override void SetupSkin()
    {
        machineSprite.sprite = machineSprites[machineLevel - 1];
    }
}


