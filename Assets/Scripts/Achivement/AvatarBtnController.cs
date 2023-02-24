using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarBtnController : MonoBehaviour
{
    [SerializeField] int avatarId;
    [SerializeField] GameObject lockGo;
    bool hasUnlocked;
    // Start is called before the first frame update
    void Start()
    {
        if (avatarId == 0)
        {
            if (PlayerPrefs.GetInt(DatabaseController.LOGIN_TYPE) == 1)
            {
                string filePath = Application.persistentDataPath + "/profile_img.png";
                if (File.Exists(filePath))
                {
                    gameObject.SetActive(true);
                    LoadProfileImage();
                }
                else
                {
                    DatabaseController.Instance.GetFacebookProfileImage();
                    gameObject.SetActive(true);
                    StartCoroutine(WaitingForProfileImage());
                }
            }
            else
                gameObject.SetActive(false);
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
        GetComponentInChildren<RawImage>().texture = tex;
        gameObject.SetActive(true);
    }
    public bool HasAvatarUnlocked()
    {
        if (avatarId < 4)
        {
            hasUnlocked = true;
            return true;
        }
        else
        {
            hasUnlocked = DataController.Instance.HasUnlockAvatar(avatarId);
            if (lockGo != null)
                lockGo.SetActive(!hasUnlocked);
            return hasUnlocked;
        }
    }
    public void OnClickAvatar()
    {
        DataController.Instance.GetGameData().profileDatas.AvatarID = avatarId;
    }
}
