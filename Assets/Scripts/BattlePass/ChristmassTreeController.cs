using DigitalRuby.Tween;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChristmassTreeController : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic christmassTree;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform topOfTree;
    public void InitTree(int level)
    {
        int index = (level / 5) + 1;
        string levelStr = "lvl" + index;
        SetTreeLevel(levelStr);
    }
    private void Update()
    {
    }
    public void SetTreeLevel(string levelStr)
    {
        christmassTree.AnimationState.SetAnimation(0, levelStr, true);
    }
    public void OnLevelUp(int level)
    {
        SpawnEffect();
        System.Action<ITween<Vector3>> updateFoodProp = (t) =>
        {
            christmassTree.transform.localScale = t.CurrentValue;
        };
        TweenFactory.Tween("FillProgress" + Random.Range(0, 100f) + Time.time, new Vector3(0.9f, 0.9f, 1), new Vector3(1, 1, 1), 0.5f, TweenScaleFunctions.Linear, updateFoodProp, null);
    }
    private void SpawnEffect()
    {
        GameObject go = Instantiate(effectPrefab, topOfTree);
        Destroy(go, 0.5f);
    }
}
