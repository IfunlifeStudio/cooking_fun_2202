using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;

public class ToppingIngredient : MonoBehaviour
{
    public int foodId = 10;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private AudioClip clickAudio, cookingWarningAudio;
    [SerializeField] private ToppingIngredient[] priorityIngredient;
    [SerializeField] GameObject upgradeEffect;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isEvent = false;
    private bool isIngredientUnlocked;
    private Vector3 originScale;
    private Vector3 originPosition;
    bool canAddIngredient;
    int ingredientLevel;
    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale;
        originPosition = transform.position;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = DataController.Instance.IsIngredientUnlocked(foodId, currentLevel);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (isIngredientUnlocked)
        {
            ingredientLevel = DataController.Instance.GetIngredientLevel(foodId);
            if (!HasCompletedUpgradeProcessing())
            {
                spriteRenderer.sprite = sprites[ingredientLevel - 1];
            }
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
                DataController.Instance.SaveData(false);
                spriteRenderer.sprite = sprites[ingredientLevel - 2];
                StartCoroutine(DelayUpgrade(ingredientLevel));
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedIngredientUpgrade(foodId);
                DataController.Instance.HasCompletedIngredientUpgradeInGame(foodId);
                DataController.Instance.SaveData(false);
                spriteRenderer.sprite = sprites[ingredientLevel - 1];
                StartCoroutine(DelayUpgrade(ingredientLevel + 1));
                return true;
            }
        }
        return false;
    }
    public void AddIngredient()
    {
        if (!isIngredientUnlocked) return;
        ScaleIngredient();
        canAddIngredient = true;
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].AddIngredient(foodId))
            {
                canAddIngredient = true;
                break;
            }
            else
                canAddIngredient = false;
        }
        if (priorityIngredient != null && priorityIngredient.Length > 0 && !canAddIngredient)
        {
            for (int i = 0; i < priorityIngredient.Length; i++)
                priorityIngredient[i].CookingWarning();
        }
        else if (!canAddIngredient && priorityIngredient.Length == 0)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].gameObject.activeInHierarchy) continue;
                if (plates[i].activeFood == null)
                {
                    plates[i].Pulse();
                    return;
                }
            }
        }
    }
    public void OnSelect(LeanFinger finger)
    {
        AddIngredient();
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
    public void CookingWarning()
    {
        System.Action<ITween<Vector3>> updateIngredientPosition = (t) =>
        {
            transform.localPosition = t.CurrentValue;
        };
        Vector3 targetPos = new Vector3(originPosition.x, originPosition.y + 0.32f, -0.5f);
        TweenFactory.Tween("ToppingPos" + Random.Range(0, 100f) + Time.time, originPosition, targetPos, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientPosition)
            .ContinueWith(new Vector3Tween().Setup(targetPos, originPosition, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientPosition));
        if (cookingWarningAudio != null)
            AudioController.Instance.PlaySfx(cookingWarningAudio);
        AudioController.Instance.Vibrate();

    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(4f);
        if (level > 3) level = 3;
        spriteRenderer.sprite = sprites[level - 1];
        ScaleIngredient();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}
