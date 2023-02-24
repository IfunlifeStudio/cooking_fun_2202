using Facebook.Unity;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProfileController : MonoBehaviour
{
    [SerializeField] MainMenuController menuController;
    [SerializeField] GameObject profileNotify;
    [SerializeField] CertificateCoverController certificateCover;
    [SerializeField] ProfilePanelController profilePanel;
    int coverIndex = 0;
    private void Start()
    {
        if (PlayerPrefs.GetInt("has_profile_notify", 0) != 0)
            profileNotify.SetActive(true);
        //var lastPassedLevel = LevelDataController.Instance.lastestPassedLevel;
        //if (lastPassedLevel != null)
        //{
        //    int totalStageInRes = DataController.Instance.GetTotalLevelsPerRestaurant(lastPassedLevel.chapter) * 3;
        //    int totalStageGranted = DataController.Instance.GetTotalStageGranted(lastPassedLevel.chapter);
        //    int boxindex = GetRewardBoxIndexCanClaim(totalStageGranted, totalStageInRes);
        //    if (boxindex != 0)
        //    {
        //        if (DataController.Instance.GetGameData().GetCertificateDataAtRes(lastPassedLevel.chapter).RewardRecord[boxindex - 1] == 0)
        //        {
        //            coverIndex = lastPassedLevel.chapter;
        //            profileNotify.SetActive(true);
        //        }
        //    }
        //}

    }
    //private int GetRewardBoxIndexCanClaim(int stageGranted, int total)
    //{
    //    if (stageGranted >= (total * 3 / 5))
    //        return 1;
    //    else if (stageGranted >= (total * 4 / 5))
    //        return 2;
    //    else if (stageGranted == total)
    //        return 3;
    //    return 0;

    //}
    public void ShowCertificateCover(int resID)
    {
        menuController.detailPanel.OnHide();
        coverIndex = resID;
        Instantiate(certificateCover, menuController.cameraCanvas).Init(resID, transform.position, OnHideCoverAndUnlockRes);
    }
    public void OnHideCoverAndUnlockRes()
    {
        TurnNotifyIconOn();
        menuController.UnlockResAndCollectReward();
    }
    public void ShowProfilePanel()
    {
        menuController.HideUI();
        var profile = Instantiate(profilePanel, menuController.cameraCanvas);
        profile.Init(DataController.Instance.GetGameData().profileDatas, () => { menuController.ShowUI(); });
        if (coverIndex == 0) coverIndex = PlayerPrefs.GetInt("has_profile_notify", 0);
        if (coverIndex != 0)
        {
            profile.DisplayCertiCoverNotify(coverIndex - 1);
            coverIndex = 0;
        }
        TurnNotifyIconOff();
    }
    public void TurnNotifyIconOn()
    {
        profileNotify.SetActive(true);
        PlayerPrefs.SetInt("has_profile_notify", coverIndex);
    }
    public void TurnNotifyIconOff()
    {
        profileNotify.SetActive(false);
        //PlayerPrefs.SetInt("has_profile_notify", 0);
    }
}
