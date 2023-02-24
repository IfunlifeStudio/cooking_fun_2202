using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class FriendItem : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI nameText, progressText;
    [SerializeField] protected AvatarImageController avatarPrefab;
    [SerializeField] protected Button requestBtn;
    public string Id { get; set; }
    bool isBot = false;
    public bool IsBot { get { return isBot; } set { isBot = value; } }
    public abstract void Init(ProfileData frData, int order, FireStoreColection action, Transform parent);
    //public abstract bool CanRequest();
    public abstract void SendRequest();
    public abstract void SetRequestBtnActive(bool state);
}
