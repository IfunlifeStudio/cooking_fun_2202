using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    public int countMachine = 0;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, antiOverCookEffect, upgradeEffect;
    public SpriteRenderer machineBody;
    public SpriteRenderer locked;
    [SerializeField] private Sprite[] machineBodiesSprites;
    private int idFoodActive = 1, machineLevel;
    public Food activeFood = null;
    [HideInInspector] public bool isUnlock = false;
    public BoxCollider2D boxCollider2D;
    [SerializeField] private AudioClip foodServeSfx;
    private Vector3 originScale;
    private IEnumerator Start()
    {
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        machineLevel = DataController.Instance.GetMachineLevel(id);
        originScale = transform.localScale;
        state = MachineStates.Standby;
        isPlaying = false;
        yield return null;
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

            for (int i = 0; i < activeFood?.transform.childCount; i++)
            {
                activeFood.transform.GetChild(i).GetComponent<IngredientSpritePizzaController>().IsCooking();
            }
            if (timeRemain <= 0)
            {
                state = MachineStates.Completed;
                timeRemain = burnTime = 15;
                if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
                    timeRemain = burnTime = 23;
                else if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
                    timeRemain = burnTime = 12;
                PlayAudio(completeSfx);
                for (int i = 0; i < activeFood?.transform.childCount; i++)
                {
                    activeFood.transform.GetChild(i).GetComponent<IngredientSpritePizzaController>().IsCompleted();
                }
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                    activeClock?.HideClock();
            }
        }
        else if (state == MachineStates.Completed)//check no-burn item
        {

            if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && activeFood != null)
            {
                if (CustomerFactory.Instance.CheckFood(idFoodActive))
                {
                    bool foodHasDispose = activeFood.OnClick();
                    if (foodHasDispose)
                    {
                        state = MachineStates.Standby;
                        AudioController.Instance.PlaySfx(foodServeSfx);
                        transform.GetChild(0).gameObject.SetActive(false);//ẩn thức ăn
                        antiOverCookEffect.SetActive(false);
                        activeClock?.HideClock();
                        machineBody.gameObject.SetActive(false);
                        activeFood = null;
                        GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                        Destroy(autoEffect, 2);
                        return;
                    }
                }


            }
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook)) return;
            boxCollider2D.isTrigger = true;
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / burnTime, false);
            if (timeRemain <= 0)
            {
                state = MachineStates.Burned;
                animationController.PlayAnimation("lv2", true);
                PlayAudio(completeSfx);
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position);
                else
                    MessageManager.Instance.SendMessage(
                        new Message(CookingMessageType.OnFoodBurned,
                        new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position + new Vector3(0, 1, 0), this }));
                FindObjectOfType<GameController>().OnFoodBurned(idFoodActive);

            }
        }
        else if (state == MachineStates.Burned)
        {
            activeClock.HideClock();
            for (int i = 0; i < activeFood.transform.childCount; i++)
            {
                activeFood.transform.GetChild(i).GetComponent<IngredientSpritePizzaController>().IsBurned();
            }

        }

    }
    public bool IsMachineReady()
    {
        return isUnlock && activeFood == null;
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                antiOverCookEffect.SetActive(true);
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
    public virtual bool StoreCookedFood(int id)
    {
        if (activeFood == null)
        {

            isPlaying = true;
            machineBody.gameObject.SetActive(true);
            activeFood = FoodFactory.Instance.SpawnFoodById(id, transform);
            activeFood.GetComponent<FoodAnimatorController>().Play("FoodAppear");
            transform.GetChild(0).gameObject.SetActive(true);//ẩn thức ăn
            activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1f));
            //AudioController.Instance.PlaySfx(addIngredientSfx);
            idFoodActive = id;
            FillIngredent();
            return true;
        }
        return false;
    }
    public bool AddIngredient(int id)
    {
        bool result = false;

        if (activeFood != null)
        {
            Food tmp = activeFood.MergeFood(id);
            if (tmp != null)
            {
                activeFood = tmp;
                tmp.GetComponent<FoodAnimatorController>().Play("FoodAppear");
                //AudioController.Instance.PlaySfx(addIngredientSfx);
                result = true;
            }
        }
        return result;
    }
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
        ServeFood();
    }

    public override bool ServeFood()
    {
        bool result = false;
        //k sử dụng các item       
        if (activeFood != null && state == MachineStates.Completed)
        {
            bool foodHasDispose = activeFood.OnClick();
            if (foodHasDispose)
            {
                state = MachineStates.Standby;
                AudioController.Instance.PlaySfx(foodServeSfx);
                transform.GetChild(0).gameObject.SetActive(false);//ẩn thức ăn
                antiOverCookEffect.SetActive(false);
                activeClock.HideClock();
                machineBody.gameObject.SetActive(false);
                activeFood = null;
                timeRemain = workDuration;
                result = true;
            }
        }
        //ném vào thùng rác
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            TossFood();
            result = true;
        }
        if (result)
        {
            boxCollider2D.isTrigger = true;
            animationController.PlayAnimation("idle", false);
        }
        clickTimeStamp = Time.time;
        return result;
    }
    public void CookingWarning()
    {
        AudioController.Instance.Vibrate();
    }
    public override void TossFood()
    {

        state = MachineStates.Standby;
        transform.GetChild(0).gameObject.SetActive(false);

        activeClock.HideClock();
        machineBody.gameObject.SetActive(false);
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
        activeFood.DisposeFood();
        activeFood = null;
        timeRemain = workDuration;
    }

    public override void BackToWorkingStatus()
    {

        state = MachineStates.Completed;
        animationController.PlayAnimation("lv1", true);
        for (int i = 0; i < activeFood?.transform.childCount; i++)
        {
            activeFood.transform.GetChild(i).GetComponent<IngredientSpritePizzaController>().IsSecondChange();
        }
        timeRemain = burnTime;
    }

    public override void SetupSkin()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
    }

    public override void ShowUpgradeProcessAgain()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 2];
        if (gameObject.activeInHierarchy && isUnlock)
            StartCoroutine(DelayUpgrade(machineLevel));
    }

    public override void ShowUpgradeProcess()
    {
        machineBody.sprite = machineBodiesSprites[machineLevel - 1];
        if (gameObject.activeInHierarchy && isUnlock)
            StartCoroutine(DelayUpgrade(machineLevel + 1));
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(4f);
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
