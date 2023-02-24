using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauChickenMachineController : Machine
{
    public int foodIDHau = 100;
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private MachineAnimationController animationControllerHau;
    [SerializeField] private MachineAnimationController animationControllerChicken;
    [SerializeField] protected AudioClip foodServeSfx;
    [SerializeField] private GameObject instantCookEffect, autoServeEffect, antiOverCookEffect, upgradeEffect;
    [HideInInspector] public int idCheck = 1;
    //public SpriteRenderer machineBody;
    public SpriteRenderer locked;
    public Food activeFood = null;
    public bool isUnlock = false;
    public BoxCollider2D boxCollider2D;
    private IEnumerator Start()
    {
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        int ingredientLevelHau = DataController.Instance.GetIngredientLevel(foodIDHau);
        int ingredientLevelChicken = DataController.Instance.GetIngredientLevel(foodId);
        animationControllerChicken.SetMachineSkin(ingredientLevelChicken.ToString());
        animationControllerHau.SetMachineSkin(ingredientLevelHau.ToString());
        state = MachineStates.Standby;
        isPlaying = false;
        yield return null;
    }
    public bool IsUnlock()
    {
        return isUnlock && idCheck == 1;
    }

    private void PlayAnim(string anim, bool loop)
    {
        if (idCheck == 18900)
            animationControllerChicken.PlayAnimation(anim, loop);
        else if (idCheck == 18800) animationControllerHau.PlayAnimation(anim, loop);
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
            PlayAnim("idle", false);
            return;
        }
        if (state == MachineStates.Working)
        {
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / workDuration, true);
            if (timeRemain <= 0)
            {
                PlayAnim("lv1", true);
                state = MachineStates.Completed;
                activeClock.HideClock();
                timeRemain = burnTime = 15;
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                    activeClock?.HideClock();
            }
        }
        else if (state == MachineStates.Completed)//check no-burn item
        {

            if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && activeFood != null)
            {
                if (CustomerFactory.Instance.CheckFood(idCheck))
                {
                    bool foodHasDispose = activeFood.OnClick();
                    if (foodHasDispose)
                    {
                        state = MachineStates.Standby;
                        PlayAudio(foodServeSfx);
                        antiOverCookEffect.SetActive(false);
                        activeClock?.HideClock();
                        activeFood = null;
                        PlayAnim("idle", true);
                        idCheck = 1;
                        GameObject autoEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                        Destroy(autoEffect, 2);
                        return;
                    }
                }


            }

            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook)) return;
            timeRemain -= Time.deltaTime;
            activeClock.SetFillAmount(1 - timeRemain / burnTime, false);
            if (timeRemain <= 0)
            {
                state = MachineStates.Burned;
                PlayAnim("lv2", true);
                PlayAudio(completeSfx);
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position);
                else
                    MessageManager.Instance.SendMessage(
                        new Message(CookingMessageType.OnFoodBurned,
                        new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position + new Vector3(0, 1, 0), this }));
                FindObjectOfType<GameController>().OnFoodBurned(idCheck);
                if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && ServeFood())
                {
                    GameObject autoEffect = Instantiate(antiOverCookEffect, transform.position, antiOverCookEffect.transform.rotation);
                    Destroy(autoEffect, 2);
                    FillIngredent();
                }
            }

        }
        else if (state == MachineStates.Burned)
        {
            activeClock.HideClock();
        }
    }
    public override bool FillIngredent()
    {
        if (state == MachineStates.Standby)
        {
            activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
            isPlaying = true;
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Anti_OverCook))
                antiOverCookEffect.SetActive(true);
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position + new Vector3(0, 0, -1f), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            PlayAnim("doing", false);
            state = MachineStates.Working;
            PlayAudio(processSfx);
            return true;
        }
        return false;
    }

    public virtual bool StoreCookedFood(int id)
    {
        if (activeFood == null && idCheck == 1)
        {
            idCheck = id;
            activeFood = FoodFactory.Instance.SpawnFoodById(id, transform);
            //AudioController.Instance.PlaySfx(addIngredientSfx);
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
                PlayAudio(foodServeSfx);
                activeClock.HideClock();
                antiOverCookEffect.SetActive(false);
                activeFood = null;
                timeRemain = workDuration;
                PlayAnim("idle", true);
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

            idCheck = 1;
            activeFood = null;
        }
        clickTimeStamp = Time.time;
        return result;
    }

    public override void TossFood()
    {

        state = MachineStates.Standby;
        PlayAnim("idle", true);
        activeClock.HideClock();
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
        timeRemain = workDuration;
    }

    public override void BackToWorkingStatus()
    {
        state = MachineStates.Completed;
        PlayAnim("lv1", true);
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
