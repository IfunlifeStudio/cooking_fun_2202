using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardProgressController : MonoBehaviour
{
    [SerializeField] LevelRewardBoxController[] LevelRewardBoxes;
    [SerializeField] LevelRewardInforPanelController inforPanel;
    [SerializeField] SimpleScrollSnap simpleScrollSnap;
    int startIndex = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        int currentChapter = DataController.Instance.currentChapter;
        var rewardProgress = DataController.Instance.GetGameData().GetLevelRewardProgress(currentChapter);
        var restaurantData = DataController.Instance.GetRestaurantById(currentChapter);
        int totalStageGranted = DataController.Instance.GetTotalStageGranted(currentChapter);
        int canClamRewardMilstone = -1;
        for (int i = 0; i < LevelRewardBoxes.Length; i++)
        {
            int boxStatus = 0;
            if (totalStageGranted >= restaurantData.levelRewardProgress[i])
            {
                startIndex = Mathf.Min(i + 1, 4);
                if (rewardProgress.rewardProgress[i] != 1)
                {
                    boxStatus = -1;
                    canClamRewardMilstone = i;
                }
                else if (rewardProgress.rewardProgress[i] == 1) boxStatus = 1;
            }
            LevelRewardBoxes[i].SetupData(totalStageGranted, restaurantData.levelRewardProgress[i], boxStatus);
        }
        if (totalStageGranted == restaurantData.levelRewardProgress[LevelRewardBoxes.Length - 1])
        {
            if (canClamRewardMilstone != -1)
                startIndex = canClamRewardMilstone;
        }
        simpleScrollSnap.startingPanel = startIndex;
        simpleScrollSnap.SnaptoPanel(startIndex);
        startIndex = 0;
    }
    public void ShowRewardInfo(int milestone)
    {
        inforPanel.SetUpPanel(milestone);
    }
    public void TurnOffRewardInfor()
    {
        //inforPanel.TurnPanelOff();
    }
}
