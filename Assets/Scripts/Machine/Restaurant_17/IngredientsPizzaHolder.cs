using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsPizzaHolder : Machine
{
    [SerializeField] private PizzaPlateController[] plates;
    [SerializeField] private AudioClip clickAudio;
    private Vector3 originScale;
    [SerializeField] private GameObject upgradeEffect;
    private bool isAutoGiveBun;

    private void Start()
    {
        originScale = transform.localScale;
        HasCompletedUpgradeProcessing();
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
                yield return new WaitForSeconds(0.3f);
                plates[i].StoreCookedFood(foodId);
                yield return new WaitForSeconds(0.2f);
                state = MachineStates.Standby;
                break;
            }
        }

        yield return new WaitForSeconds(0.15f);
        isAutoGiveBun = false;
    }
    public void PizzaHolderTut()
    {
        plates[0].StoreCookedFood(foodId);
    }
    private void TweenGiveBun(GameObject plate, int i)
    {
       
        Food foodProp = FoodFactory.Instance.SpawnFoodById(foodId, transform);
        System.Action<ITween<Vector3>> updateFoodProp = (t) =>
        {
            foodProp.transform.position = t.CurrentValue;
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
        //TweenFactory.Tween("FoodPropScale" + Random.Range(0, 100f) + Time.time, foodProp.transform.localScale, targetPropScale, 0.4f, TweenScaleFunctions.QuinticEaseOut, updatePropScale);
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
        ScaleIngredient();   
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
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade());
    }

    public override void ShowUpgradeProcess()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade());
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeIngredientProcessing(id);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetIngredientUpgradeTime(id);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                DataController.Instance.RemoveIngredientUpgradeProcess(id);
                ShowUpgradeProcessAgain();
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedIngredientUpgrade(id);
                DataController.Instance.HasCompletedIngredientUpgradeInGame(id);
                ShowUpgradeProcess();
                return true;
            }
        }
        return false;
    }
    public void ScaleIngredient()
    {
        System.Action<ITween<Vector3>> updateIngredientScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Topping" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientScale));
    }
    IEnumerator DelayUpgrade()
    {
        yield return new WaitForSeconds(4f);
        ScaleIngredient();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
