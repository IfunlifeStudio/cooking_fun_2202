using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
public class BunMachine : Machine
{
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private bool isEvent = false;
    [SerializeField] GameObject upgradeEffect;
    private SpriteRenderer ingredientSprite;
    private bool isIngredientUnlocked;
    private Vector3 originScale;
    private Vector3 originPosition;
    int ingredientLevel = 1;
    private void Start()
    {
        originScale = transform.localScale;
        originPosition = transform.position;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = isEvent ? EventDataController.Instance.IsIngredientUnlocked(foodId, currentLevel) : DataController.Instance.IsIngredientUnlocked(foodId, currentLevel);
        ingredientSprite = GetComponentInChildren<SpriteRenderer>();
        ingredientLevel = isEvent ? EventDataController.Instance.GetIngredientLevel(foodId) : DataController.Instance.GetIngredientLevel(foodId);
        if (isIngredientUnlocked)
        {
            if (!HasCompletedUpgradeProcessing())
                ingredientSprite.sprite = sprites[ingredientLevel - 1];
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }
    public bool HasCompletedUpgradeProcessing()
    {
        var upgradeProcess = DataController.Instance.GetGameData().GetUpgradeIngredientProcessing(foodId);
        if (upgradeProcess != null)
        {
            float neededUpgradeTime = DataController.Instance.GetIngredientUpgradeTime(foodId);
            if (upgradeProcess.hasCompleteUpgradeInMenu)
            {
                DataController.Instance.RemoveIngredientUpgradeProcess(foodId);
                ingredientSprite.sprite = sprites[ingredientLevel - 2];
                StartCoroutine(DelayUpgrade(ingredientLevel));
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedIngredientUpgrade(foodId);
                DataController.Instance.HasCompletedIngredientUpgradeInGame(foodId);
                ingredientSprite.sprite = sprites[ingredientLevel - 1];
                StartCoroutine(DelayUpgrade(ingredientLevel + 1));
                return true;
            }
        }
        return false;
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
        if (!result)
        {
            plates[0].Pulse();
        }
        clickTimeStamp = Time.time;
        return result;
    }
    public override void TossFood()
    {
    }
    public void OnSelect(LeanFinger finger)
    {
        if (!isPlaying) return;
        if (!isIngredientUnlocked) return;
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
                   {
                       transform.localScale = t.CurrentValue;
                   };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));
        AudioController.Instance.PlaySfx(clickAudio);
        ServeFood();
    }
    public override void BackToWorkingStatus()
    {

    }
    public void CookingWarning()
    {
        System.Action<ITween<Vector3>> updateIngredientPosition = (t) =>
        {
            transform.localPosition = t.CurrentValue;
        };
        //float ratio = (originPosition.y > 0) ? -1f : 1f;
        Vector3 targetPos = new Vector3(originPosition.x, originPosition.y + (0.35f), originPosition.z);
        TweenFactory.Tween("ToppingPos" + Random.Range(0, 100f) + Time.time, originPosition, targetPos, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientPosition)
            .ContinueWith(new Vector3Tween().Setup(targetPos, originPosition, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientPosition));
    }

    public override void SetupSkin()
    {
    }

    public override void ShowUpgradeProcessAgain()
    {
    }

    public override void ShowUpgradeProcess()
    {
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
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(5f);
        if (level > 3) level = 3;
        ingredientSprite.sprite = sprites[level - 1];
        ScaleIngredient();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
