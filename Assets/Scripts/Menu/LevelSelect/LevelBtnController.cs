using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
using Spine.Unity;
public class LevelBtnController : MonoBehaviour
{
    public int chapter, index = 0;
    private LevelSelectorController levelSelector;
    [SerializeField] private Sprite[] cakeSprites, starBackgrounds;
    [SerializeField] private Image cakeBackground;
    [SerializeField] private GameObject focusFrame;
    public GameObject key;
    [SerializeField] private SkeletonGraphic keySkeletonAnimation;
    [SerializeField] private TextMeshProUGUI levelIndex;
    [SerializeField] private Image bgStar;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject extraJobslevel;
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
        levelSelector = FindObjectOfType<LevelSelectorController>();
        var levelData = LevelDataController.Instance.GetLevelData(chapter, _index);
        int highestLevel = Mathf.Min(DataController.Instance.GetTotalLevelsPerRestaurant(chapter), DataController.Instance.GetHighestLevel(chapter));
        int nextLevel = DataController.Instance.GetNextLevel(chapter);
        if (highestLevel < nextLevel) highestLevel = nextLevel;
        else if (highestLevel >= nextLevel && highestLevel > 1) highestLevel = DataController.Instance.GetTotalLevelsPerRestaurant(chapter) + 1;
        if (_index == highestLevel)
        {
            cakeBackground.sprite = cakeSprites[1];
            bgStar.gameObject.SetActive(true);
            bgStar.sprite = starBackgrounds[1];
            canInteract = true;
        }
        else if (_index > highestLevel)
        {
            cakeBackground.sprite = cakeSprites[2];
            bgStar.gameObject.SetActive(true);
            bgStar.sprite = starBackgrounds[0];
            canInteract = false;
        }
        else
        if (_index < highestLevel)
        {
            cakeBackground.sprite = cakeSprites[0];
            bgStar.gameObject.SetActive(false);

            canInteract = true;
        }
        var level = DataController.Instance.GetLevelState(chapter, _index);
        if (DataController.Instance.ExtraJobData.IsExtraJob(chapter, _index) && level == 3)
            extraJobslevel.SetActive(true);
        else
        {
            for (int i = 0; i < stars.Length; i++)
                stars[i].SetActive(i == level - 1);//active star base on current level state
        }
        yield return new WaitForSeconds(0.1f);
        key.SetActive(canInteract && !levelData.IsKeyGranted());//enable and disable key
        if (index == levelSelector.GetFocusLevelIndex())
        {
            if (key.activeInHierarchy)
                keySkeletonAnimation.AnimationState.SetAnimation(0, "shake", true);
            LevelDataController.Instance.LoadLevel(chapter, index);
            levelSelector.OnSelectLevel(this);
            focusFrame.SetActive(true);
            levelSelector.ShowDetailLevel(chapter, index);
        };
    }
    public void OnSelectLevel()
    {

        if (canInteract)
        {
            System.Action<ITween<Vector3>> updateLevelBtnScale = (t) =>
            {
                if (transform != null)
                    transform.localScale = t.CurrentValue;
            };
            TweenFactory.Tween("levelbtn" + index + Time.time, Vector3.one, new Vector3(1.25f, 1.25f, 1.25f), 0.1f, TweenScaleFunctions.QuinticEaseIn, updateLevelBtnScale).ContinueWith(new Vector3Tween().Setup(new Vector3(1.25f, 1.25f, 1.25f), Vector3.one, 0.25f, TweenScaleFunctions.QuinticEaseOut, updateLevelBtnScale));
            if (key.activeInHierarchy)
                keySkeletonAnimation.AnimationState.SetAnimation(0, "shake", true);
            LevelDataController.Instance.LoadLevel(chapter, index);
            levelSelector.OnSelectLevel(this);
            focusFrame.SetActive(true);
            levelSelector.ShowDetailLevel(chapter, index);
        }
    }
    public void OnLostFocus()
    {
        if (key.activeInHierarchy)
            keySkeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        focusFrame.SetActive(false);
    }
}
