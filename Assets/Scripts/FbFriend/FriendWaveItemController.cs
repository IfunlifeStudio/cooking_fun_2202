using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendWaveItemController : FriendItem
{
    [SerializeField] Transform avatarTransform;

    public override void Init(ProfileData frData, int order, FireStoreColection action, Transform parent)
    {
        transform.parent = parent;
        nameText.text = frData.FullName;
        Id = frData.FBUserID;
        var avatar = Instantiate(avatarPrefab, avatarTransform);
        avatar.Init(frData);
    }

    public override void SendRequest()
    {
        gameObject.SetActive(false);
        FirebaseServiceController.Instance.WavingFriend(Id);
        DataController.Instance.GetGameData().WavedFriendList.Add(Id);
        DataController.Instance.SaveData(false);
        APIController.Instance.LogEventFBFriendAction("wave_friend");
    }

    public override void SetRequestBtnActive(bool state)
    {
        throw new System.NotImplementedException();
    }
}
