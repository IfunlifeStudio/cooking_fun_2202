using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
public class EventLevelBoxController : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic boxSkeleton;
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private GameObject openBoxEffectPrefab;
    [SerializeField] private EventRewardIntroController eventIntroPrefab;
    private EventLevelSelectorController levelSelector;
    private int chapter;
    private int totalLevels, totalKeys;
    private bool hasClickOpenBox;
    // Start is called before the first frame update
    void Start()
    {
        var selectors = FindObjectsOfType<EventLevelSelectorController>();
        chapter = EventDataController.Instance.currentChapter;//total key need to unlock
        foreach (var selector in selectors)
            if (selector.eventResId == chapter) levelSelector = selector;
        int[] levelProgressUnlock = EventDataController.Instance.GetLevelProgressUnlock(chapter);
        int lastestLevel = EventDataController.Instance.GetHighestLevel(chapter);
        int index = 0;
        do
        {
            totalLevels = levelProgressUnlock[index];
            index++;
        }
        while (lastestLevel > totalLevels && index < levelProgressUnlock.Length);
        var lastestPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        totalKeys = 0;
        LevelData levelData = null;
        for (int i = 1; i < totalLevels + 1; i++)
        {
            levelData = LevelDataController.Instance.GetEventLevelData(chapter, i);
            if (EventDataController.Instance.GetLevelState(chapter, i) >= levelData.key)//check if key is granted
                totalKeys++;
        }
        if (!EventDataController.Instance.HasRestaurantRewarded(chapter, (lastestLevel - 1) / 5))
            totalLevels = levelProgressUnlock[index - 1];
        else
            totalLevels = levelProgressUnlock[Mathf.Min(index, levelProgressUnlock.Length - 1)];
        keyCountText.text = totalKeys + "/" + totalLevels;
        LevelDataController.Instance.lastestPassedLevel = null;
        LevelDataController.Instance.collectedGold = 0;
    }
    public void Spawn(Vector3 localSpawnPos, Transform parent)
    {
        GameObject box = Instantiate(gameObject, parent);
        box.transform.localPosition = localSpawnPos;
    }
    public void OnClickBox()
    {
        if (totalKeys < totalLevels)//open warning pop up
        {
            Transform parent = GameObject.Find("CanvasOverlay").transform;
            int lastestLevel = EventDataController.Instance.GetHighestLevel(chapter);
            switch ((lastestLevel / 5 + 1) * 5)
            {
                case 5:
                    eventIntroPrefab.Spawn(0, 0, 60, 3, parent);
                    break;
                case 10:
                    eventIntroPrefab.Spawn(0, 50, 120, 7, parent);
                    break;
                case 15:
                    eventIntroPrefab.Spawn(0, 50, 120, 14, parent);
                    break;
            }
        }
        else
        {
            if (!hasClickOpenBox)
            {
                hasClickOpenBox = true;
                boxSkeleton.AnimationState.SetAnimation(0, "click_open", false);
                StartCoroutine(DelayOpenBox());//unlock new level after click box, instance new level btn container
            }
        }
    }
    private IEnumerator DelayOpenBox()
    {
        GameObject go = Instantiate(openBoxEffectPrefab, transform.position, Quaternion.identity);
        Destroy(go, 1);
        yield return new WaitForSeconds(0.8f);
        levelSelector.OnClickOpenLevelBox();
        Destroy(gameObject);
    }
}
