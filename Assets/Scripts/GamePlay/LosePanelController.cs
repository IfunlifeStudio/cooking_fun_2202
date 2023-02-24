using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;
public class LosePanelController : UIView
{
    [SerializeField] private SkeletonGraphic skeletonAnimation;
    private Spine.AnimationState spineAnimationState;
    [SerializeField] private GameObject endPanelFrame;
    [SerializeField] private Sprite[] objectiveSprites, conditionSprites, tickSprites;
    [SerializeField] private TextMeshProUGUI objectiveProgressText, cer_ComboTxt, cer_LikeTxt, cer_ServeTxt;
    [SerializeField] private Image objectiveIcon, conditionIcon;
    [SerializeField] protected EnergyPanelController energyPanelPrefab;
    [SerializeField] private Image conditionTick;
    bool isClick = false;
    public void Init(int objectiveType, string objectiveProgress, bool isTargetViolate, bool isConditionViolate, int conditionType, int cer_combo, int cer_like, int cer_serveFood)
    {
        UIController.Instance.PushUitoStack(this);
        objectiveIcon.sprite = objectiveSprites[objectiveType - 1];
        objectiveProgressText.text = objectiveProgress;
        if (conditionType < 1)
        {
            conditionIcon.gameObject.SetActive(false);
            conditionTick.gameObject.SetActive(false);
        }
        else
        {
            conditionIcon.sprite = conditionSprites[conditionType - 1];
            conditionIcon.gameObject.SetActive(true);
            conditionTick.gameObject.SetActive(true);
            if (isConditionViolate)
                conditionTick.sprite = tickSprites[0];
            else
                conditionTick.sprite = tickSprites[1];
        }
        cer_ComboTxt.text = cer_combo.ToString();
        cer_LikeTxt.text = cer_like.ToString();
        cer_ServeTxt.text = cer_serveFood.ToString();
        endPanelFrame.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine(DelayAnimation());

    }
    private IEnumerator DelayAnimation()
    {
        spineAnimationState = skeletonAnimation.AnimationState;
        spineAnimationState.SetAnimation(0, "start", false);
        yield return new WaitForSeconds(0.05f);
        endPanelFrame.SetActive(true);
        yield return new WaitForSeconds(1.95f);
        spineAnimationState.SetAnimation(0, "loop", true);
    }
    public void OnClickReplay()
    {

        if (DataController.Instance.IsUnlimitedEnergy())
        {
            string gameScene = DataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
            SceneController.Instance.LoadScene(gameScene);
        }
        else
        if (DataController.Instance.Energy > 0)
        {
            DataController.Instance.Energy--;
            string gameScene = DataController.Instance.GetGamePlayScene(LevelDataController.Instance.currentLevel.chapter);
            SceneController.Instance.LoadScene(gameScene);
        }
        else
            energyPanelPrefab.Spawn(transform.parent, null);
    }
    public void OnClickUpgrade()
    {
        if (AdsController.Instance.CanShowIntersAds(LevelDataController.Instance.currentLevel.chapter, LevelDataController.Instance.currentLevel.id, 1))
        {
            FindObjectOfType<GameController>().IsShowAds = true;
            AdsController.Instance.ShowInterstitial(() =>
            {
                if (isClick) return;
                isClick = true;
                SceneController.Instance.LoadScene("MainMenu");
            });
        }
        else
            SceneController.Instance.LoadScene("MainMenu");
        isClick = false;
    }
    public override void OnHide()
    {
        OnClickUpgrade();
    }
    IEnumerator DelayLoadMainmenu()
    {
        yield return new WaitForSeconds(0.4f);
        SceneController.Instance.LoadScene("MainMenu");
    }
}
