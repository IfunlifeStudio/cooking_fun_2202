using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendProfileController : UIView
{
    [SerializeField] TextMeshProUGUI nameTxt, winRateTxt, comboTxt, likeTxt, serveTxt, certiTxt;
    [SerializeField] Animator animator;
    [SerializeField] Image borderImg, avatarImg;
    [SerializeField] RawImage fbAvatarRawImg;
    [SerializeField] Sprite[] avatarSprites, borderSprites;
    [SerializeField] FriendCertificateItemController[] friendCertificates;
    public FriendProfileController Spawn(Transform parent, string avatarUrl)
    {
        var go = Instantiate(gameObject, parent).GetComponent<FriendProfileController>();
        ProfileData profileData = DataController.Instance.GetGameData().profileDatas;
        go.Init(profileData);
        UIController.Instance.PushUitoStack(this);
        return go;
    }
    public async void Init(ProfileData profileData)
    {
        UIController.Instance.PushUitoStack(this);
        animator.Play("Appear");
        nameTxt.text = profileData.FullName;
        if (profileData.TotalWin == 0)
            winRateTxt.text = "0%";
        else
            winRateTxt.text = (int)((profileData.TotalWin / profileData.TotalPlayedGame) * 100) + "%";
        comboTxt.text = profileData.TotalCombo.ToString();
        likeTxt.text = profileData.TotalLike.ToString();
        serveTxt.text = profileData.TotalServeFood.ToString();
        if (profileData.CertiColor != "")
            certiTxt.text = "<color=#" + profileData.CertiColor + ">" + Lean.Localization.LeanLocalization.GetTranslationText(profileData.CertiTile) + "</color>";
        else
            certiTxt.text = Lean.Localization.LeanLocalization.GetTranslationText(profileData.CertiTile);
        int avatarID = (int)profileData.AvatarID;
        int borderID = (int)profileData.BorderID;
        if (borderSprites.Length < borderID)
            borderID = 0;
        borderImg.sprite = borderSprites[borderID];
        if (avatarID != 0)
        {
            fbAvatarRawImg.gameObject.SetActive(false);
            avatarImg.gameObject.SetActive(true);
            avatarImg.sprite = avatarSprites[GetAvatarSprite(avatarID)];
        }
        else
        {
            fbAvatarRawImg.gameObject.SetActive(true);
            avatarImg.gameObject.SetActive(false);
            StartCoroutine(FetchProfilePic(profileData.AvatarUrl));
        }
        //StartCoroutine(GetCertificateList(profileData.FBUserID));
        List<string> certi_lst = new List<string>();
        Dictionary<string, object> certi_data = new Dictionary<string, object>();
        await FirebaseServiceController.Instance.GetCertiInfo(profileData.FBUserID).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                certi_data = task.Result;

            }
        });
        for (int i = 0; i < certi_data.Count; i++)
        {
            int chapter = int.Parse(certi_data.ElementAt(i).Key);
            if (chapter > 0)
                friendCertificates[chapter - 1].Init(int.Parse(certi_data.ElementAt(i).Value.ToString()));
        }
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
        }
        return 1;
    }
    private IEnumerator FetchProfilePic(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        byte[] bytes = DownloadHandlerTexture.GetContent(www).EncodeToPNG();
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        fbAvatarRawImg.texture = tex;
    }
    public void ShowNotify()
    {
        UIController.Instance.ShowNotiFy(NotifyType.CommingSoon);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        //StartCoroutine(DelayClose());
        animator.Play("Disappear");
        Destroy(gameObject, 0.3f);
    }
    IEnumerator DelayClose()
    {
        animator.Play("Disappear");
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject, 0.3f);
    }
}
