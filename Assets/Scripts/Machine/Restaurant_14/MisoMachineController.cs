using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;

public class MisoMachineController : Machine
{
    [SerializeField] private Sprite[] machineSprites;
    [SerializeField] private SpriteRenderer machineSprite;
    [SerializeField] private Transform clockPos;
    [SerializeField] private MachineClockController clockPrefab;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private GameObject instantCookEffect;
    [SerializeField] private bool isEvent = false;
    private Vector3 originScale;
    private MachineClockController activeClock;
    private BoxCollider2D boxCollider2D;
    private void Start()
    {
        originScale = transform.localScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        workDuration = DataController.Instance.GetMachineWorkTime(id);
        activeClock = clockPrefab.Spawn(clockPos.position - new Vector3(0, 0, 1));
        int machineLevel = DataController.Instance.GetMachineLevel(id);
        machineSprite.sprite = machineSprites[machineLevel - 1];
        for(int i = 0; i < plates.Length; i++)
            plates[i].gameObject.SetActive(i < machineLevel);
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
                animationController.PlayAnimation("lv3", true);
                boxCollider2D.enabled = true;
                PlayAudio(completeSfx);
                activeClock.HideClock();
            }
        }
        else if (state == MachineStates.Completed)//check no-burn item
        {
            timeRemain -= Time.deltaTime;//simulate burn product
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
            state = MachineStates.Working;
            timeRemain = workDuration;
            if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            {
                timeRemain = workDuration = 0.06f;
                GameObject instantEffect = Instantiate(instantCookEffect, transform.position, instantCookEffect.transform.rotation);
                Destroy(instantEffect, 2);
            }
            animationController.PlayAnimation("lv1", true);
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
    public override void TossFood()
    {

    }

    public override void BackToWorkingStatus()
    {
        
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
