using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FbFriendsController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject loginBtn;
    [SerializeField] ScrollRect frPanel;
    List<string> frIdList = new List<string>();
    List<ProfileData> fbUserList = new List<ProfileData>();
    [SerializeField] FriendListPanelController friendListPanel;
    [SerializeField] FriendGiftPanelController friendGiftPanel;
    [SerializeField] FriendWavePanelController friendWavePanel;
    [SerializeField] GameObject notifyGo;
    ProfileData botData = new ProfileData(100, 100, 100, 100, 100, 1, 1, 395, 0, "Lovely Chef", "", "");
    Action onCloseCallback;
    // Start is called before the first frame update
    void Start()
    {
        UIController.Instance.PushUitoStack(this);
        bool hasLogin = FB.IsLoggedIn;
        animator.Play("Appear");
        loginBtn.SetActive(!hasLogin);
        if (hasLogin)
        {
            DatabaseController.Instance.SpawnLoading();
            botData.FBUserID = "9999";
            FB.API("/me/friends?fields=id,name,picture.width(128).height(128)", HttpMethod.GET, HandleData);
        }
    }
    private async void HandleData(IGraphResult result)
    {
        if (String.IsNullOrEmpty(result.Error) && !result.Cancelled)
        { //if there isn't an error
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendList = (List<object>)dictionary["data"];
            string name, url;
            foreach (var dic in friendList)
            {
                var obj = (Dictionary<string, object>)dic;
                name = (string)obj["name"];
                string id = (string)obj["id"];
                var picture = (Dictionary<string, object>)obj["picture"];
                var pictureData = picture["data"];
                var obj1 = (Dictionary<string, object>)pictureData;
                url = (string)obj1["url"];
                ProfileData fbUser = await FirebaseServiceController.Instance.GetFriendProfileData(id);
                if (fbUser != null)
                {
                    if (fbUser.AvatarUrl == "")
                        fbUser.AvatarUrl = url;
                    fbUser.FBUserID = id;
                    fbUserList.Add(fbUser);
                }
            }
        }
        else
        {
            UIController.Instance.ShowNotiFy(NotifyType.NoInternet);
            DatabaseController.Instance.DespawnLoadingOverlay();
        }
        fbUserList.Add(DataController.Instance.GetGameData().profileDatas);
        fbUserList = fbUserList.OrderByDescending(obj => obj.KeyGranted).ToList();
        fbUserList.Add(botData);
        OnClickFriendListTab();
        DatabaseController.Instance.DespawnLoadingOverlay();
    }
    public void OnClickGiftTab()
    {
        SetActiveTab(2);
        TurnGiftNotifyOff();
        friendGiftPanel.Init(fbUserList);
    }
    public void OnClickFriendListTab()
    {
        SetActiveTab(1);
        friendListPanel.Init(fbUserList);
    }
    public void OnClickWaveTab()
    {
        SetActiveTab(3);
        friendWavePanel.Init(fbUserList);
    }
    public void SetActiveTab(int index)
    {
        friendListPanel.gameObject.SetActive(index == 1);
        friendGiftPanel.gameObject.SetActive(index == 2);
        friendWavePanel.gameObject.SetActive(index == 3);
    }
    public void DisplayFrGiftNotify()
    {
        notifyGo.SetActive(true);
    }
    public void SetOnClosedCallBack(Action callback)
    {
        onCloseCallback = callback;
    }
    private void TurnGiftNotifyOff()
    {
        notifyGo.SetActive(false);
    }
    public void InviteFriend()
    {
        APIController.Instance.LogEventFBFriendAction("invite_friend");
        if (NativeShare.TargetExists("com.facebook.orca"))
            new NativeShare().SetUrl("https://cookinglove.page.link/invitefriend").SetText("Play with me").AddTarget("com.facebook.orca").Share();
        else
        {
            FB.AppRequest(
    "Hey!Come and play thid awsome game",
    null, null,
    null, null, null, null, null
);
        }
    }
    public void OnClickLogin()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            DatabaseController.Instance.LoginWithFaceBook();
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        if (onCloseCallback != null) onCloseCallback.Invoke();
        animator.Play("Disappear");
        Destroy(gameObject, 0.3f);
    }
}
