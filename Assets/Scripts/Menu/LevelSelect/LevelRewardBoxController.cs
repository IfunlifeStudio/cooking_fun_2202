using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelRewardBoxController : MonoBehaviour
{
    [SerializeField] int rewardMilestone;
    [SerializeField] SkeletonGraphic rewardBoxSpine;
    [SerializeField] Slider progressSlider;
    [SerializeField] TextMeshProUGUI progressTxt;
    [SerializeField] LevelRewardProgressController progressController;
    public int boxStatus = 0;
    // Start is called before the first frame update
    public void SetupData(int stage, int milestones, int boxStatus)
    {
        this.boxStatus = boxStatus;
        if (boxStatus == 0)// dont claim && not enough stage
            rewardBoxSpine.AnimationState.SetAnimation(1, "idle", true);
        else if (boxStatus == 1) //has claimed
            rewardBoxSpine.AnimationState.SetAnimation(1, "open", true);
        else if (boxStatus == -1) //can claim
            rewardBoxSpine.AnimationState.SetAnimation(1, "idle_click", true);
        string progress = stage + "/" + milestones;
        progressTxt.text = progress;
        progressSlider.value = Mathf.Min(stage / milestones, 1);
        Debug.Log("stage "+ stage+ " milestones "+ milestones);
    }
    public void OnClickRewardBox()
    {
        if (boxStatus == 1) return;
        if (boxStatus == 0) { progressController.ShowRewardInfo(rewardMilestone); }
        else if (boxStatus == -1)
        {
            boxStatus = 1;
            int currentChapter = DataController.Instance.currentChapter;
            rewardBoxSpine.AnimationState.SetAnimation(1, "click_open", false);
            FindObjectOfType<DetailPanelController>().OnHide();
            switch (rewardMilestone)
            {
                case 1:
                    FindObjectOfType<RewardPanelController>().Init(0, 0, 0, new int[1] { 100200 }, new int[1] { 1 });
                    break;
                case 2:
                    FindObjectOfType<RewardPanelController>().Init(0, 0, 10, new int[1] { 100100 }, new int[1] { 1 });
                    break;
                case 3:
                    FindObjectOfType<RewardPanelController>().Init(0, 0, 15, new int[1] { 100300 }, new int[1] { 1 });
                    break;
                case 4:
                    FindObjectOfType<RewardPanelController>().Init(0, 0, 20, new int[1] { 100200 }, new int[1] { 2 });
                    break;
                case 5:
                    if (currentChapter <= 4)
                        FindObjectOfType<RewardPanelController>().Init(0, 40, 0, new int[2] { 100200, currentChapter + 3 }, new int[2] { 2, 1 });
                    else
                        FindObjectOfType<RewardPanelController>().Init(0, 40, 0, new int[1] { 100200 }, new int[1] { 2 });
                    break;
            }
            string sourceStr = currentChapter + "_" + rewardMilestone;
            APIController.Instance.LogEventOpenRewardMilestone(sourceStr);
            DataController.Instance.GetGameData().AddLevelRewardProgress(currentChapter, rewardMilestone);
        }


    }
}
