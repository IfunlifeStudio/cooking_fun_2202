using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;

public class AsparagusIngredientPlate : MonoBehaviour
{
    public int foodId = 10;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private ServePlateController[] plates;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ToppingIngredient[] priorityIngredient;
    [SerializeField] private bool isEvent = false;
    [SerializeField] private GameObject upgradeEffect;
    private bool isIngredientUnlocked;
    private Vector3 originScale;
    private Vector3 originPosition;
    bool canAddIngredient;
    int ingredientLevel = 1;
    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = isEvent ? EventDataController.Instance.IsIngredientUnlocked(foodId, currentLevel) : DataController.Instance.IsIngredientUnlocked(foodId, currentLevel);
        if (isIngredientUnlocked)
        {
            ingredientLevel = isEvent ? EventDataController.Instance.GetIngredientLevel(foodId) : DataController.Instance.GetIngredientLevel(foodId);
            if (!HasCompletedUpgradeProcessing())
                spriteRenderer.sprite = sprites[ingredientLevel - 1];
        }
        else
            transform.gameObject.SetActive(false);
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
                spriteRenderer.sprite = sprites[ingredientLevel - 2];
                StartCoroutine(DelayUpgrade(ingredientLevel));
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedIngredientUpgrade(foodId);
                DataController.Instance.HasCompletedIngredientUpgradeInGame(foodId);
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
    public void OnSelect(LeanFinger finger)
    {
        AddIngredient();
    }
    public void OnSelectUp(LeanFinger finger)
    {
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(5f);
        if (level > 3) level = 3;
        spriteRenderer.sprite = sprites[level - 1];
        ScaleIngredient();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
}

