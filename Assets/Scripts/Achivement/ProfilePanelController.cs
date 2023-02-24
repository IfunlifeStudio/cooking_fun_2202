using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] Sprite[] avatarSprites, borderSprites;
    [SerializeField] RawImage profileImage;
    [SerializeField] Image borderImg, avatarImg;
    [SerializeField] TextMeshProUGUI winRateTxt, comboTxt, likeTxt, foodTxt, nameTxt, certiText;
    [SerializeField] GameObject certiNotifyGo, friendNotifyGo, comingSoonPrefab, helpPanel, signinBtn, friendPnlPrefab, profileLayer, profileTut;
    [SerializeField] CertificateItemController[] certis;
    [SerializeField] BestFriendPanelController bestFriendController;
    [SerializeField] AvatarPanelController avatarPanel;
    [SerializeField] Transform Layer0;
    FbFriendsController activeFbFriendPanel;
    Action onCloseCallback;
    bool hasFrNoti, hasCertiCoverNoti;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        signinBtn.SetActive(!FB.IsLoggedIn);
        ShowPanel();
        UIController.Instance.PushUitoStack(this);
        yield return new WaitForSeconds(0.2f);
        if (DataController.Instance.HighestRestaurant == 1 && !DataController.Instance.GetTutorialData().Contains(112221))
        {
            DataController.Instance.GetTutorialData().Add(112221);
            var go = Instantiate(profileTut, transform);
            go.GetComponentInChildren<Button>().onClick.AddListener(certis[0].OnClickCertificate);
        }
    }
    public void Init(ProfileData profileData, Action onCloseCallback)
    {
        this.onCloseCallback = onCloseCallback;
        if (profileData.TotalWin == 0)
            winRateTxt.text = "0%";
        else
            winRateTxt.text = (int)((profileData.TotalWin / profileData.TotalPlayedGame) * 100) + "%";
        comboTxt.text = profileData.TotalCombo.ToString();
        likeTxt.text = profileData.TotalLike.ToString();
        foodTxt.text = profileData.TotalServeFood.ToString();
        GetProfile();
        //FirebaseServiceController.Instance.PushProfileData();
    }
    public void CheckCertificateTitle()
    {
        bool has_notify = AchivementController.Instance.HasAchievement();
        string certi_name = DataController.Instance.GetGameData().profileDatas.CertiTile;
        string achieve_color = DataController.Instance.GetGameData().profileDatas.CertiColor;
        if (certi_name == "" && !has_notify)
        {
            certiText.text = Lean.Localization.LeanLocalization.GetTranslationText("more_title");
            //certiBtn.GetComponent<Button>().interactable = false;
        }
        else if (certi_name == "" && has_notify)
        {
            certiNotifyGo.SetActive(has_notify);
            certiText.text = Lean.Localization.LeanLocalization.GetTranslationText("receive_title");
        }
        else
            certiText.text = "<color=#" + achieve_color + ">" + Lean.Localization.LeanLocalization.GetTranslationText(certi_name) + "</color>";
    }
    public void DisplayCertiCoverNotify(int index)
    {
        hasFrNoti = true;
        certis[index].ShowNotification();
    }
    public void OnClickLogin()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            DatabaseController.Instance.LoginWithFaceBook();
    }
    public void OnClickComingSoon()
    {
        UIController.Instance.ShowNotiFy(NotifyType.CommingSoon);
    }
    public void OnClickProfile()
    {
        profileLayer.SetActive(true);
        if (activeFbFriendPanel != null)
            activeFbFriendPanel.gameObject.SetActive(false);
    }
    public void OnClickAvatar()
    {
        certiNotifyGo.SetActive(false);
        var go = Instantiate(avatarPanel, transform);
        go.InIt(() => { GetProfile(); CheckCertificateTitle(); });
    }

    public void GetProfile()
    {
        int avatarID = (int)DataController.Instance.GetGameData().profileDatas.AvatarID;
        int borderID = (int)DataController.Instance.GetGameData().profileDatas.BorderID;
        nameTxt.text = DataController.Instance.GetGameData().profileDatas.FullName;
        if (borderSprites.Length < borderID)
            borderID = 0;
        borderImg.sprite = borderSprites[borderID];
        if (avatarID != 0)
        {
            profileImage.gameObject.SetActive(false);
            avatarImg.gameObject.SetActive(true);
            avatarImg.sprite = avatarSprites[GetAvatarSprite(avatarID)];
        }
        else
        {
            avatarImg.gameObject.SetActive(false);
            profileImage.gameObject.SetActive(true);
            if (PlayerPrefs.GetInt(DatabaseController.LOGIN_TYPE) == 1)
            {
                string filePath = Application.persistentDataPath + "/profile_img.png";
                if (File.Exists(filePath))
                    LoadProfileImage();
                else
                {
                    DatabaseController.Instance.GetFacebookProfileImage();
                    StartCoroutine(WaitingForProfileImage());
                }
            }
        }
    }
    private IEnumerator WaitingForProfileImage()
    {
        string filePath = Application.persistentDataPath + "/profile_img.png";
        while (!File.Exists(filePath))
            yield return new WaitForSeconds(0.1f);
        LoadProfileImage();
    }
    public void LoadProfileImage()
    {
        string filePath = Application.persistentDataPath + "/profile_img.png";
        Texture2D tex = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        profileImage.texture = tex;
        nameTxt.text = DataController.Instance.GetGameData().profileDatas.FullName;
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        TemporarilyHidePanel();
        onCloseCallback.Invoke();
        DataController.Instance.SaveData(false);
        Destroy(gameObject, 0.3f);
    }
    public void TemporarilyHidePanel()
    {
        animator.Play("Disappear");
    }
    public void ShowPanel()
    {
        animator.Play("Appear");
        CheckCertificateTitle();
    }
    private int GetAvatarSprite(int avatarId)
    {
        switch (avatarId)
        {
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 3;
            case 40:
                return 4;
            case 2022:
                return 5;
        }
        return 1;
    }
}
