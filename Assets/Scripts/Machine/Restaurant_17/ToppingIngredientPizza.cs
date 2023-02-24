using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingIngredientPizza : MonoBehaviour
{
    public int foodId = 10;
    [SerializeField] private PizzaPlateController[] plates;
    [SerializeField] private PizzaMachineController[] machine;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private GameObject upgradeEffect;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isEvent = false;
    private bool isIngredientUnlocked;
    private Vector3 originScale;
    int ingredientLevel = 1;
    // Start is called before the first frame update
    void Start()
    {

        originScale = transform.localScale;
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
    public void AddIngredient()
    {
        if (!isIngredientUnlocked) return;
        ScaleIngredient();
        for (int i = 0; i < plates.Length; i++)
        {
            if (!plates[i].gameObject.activeInHierarchy) continue;
            if (plates[i].AddIngredient(foodId))
                break;
        }

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
    public void OnSelect(LeanFinger finger)
    {

        AddIngredient();
    }
    public void OnSelectUp(LeanFinger finger)
    {

    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(4f);
        if (level > 3) level = 3;
        spriteRenderer.sprite = sprites[level - 1];
        ScaleIngredient();
        GameObject go = Instantiate(upgradeEffect, transform.position, Quaternion.identity);
        Destroy(go, 1);
    }
    private void ScaleIngredient()
    {
        System.Action<ITween<Vector3>> updateIngredientScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Topping" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientScale));
    }
}
