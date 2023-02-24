using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AvatarImageController : MonoBehaviour
{
    [SerializeField] protected Sprite[] avatarSprites, borderSprites;
    [SerializeField] protected Image avataImg, borderImg;
    [SerializeField] protected RawImage avatarRawImg;
    public void Init(ProfileData profileData)
    {
        borderImg.sprite = borderSprites[(int)profileData.BorderID];
        if (profileData.AvatarID > 3) profileData.AvatarID = 3;
        if (profileData.AvatarID != 0)
        {
            avatarRawImg.gameObject.SetActive(false);
            avataImg.gameObject.SetActive(true);
            avataImg.sprite = avatarSprites[(int)profileData.AvatarID];
        }
        else
        {
            if (avatarRawImg.texture == null && gameObject.activeInHierarchy && profileData.AvatarUrl != "")
                StartCoroutine(FetchProfilePic(profileData.AvatarUrl));
            else
                OnUnloadAvatar();
        }
    }
    private IEnumerator FetchProfilePic(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        var bytes = DownloadHandlerTexture.GetContent(www);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes.EncodeToPNG());
        yield return new WaitForSeconds(0.2f);
        avatarRawImg.texture = tex;
        avataImg.gameObject.SetActive(false);
        avatarRawImg.gameObject.SetActive(true);
    }
    private void OnUnloadAvatar()
    {
        avatarRawImg.gameObject.SetActive(false);
        avataImg.gameObject.SetActive(true);
    }
}
