using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunMachine16 : Machine
{
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private bool isEvent = false;
    private SpriteRenderer ingredientSprite;
    private bool isIngredientUnlocked;
    private Vector3 originScale;

    private bool isAutoGiveBun;

    private void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = isEvent ? EventDataController.Instance.IsIngredientUnlocked(foodId, currentLevel) : DataController.Instance.IsIngredientUnlocked(foodId, currentLevel);
        ingredientSprite = GetComponentInChildren<SpriteRenderer>();
        if (isIngredientUnlocked)
        {
            int ingredientLevel = isEvent ? EventDataController.Instance.GetIngredientLevel(foodId) : DataController.Instance.GetIngredientLevel(foodId);
            ingredientSprite.sprite = sprites[ingredientLevel - 1];
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isAutoGiveBun)
            return;
        StartCoroutine(AutoGiveBun());
    }
    private IEnumerator AutoGiveBun()
    {
        isAutoGiveBun = true;
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].activeFood == null)
            {
                TweenGiveBun(plates[i].gameObject, i);
                state = MachineStates.Standby;
                yield return new WaitForSeconds(0.4f);
                plates[i].StoreCookedFood(foodId);
                break;
            }
        }
        yield return new WaitForSeconds(0.15f);
        isAutoGiveBun = false;
    }
    private void TweenGiveBun(GameObject plate, int i)
    {
        if (isPlaying && isIngredientUnlocked)
        {
            System.Action<ITween<Vector3>> updateMachineScale = (t) =>
            {
                transform.localScale = t.CurrentValue;
            };
            Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
            TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
                .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
            /*if (BasePanelController.Instance.canBack)
                AudioController.Instance.PlaySfx(clickAudio);*/
        }
        Food foodProp = FoodFactory.Instance.SpawnFoodById(foodId, transform);
        System.Action<ITween<Vector3>> updateFoodProp = (t) =>
        {
            foodProp.transform.position = t.CurrentValue;
        };
        System.Action<ITween<Vector3>> updatePropScale = (t) =>
        {
            foodProp.transform.localScale = t.CurrentValue;
        };
        System.Action<ITween<Vector3>> completedPropMovement = (t) =>
        {
            foodProp.gameObject.SetActive(false);
            FoodFactory.Instance.DisposeFood(foodProp);
        };
        Vector3 targetPropScale;
        switch (i)
        {
            case 0:
                targetPropScale = new Vector3(1.1f, 1.1f, 1.1f);
                break;
            case 1:
                targetPropScale = new Vector3(0.9f, 0.9f, 0.9f);
                break;
            case 2:
                targetPropScale = new Vector3(0.75f, 0.75f, 0.75f);
                break;
            default:
                targetPropScale = Vector3.one;
                break;
        }
        TweenFactory.Tween("FoodPropScale" + Random.Range(0, 100f) + Time.time, foodProp.transform.localScale, targetPropScale, 0.4f, TweenScaleFunctions.QuinticEaseOut, updatePropScale);
        Vector3 starttPos = new Vector3(foodProp.transform.position.x, foodProp.transform.position.y, -0.4f);
        Vector3 targetPos = new Vector3(plate.transform.position.x, plate.transform.position.y, -0.4f);
        TweenFactory.Tween("FoodProp" + Random.Range(0, 100f) + Time.time, starttPos, targetPos, 0.4f, TweenScaleFunctions.QuinticEaseOut, updateFoodProp, completedPropMovement);
    }
    public override bool FillIngredent()
    {
        return false;
    }

    public override bool ServeFood()
    {
        bool result = false;
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].StoreCookedFood(foodId))
            {
                state = MachineStates.Standby;
                result = true;
                break;
            }
        }
        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
    }
    public void OnSelect(LeanFinger finger)
    {

        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
        
        AudioController.Instance.PlaySfx(clickAudio);

    }
    public void OnSelectUp(LeanFinger finger)
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
