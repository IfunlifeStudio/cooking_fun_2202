using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplePieMachineController : Machine
{
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    private MachineClockController activeClock;
    [SerializeField] private MachineAnimationController animApplePie;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField]
    private GameObject Cake;
    [SerializeField]
    private Sprite[] rawCakeSprite;
    [SerializeField]
    private Sprite[] cookedCakeSprite;
    [SerializeField] 
    private Sprite[] burnedCakeSprite;
    [SerializeField] private GameObject instantCookEffect, antiOverCookEffect;
    private Vector3 originScale;
   
    private bool isMachineUnlock;
    private Collider2D _collider2D;
    public int Count = -1;
    int ingredientLevel = -1;
    int machineCount;
    private IEnumerator Start()
    {
        originScale = transform.localScale;
        _collider2D = GetComponent<Collider2D>();
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isMachineUnlock = DataController.Instance.IsMachineUnlocked(id, currentLevel);
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        animationController = GetComponent<MachineAnimationController>();
        state = MachineStates.Standby;
        activeClock = clockPrefab.Spawn(clockPos.position- new Vector3(0,0,1f));
        machineCount = DataController.Instance.GetMachineCount(id);                                    
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        ingredientLevel = DataController.Instance.GetIngredientLevel(foodId);
        Cake.GetComponent<SpriteRenderer>().sprite = rawCakeSprite[ingredientLevel - 1];
        Cake.SetActive(false);
        animApplePie.SetMachineSkin(machineLevel.ToString());
        //animApplePie.PlayAnimation("off",false);
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
            //animApplePie.Pause();
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
                Cake.GetComponent<SpriteRenderer>().sprite = cookedCakeSprite[ingredientLevel - 1];
                if (FindObjectOfType<ApplePieMachineLevel>().machineIndex==Count)
                {
                    animApplePie.PlayAnimation("off", false);
                    FindObjectOfType<ApplePieMachineLevel>().machineIndex = -1;
                }    
                    
                timeRemain = burnTime = 15;
                if (PlayerClassifyController.Instance.GetPlayerTier() == -1)
                    timeRemain = burnTime = 23;
                else if (PlayerClassifyController.Instance.GetPlayerTier() == 1)
                    timeRemain = burnTime = 12;
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
                Cake.GetComponent<SpriteRenderer>().sprite = burnedCakeSprite[ingredientLevel - 1];
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                    FindObjectOfType<GameController>()?.SendMessageBurnedViolate(transform.position);
                else
                    MessageManager.Instance.SendMessage(
                        new Message(CookingMessageType.OnFoodBurned,
                        new object[] { LevelDataController.Instance.currentLevel.ConditionType(), transform.position + new Vector3(0, 1, 0), this }));
                FindObjectOfType<GameController>().OnFoodBurned(foodId);
                activeClock.HideClock();        
                PlayAudio(burnSfx);
            }
        }
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
                GameObject instantEffect = Instantiate(instantCookEffect, Cake.transform.position + new Vector3(-0.6f, 0, -1f), instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            Cake.SetActive(true);
            state = MachineStates.Working;
            PlayAudio(processSfx);
            animApplePie.PlayAnimation("on", true);
            FindObjectOfType<ApplePieMachineLevel>().machineIndex = Count;
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
                    activeClock.HideClock();
                    antiOverCookEffect.SetActive(false);
                    result = true;
                     Cake.GetComponent<SpriteRenderer>().sprite = rawCakeSprite[ingredientLevel - 1];
                     Cake.SetActive(false);
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
            animApplePie.PlayAnimation("off", false);
            _collider2D.enabled = false;
           
        }
            
        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
        state = MachineStates.Standby;
        transform.GetChild(0).gameObject.SetActive(false);
        activeClock.HideClock(); 
        antiOverCookEffect.SetActive(false);
        Cake.GetComponent<SpriteRenderer>().sprite = rawCakeSprite[ingredientLevel - 1];
        Cake.SetActive(false);
        FindObjectOfType<TrashBinApplePie>().TossTrash(foodId);
    }
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
        if (isMachineUnlock)
        {
            
            ServeFood();
        }
    }
    public void OnSelectUp(LeanFinger finger)
    {
    }

    public override void BackToWorkingStatus()
    {
        _collider2D.enabled = true;
        state = MachineStates.Completed;
        Cake.GetComponent<SpriteRenderer>().sprite = cookedCakeSprite[ingredientLevel - 1];
        animApplePie.PlayAnimation("off", true);
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
