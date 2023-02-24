using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendBtnController : MonoBehaviour
{
    [SerializeField] Image frBtnImg;
    [SerializeField] Sprite[] frBtnSprites;
    [SerializeField] GameObject frNotifyGo;
    [SerializeField] GameObject loginFbPanel;
    [SerializeField] FbFriendsController FbFriendPanel;
    bool hasFrNoti = false;
    IEnumerator Start()
    {
        if (FB.IsLoggedIn)
        {
            frBtnImg.sprite = frBtnSprites[0];
            yield return GetGiftFirestoreData();
        }
        else
        {
            frBtnImg.sprite = frBtnSprites[1];
        }
    }

    public IEnumerator GetGiftFirestoreData()
    {
        DataController.Instance.frNeedHelpList.Clear();
        DataController.Instance.frGivenEnergyList.Clear();
        DataController.Instance.frGivenRubyList.Clear();
        DataController.Instance.frWaveList.Clear();
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        yield return FirebaseServiceController.Instance.GetFrListWithAction(userID, FireStoreColection.Help.ToString()).ContinueWith(task => { if (task.IsCompleted) DataController.Instance.frNeedHelpList = task.Result; });
        yield return FirebaseServiceController.Instance.GetFrListWithAction(userID, FireStoreColection.GiveEnergy.ToString()).ContinueWith(task => { if (task.IsCompleted) DataController.Instance.frGivenEnergyList = task.Result; });
        yield return FirebaseServiceController.Instance.GetFrListWithAction(userID, FireStoreColection.GiveRuby.ToString()).ContinueWith(task => { if (task.IsCompleted) DataController.Instance.frGivenRubyList = task.Result; });
        yield return FirebaseServiceController.Instance.GetFrListWithAction(userID, FireStoreColection.Wave.ToString()).ContinueWith(task => { if (task.IsCompleted) DataController.Instance.frWaveList = task.Result; });
        yield return new WaitForSeconds(0.4f);
        CheckGiftList();

    }
    private void CheckGiftList()
    {
        if (DataController.Instance.frNeedHelpList.Count > 0 || DataController.Instance.frGivenEnergyList.Count > 0 ||
            DataController.Instance.frGivenRubyList.Count > 0 || DataController.Instance.frWaveList.Count > 0)
        {
            frNotifyGo.SetActive(true);
            hasFrNoti = true;
        }
        else
        {
            hasFrNoti = false;
        }
    }
    public void OnClickFriendIcon()
    {
        if (FB.IsLoggedIn)
        {
            frNotifyGo.SetActive(false);
            var go = Instantiate(FbFriendPanel, transform.parent.parent);
            if (hasFrNoti)
            {
                hasFrNoti = false;
                go.DisplayFrGiftNotify();
                go.SetOnClosedCallBack(CheckGiftList);
            }
        }
        else
        {
            Instantiate(loginFbPanel, FindObjectOfType<MainMenuController>().cameraCanvas);
        }
        APIController.Instance.LogEventFBFriendAction("tap_icon");
    }
}
