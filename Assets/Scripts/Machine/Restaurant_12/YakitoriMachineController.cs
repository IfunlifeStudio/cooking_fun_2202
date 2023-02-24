using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YakitoriMachineController : Machine
{
    [SerializeField] private Sprite[] machineSprites;
    [SerializeField] private SpriteRenderer machineSprite;
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private MachineAnimationController[] animationControllers;
    [SerializeField] private GameObject instantCookEffect, upgradeEffect;
    [SerializeField] private bool isEvent = false;
    private Vector3 originScale;
    private MachineClockController activeClock;
    private BoxCollider2D boxCollider2D;
    int machineLevel;
    private void Start()
    {
        originScale = transform.localScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        workDuration = isEvent ? EventDataController.Instance.GetMachineWorkTime(id) : DataController.Instance.GetMachineWorkTime(id);
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        machineLevel = isEvent ? EventDataController.Instance.GetMachineLevel(id) : DataController.Instance.GetMachineLevel(id);
        machineSprite.sprite = machineSprites[machineLevel - 1];
        int foodLevel = isEvent ? EventDataController.Instance.GetIngredientLevel(foodId) : DataController.Instance.GetIngredientLevel(foodId);
        for (int i = 0; i < machineLevel; i++)
        {
            animationControllers[i].gameObject.SetActive(true);
            animationControllers[i].SetMachineSkin(foodLevel.ToString());
        }
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
            PauseAllAnimation();
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (timeRemain <= 0)
            {
                state = MachineStates.Completed;
                PlayAnimation("lv2");
                boxCollider2D.enabled = true;
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
        else if (state == MachineStates.Completed /*|| LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook)*/)//check no-burn item
        {
            activeClock.HideClock();
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].gameObject.activeInHierarchy) continue;
                plates[i].StoreCookedFood(foodId);
            }
            state = MachineStates.Standby;
            PlayAnimation("idle");
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
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position, instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            PlayAnimation("lv1");
            PlayAudio(processSfx);
            boxCollider2D.enabled = false;
            return true;
        }
        return false;
    }
    public override bool ServeFood()
    {
        return false;
    }
    public void PlayAnimation(string nameAnim)
    {
        for (int i = 0; i < machineLevel; i++)
        {
            animationControllers[i].PlayAnimation(nameAnim, true);
        }
    }
    public void PauseAllAnimation()
    {
        for (int i = 0; i < machineLevel; i++)
        {
            animationControllers[i].Pause();
        }
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
        yield return new WaitForSeconds(5f);
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
