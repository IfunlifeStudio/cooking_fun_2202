using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientKingCrabController : MonoBehaviour
{
    public int id;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private CrabMachineLevelController[] machines;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private bool isEvent = false;
    [SerializeField]
    private int index = 0;
    private Vector3 originScale;
    private bool isIngredientUnlocked;
    private SpriteRenderer spriteRenderer;
    int ingredientLevel = 1;
    private void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = DataController.Instance.IsIngredientUnlocked(id, currentLevel);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (isIngredientUnlocked)
        {
            ingredientLevel = DataController.Instance.GetIngredientLevel(id);
            if (!HasCompletedUpgradeProcessing())
                spriteRenderer.sprite = sprites[ingredientLevel - 1];
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
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
                spriteRenderer.sprite = sprites[ingredientLevel - 2];
                StartCoroutine(DelayUpgrade(ingredientLevel));
                return true;
            }
            else if (upgradeProcess.timeStamp != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - upgradeProcess.timeStamp > neededUpgradeTime))
            {
                DataController.Instance.CompletedIngredientUpgrade(id);
                DataController.Instance.HasCompletedIngredientUpgradeInGame(id);
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
    public void AddIngredient()
    {
        if (!isIngredientUnlocked) return;
        ScaleIngredient();
        AudioController.Instance.PlaySfx(clickAudio);
        for (int i = 0; i < machines.Length; i++)
        {
            if (machines[i].isUnlock)
            {
                if (machines[i].FillIngredent()) break;
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
        TweenFactory.Tween("Holder" + Random.Range(0, 100f) + Time.time, originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientScale));
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
