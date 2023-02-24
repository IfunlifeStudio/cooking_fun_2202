using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendListItemController : FriendItem
{
    [SerializeField] FriendProfileController frProfilePrefab;
    [SerializeField] Transform avatarTransform;
    [SerializeField] Image medal;
    [SerializeField] Sprite[] medalSprites;
    [SerializeField] GameObject profileBtn;
    ProfileData profileData;

    public override void Init(ProfileData frData, int order, FireStoreColection action, Transform parent)
    {
        profileData = frData;
        transform.parent = parent;
        string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
        if (userID == frData.FBUserID)
            SetRequestBtnActive(false);
        Id = frData.FBUserID;
        nameText.text = frData.FullName;
        progressText.text = frData.KeyGranted.ToString();
        if (order < 4 && frData.FBUserID != "9999")
        {
            medal.gameObject.SetActive(true);
            medal.sprite = medalSprites[order - 1];
        }
        else
            medal.gameObject.SetActive(false);
        //orderText.text = order;
        var avatar = Instantiate(avatarPrefab, avatarTransform);
        avatar.Init(frData);
    }
    public override void SendRequest()
    {
        if (APIController.Instance.CanConnetInternet())
        {
            if (IsBot)
            {
                PlayerPrefs.SetString("bot_energy_timestamp", DataController.ConvertToUnixTime(DateTime.UtcNow).ToString());
                FirebaseServiceController.Instance.BotSendEnergy(Id);
            }
            else
            {
                FirebaseServiceController.Instance.SendFriendRequest(Id);
                APIController.Instance.LogEventFBFriendAction("send_energy_a_friend");
            }

            SetRequestBtnActive(false);
        }
    }
    public void InactiveRequestBtn()
    {
        requestBtn.gameObject.SetActive(false);
    }
    public void InactiveProfileBtn()
    {
        profileBtn.SetActive(false);
    }
    public void OnClickShowProfile()
    {
        var go = Instantiate(frProfilePrefab, FindObjectOfType<MainMenuController>().cameraCanvas);
        APIController.Instance.LogEventFBFriendAction("watch_profile_friend");
        go.Init(profileData);
    }
    public override void SetRequestBtnActive(bool state)
    {
        requestBtn.interactable = state;
    }
}
