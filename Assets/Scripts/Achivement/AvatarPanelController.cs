using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPanelController : UIView
{
    [SerializeField] GameObject avatarLayer, borderLayer, avatarTag, borderTag, helpPanel, notifyGo, notifyPanel;
    [SerializeField] Sprite[] avatarSprites, borderSprites;
    [SerializeField] Image avatarImg, borderImg;
    [SerializeField] RawImage avatarRawImg;
    [SerializeField] private Toggle[] avatarToggles;
    [SerializeField] private Toggle[] borderToggles;
    [SerializeField] TMP_InputField nameIf;
    [SerializeField] TextMeshProUGUI certificateTitle;
    [SerializeField] BestFriendPanelController certificateTitleSelector;
    private int index = 0, tmpAvatarId, tmpBorderId, tmpSaveValue;

    Action onCloseCallback;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
        yield return new WaitForSeconds(0.2f);
        var profileData = DataController.Instance.GetGameData().profileDatas;
        nameIf.placeholder.GetComponent<TextMeshProUGUI>().text = profileData.FullName;
        string certi_name = profileData.CertiTile;
        string achieve_color = profileData.CertiColor;
        tmpAvatarId = (int)profileData.AvatarID;
        tmpBorderId = (int)profileData.BorderID;
        DisplayAvatarWithId(tmpAvatarId);
        DisplayBorderWithId(tmpBorderId);
        TurnToggleOn((int)profileData.AvatarID, avatarToggles);
        TurnToggleOn((int)profileData.BorderID, borderToggles);
        CheckCertificateTitle();
        for (int i = 0; i < borderToggles.Length; i++)
        {
            if (borderToggles[i].GetComponent<BorderBtnController>().HasBorderUnlocked())
                borderToggles[i].onValueChanged.AddListener(OnClickBorderToggle);
        }
        for (int i = 0; i < avatarToggles.Length; i++)
        {
            if (avatarToggles[i].GetComponent<AvatarBtnController>().HasAvatarUnlocked())
                avatarToggles[i].onValueChanged.AddListener(OnClickAvatarToggle);
        }
        tmpSaveValue = 0;
    }
    public void CheckCertificateTitle()
    {
        bool has_notify = AchivementController.Instance.HasAchievement();
        string certi_name = DataController.Instance.GetGameData().profileDatas.CertiTile;
        string achieve_color = DataController.Instance.GetGameData().profileDatas.CertiColor;
        if (certi_name == "" && !has_notify)
        {
            certificateTitle.text = Lean.Localization.LeanLocalization.GetTranslationText("more_title");
        }
        else if (certi_name == "" && has_notify)
        {
            notifyGo.SetActive(has_notify);
            certificateTitle.text = Lean.Localization.LeanLocalization.GetTranslationText("receive_title");
        }
        else
            certificateTitle.text = "<color=#" + achieve_color + ">" + Lean.Localization.LeanLocalization.GetTranslationText(certi_name) + "</color>";
    }
    public void TurnToggleOn(double id, Toggle[] toggles)
    {

        for (int i = 0; i < toggles.Length; i++)
        {
            if (id.ToString() == toggles[i].name)
            {
                toggles[i].isOn = id.ToString() == toggles[i].name;
                index = i;
                break;
            }
        }
    }
    public void InIt(Action onCloseCallback)
    {
        this.onCloseCallback = onCloseCallback;
    }
    public void OnClickAvatarLayer()
    {
        avatarTag.SetActive(true);
        avatarLayer.SetActive(true);
        borderTag.SetActive(false);
        borderLayer.SetActive(false);
    }
    public void OnClickBorderLayer()
    {
        avatarTag.SetActive(false);
        avatarLayer.SetActive(false);
        borderLayer.SetActive(true);
        borderTag.SetActive(true);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        if (onCloseCallback != null)
            onCloseCallback.Invoke();
        GetComponent<Animator>().Play("Disappear");
        DataController.Instance.SaveData(false);
        Destroy(gameObject, 0.3f);
    }
    public void OnClickAvatarBtn(int avatarId)
    {
        tmpSaveValue++;
        this.tmpAvatarId = avatarId;
    }
    public void OnClickBorderBtn(int borderId)
    {
        tmpSaveValue++;
        this.tmpBorderId = borderId;
    }
    private void DisplayAvatarWithId(int id)
    {
        if (id != 0)
        {
            avatarRawImg.gameObject.SetActive(false);
            avatarImg.gameObject.SetActive(true);
            avatarImg.sprite = avatarSprites[GetAvatarSprite(id)];
        }
        else
        {
            avatarImg.gameObject.SetActive(false);
            avatarRawImg.gameObject.SetActive(true);
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
    private void DisplayBorderWithId(int id)
    {
        if (borderSprites.Length < id)
            id = 0;
        borderImg.sprite = borderSprites[id];
    }
    public void OnClickBorderToggle(bool state)
    {
        if (state)
        {
            DisplayBorderWithId(tmpBorderId);
        }

    }
    public void OnClickAvatarToggle(bool state)
    {
        if (state)
        {
            DisplayAvatarWithId(tmpAvatarId);
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
            case 2022:
                return 5;
        }
        return 1;
    }
    public void OnCickCertificaterTitle()
    {
        tmpSaveValue++;
        if (!AchivementController.Instance.HasAchievement())
        {
            Instantiate(helpPanel, transform);
        }
        else
        {
            var go = Instantiate(certificateTitleSelector, transform) as BestFriendPanelController;
            go.Init(() =>
            {
                string certi_name = DataController.Instance.GetGameData().profileDatas.CertiTile;
                string achieve_color = DataController.Instance.GetGameData().profileDatas.CertiColor;
                if (certi_name != "") certificateTitle.text = "<color=#" + achieve_color + ">" + Lean.Localization.LeanLocalization.GetTranslationText(certi_name) + "</color>";
            });
        }
        notifyGo.SetActive(false);
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
        avatarRawImg.texture = tex;
    }
    public void OnClickSaveBtn()
    {
        tmpSaveValue = 0;
        if (nameIf.text != "")
            DataController.Instance.GetGameData().profileDatas.FullName = nameIf.text;
        DataController.Instance.GetGameData().profileDatas.AvatarID = tmpAvatarId;
        DataController.Instance.GetGameData().profileDatas.BorderID = tmpBorderId;
    }
    public void OnClickClose()
    {
        if (tmpSaveValue > 0)
            notifyPanel.SetActive(true);
        else
            OnHide();
    }
    public void OnClickSaveAndClose()
    {
        OnClickSaveBtn();
        OnHide();
    }
}
