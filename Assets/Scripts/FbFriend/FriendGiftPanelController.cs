using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendGiftPanelController : FriendController
{
    [SerializeField] GameObject descriptionTxtGo, claimAllBtn;

    List<FriendGiftItemController> friendList = new List<FriendGiftItemController>();
    bool hasInit = false;
    public override void Init(List<ProfileData> friendDataList)
    {

        int order = 0;
        if (!hasInit)
        {
            hasInit = true;
            DatabaseController.Instance.SpawnLoading();
            foreach (var fr in friendDataList)
            {
                order++;
                if (DataController.Instance.frNeedHelpList.Contains(fr.FBUserID))
                {
                    FriendGiftItemController go = SpawnFriend(fr, order, FireStoreColection.Help).GetComponent<FriendGiftItemController>();
                    friendList.Add(go);
                }
                if (DataController.Instance.frGivenEnergyList.Contains(fr.FBUserID))
                {
                    FriendGiftItemController go = SpawnFriend(fr, order, FireStoreColection.GiveEnergy).GetComponent<FriendGiftItemController>();
                    friendList.Add(go);
                }
                if (DataController.Instance.frGivenRubyList.Contains(fr.FBUserID))
                {
                    FriendGiftItemController go = SpawnFriend(fr, order, FireStoreColection.GiveRuby).GetComponent<FriendGiftItemController>();
                    friendList.Add(go);
                }
                if (DataController.Instance.frWaveList.Contains(fr.FBUserID))
                {
                    FriendGiftItemController go = SpawnFriend(fr, order, FireStoreColection.Wave).GetComponent<FriendGiftItemController>();
                    friendList.Add(go);
                }
            }
            DatabaseController.Instance.DespawnLoadingOverlay();
        }
        bool isEmptyList = (friendList.Count == 0);
        descriptionTxtGo.SetActive(isEmptyList);
        claimAllBtn.SetActive(!isEmptyList);
    }
    public void OnClickClaimAll()
    {
        if (APIController.Instance.CanConnetInternet())
        {
            for (int i = 0; i < friendList.Count; i++)
            {
                if (friendList[i].CanRequest())
                {
                    friendList.Remove(friendList[i]);
                    if (friendList.Count == 0)
                        claimAllBtn.SetActive(false);
                }
            }
        }
    }
}
