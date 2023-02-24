using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FriendController : MonoBehaviour
{
    [SerializeField] protected FriendItem friendItemPrefab;
    [SerializeField] protected ScrollRect scrollPanel;
    public abstract void Init(List<ProfileData> friendDataList);

    public FriendItem SpawnFriend(ProfileData frData, int order, FireStoreColection action)
    {
        FriendItem friend = null;
        friend = Instantiate(friendItemPrefab, scrollPanel.content);
        friend.Init(frData, order, action, scrollPanel.content);
        return friend;
    }
}
