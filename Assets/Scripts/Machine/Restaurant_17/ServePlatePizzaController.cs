using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServePlatePizzaController : Machine
{
    private MachineClockController activeClock;
    [SerializeField] protected Transform clockPos;
    [SerializeField] protected MachineClockController clockPrefab;
    [SerializeField] protected ServePlateController[] plates;
    [SerializeField] protected GameObject instantCookEffect, antiOverCookEffect;
    [SerializeField] private SpriteRenderer machineSprite;
    [SerializeField] private Sprite[] machineSprites;
    private Vector3 originScale;
    private Collider2D _collider2D;
    public Food activeFood = null;
    protected virtual void Start()
    {

        originScale = transform.localScale;
        _collider2D = GetComponent<Collider2D>();
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        transform.GetChild(0).gameObject.SetActive(false);
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        machineSprite.sprite = machineSprites[machineLevel - 1];
        int foodLevel = DataController.Instance.GetIngredientLevel(foodId);
        animationController.SetMachineSkin(foodLevel.ToString());
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
                _collider2D.enabled = true;
                state = MachineStates.Completed;

                animationController.PlayAnimation("lv2", true);
                timeRemain = burnTime = 15;
                if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
                    timeRemain = burnTime = 23;
                else if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
                    timeRemain = burnTime = 12;
                AudioController.Instance.PlaySfx(completeSfx);
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
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position);
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
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0.4f, 0), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            transform.GetChild(0).gameObject.SetActive(true);
            animationController.PlayAnimation("lv1", true);
            AudioController.Instance.PlaySfx(processSfx);
            return true;
        }
        return false;
    }
    public override bool ServeFood()
    {
        bool result = false;
        //k sử dụng các item
        if (state == MachineStates.Completed)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                //máy chưa có thức ăn, bỏ qua
                if (!plates[i].gameObject.activeInHierarchy) continue;
                //đĩa đã active
                if (plates[i].AddIngredient(foodId))
                {
                    state = MachineStates.Standby;
                    transform.GetChild(0).gameObject.SetActive(false);//ẩn thức ăn
                    activeClock.HideClock();
                    antiOverCookEffect.SetActive(false);
                    result = true;
                    break;
                }
            }
        }

        //ném vào thùng rác
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            TossFood();
            result = true;
        }
        if (result)
            _collider2D.enabled = false;
        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
        state = MachineStates.Standby;
        transform.GetChild(0).gameObject.SetActive(false);
        activeClock.HideClock();
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
    }
    public void OnSelect(LeanFinger finger)
    {

        if (activeFood != null)
        {
           /* bool foodHasDispose = activeFood.OnClick();
            if (foodHasDispose)
            {
                boxCollider2D.enabled = false;
                AudioController.Instance.PlaySfx(foodServeSfx);
                activeFood = null;
            }*/
        }
    }

    public override void BackToWorkingStatus()
    {
        _collider2D.enabled = true;
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
