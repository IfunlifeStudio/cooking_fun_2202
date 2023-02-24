using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
using Spine.Unity;
public class EventLevelBtnController : MonoBehaviour
{
    public int chapter, index = 0;
    private EventLevelSelectorController levelSelector;
    [SerializeField] private Sprite[] cakeSprites;
    [SerializeField] private Image cakeBackground;
    [SerializeField] private GameObject focusFrame;
    public GameObject key;
    [SerializeField] private TextMeshProUGUI levelIndex;
    private bool canInteract = false;
    public void Init(int _chapter, int _index)
    {
        StartCoroutine(DelayInit(_chapter, _index));
    }
    private IEnumerator DelayInit(int _chapter, int _index)
    {
        gameObject.SetActive(true);
        index = _index;
        chapter = _chapter;
        levelIndex.text = index.ToString();
        var selectors = FindObjectsOfType<EventLevelSelectorController>();
        foreach (var selector in selectors)
            if (selector.eventResId == _chapter) levelSelector = selector;
        var levelData = LevelDataController.Instance.GetEventLevelData(chapter, _index);
        int highestLevel = Mathf.Min(EventDataController.Instance.GetTotalLevelsPerRestaurant(chapter), EventDataController.Instance.GetHighestLevel(chapter));
        int nextLevel = EventDataController.Instance.GetNextLevel(chapter);
        if (highestLevel < nextLevel) highestLevel = nextLevel;
        else if (highestLevel >= nextLevel && highestLevel > 1) highestLevel = EventDataController.Instance.GetTotalLevelsPerRestaurant(chapter) + 1;
        if (_index == highestLevel)
        {
            cakeBackground.sprite = cakeSprites[1];
            canInteract = true;
        }
        else if (_index > highestLevel)
        {
            cakeBackground.sprite = cakeSprites[2];
            canInteract = false;
        }
        else
        if (_index < highestLevel)
        {
            cakeBackground.sprite = cakeSprites[0];
            canInteract = true;
        }
        yield return new WaitForSeconds(0.1f);
        key.SetActive(EventDataController.Instance.GetLevelState(levelData.chapter, levelData.id) > 0);//enable and disable key
        if (index == levelSelector.GetFocusLevelIndex()) OnSelectLevel();
    }
    public void OnSelectLevel()
    {
        System.Action<ITween<Vector3>> updateLevelBtnScale = (t) =>
                {
                    if (transform != null)
                        transform.localScale = t.CurrentValue;
                };
        TweenFactory.Tween("levelbtn" + index + Time.time, Vector3.one, new Vector3(1.25f, 1.25f, 1.25f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updateLevelBtnScale).ContinueWith(new Vector3Tween().Setup(new Vector3(1.25f, 1.25f, 1.25f), Vector3.one, 0.25f, TweenScaleFunctions.QuinticEaseOut, updateLevelBtnScale));
        if (canInteract)
        {
            LevelDataController.Instance.currentLevel = LevelDataController.Instance.GetEventLevelData(chapter, index);
            levelSelector.OnSelectLevel(this);
            focusFrame.SetActive(true);
        }
    }
    public void OnLostFocus()
    {
        focusFrame.SetActive(false);
    }
}
