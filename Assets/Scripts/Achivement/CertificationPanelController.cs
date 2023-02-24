using DigitalRuby.Tween;
using Facebook.Unity;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CertificationPanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip completeAudio;
    [SerializeField] RawImage profileImage;
    [SerializeField] TextMeshProUGUI userNameTxt, winRateTxt, comboTxt, likeTxt, serveTxt, timeText;
    [SerializeField] GameObject helpPanel, saveBtn, shareBtn, closeBtn, inforBtn, certiTut, playBtn;
    [SerializeField] Image certiImage, avatarImg, resBg, leftCoverImg, rightCoverImg, pushLockImg, splintImg1, splintImg2;
    [SerializeField] Image[] CuffsImg;
    [SerializeField] Sprite[] certiSprites, avatarSprites, resBgSprites, coverSprites, pushLockSprites, splintSprites, cuffsSprites;
    //[SerializeField] SkeletonGraphic[] rewardBox;
    [SerializeField] ScreenshotImageController screenshotImage;
    //[SerializeField] CertificateRewardPanel certificateReward;
    [SerializeField] FullCertificateLayerController fullCertificateLayer;
    ProfilePanelController profilePanel;
    int resID, winRate;
    string cerStr;
    Action onCloseCallback;
    bool isDelayShare;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Appear");
        UIController.Instance.PushUitoStack(this);
        //totalState = DataController.
    }
    public void SetupData(int resID, bool isFulKey, bool isFullStage, ProfilePanelController profilePanel, Action<string, int> onTrackingCallback = null)
    {
        this.resID = resID;
        this.profilePanel = profilePanel;
        var certificateData = DataController.Instance.GetGameData().GetCertificateDataAtRes(resID);
        leftCoverImg.sprite = coverSprites[resID - 1];
        rightCoverImg.sprite = coverSprites[resID - 1];
        pushLockImg.sprite = pushLockSprites[resID - 1];
        splintImg1.sprite = splintSprites[resID - 1];
        splintImg2.sprite = splintSprites[resID - 1];
        playBtn.SetActive(!isFullStage);
        saveBtn.SetActive(isFullStage);
        for (int i = 0; i < CuffsImg.Length; i++)
        {
            CuffsImg[i].sprite = cuffsSprites[resID - 1];
        }
        if (certificateData.CompletedTime != "")
            timeText.text = certificateData.CompletedTime;
        else
        {
            certificateData.CompletedTime = DateTime.Now.Date.ToString("MM/dd/yyyy");
            timeText.text = certificateData.CompletedTime;
        }
        certiImage.sprite = certiSprites[GetWinrating(certificateData.WinRate)];
        resBg.sprite = resBgSprites[resID - 1];
        winRate = certificateData.WinRate;
        winRateTxt.text = winRate + "%";
        comboTxt.text = certificateData.Combo.ToString();
        likeTxt.text = certificateData.Like.ToString();
        serveTxt.text = certificateData.Food.ToString();
        cerStr = Lean.Localization.LeanLocalization.GetTranslationText(AchivementController.Instance.GetWinrateCerti(resID));
        string in_res_cooking = Lean.Localization.LeanLocalization.GetTranslationText("certi_res_" + (resID));
        string certificateStr = "<color=#" + AchivementController.Instance.GetWinrateCerti(resID, false) + ">" + cerStr + "</color> " + in_res_cooking;
        if (certificateData.RewardRecord[2] == 1) certificateData.IsCompletedCertificate = true;
        //bool isfullver = /*certificateData.CompletedTime != "" && */certificateData.RewardRecord[2] == 1 || certificateData.IsCompletedCertificate == true || totalStageInRes == totalStageGranted;
        fullCertificateLayer.Init(isFulKey, resID, certificateStr, certificateData.CompletedTime);
        if (isFulKey && !certificateData.IsCompletedCertificate)
        {
            certificateData.IsCompletedCertificate = true;
            OnCompleteCertificate();
        }
        if (resID == 1 && !isFulKey && !DataController.Instance.GetTutorialData().Contains(112222))
        {
            DataController.Instance.GetTutorialData().Add(112222);
            Instantiate(certiTut, transform);
            DataController.Instance.SaveData(false);
        }
        GetProfile();
    }
    public int GetWinrating(int winrate)
    {
        if (winrate >= 90)
            return 0;
        else if (winrate >= 80)
            return 1;
        else if (winrate >= 70)
            return 2;
        else
            return 3;
    }
    #region profile
    public void GetProfile()
    {
        int avatarID = (int)DataController.Instance.GetGameData().profileDatas.AvatarID;
        userNameTxt.text = DataController.Instance.GetGameData().profileDatas.FullName;
        if (avatarID == 0)
        {
            avatarImg.gameObject.SetActive(false);
            profileImage.gameObject.SetActive(true);
            string filePath = Application.persistentDataPath + "/profile_img.png";
            if (File.Exists(filePath))
                LoadProfileImage();
            else
            {
                DatabaseController.Instance.GetFacebookProfileImage();
                StartCoroutine(WaitingForProfileImage());
            }
        }
        else
        {
            profileImage.gameObject.SetActive(false);
            avatarImg.gameObject.SetActive(true);
            avatarImg.sprite = avatarSprites[GetAvatarSprite(avatarID)];
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
        userNameTxt.text = DataController.Instance.GetGameData().profileDatas.FullName;
    }
    #endregion
    public void OnClickHelpBtn()
    {
        Instantiate(helpPanel, transform);
    }
    public void OnClickShare()
    {
        APIController.Instance.LogEventCertiAction("share_cert", resID);
        if (!isDelayShare)
        {
            SetActiveAllButton(false);
            int resWidth = Screen.width;
            int resHeight = Screen.height;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            var myCamera = FindObjectOfType<Camera>();
            myCamera.targetTexture = rt;
            Texture2D ss = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            myCamera.Render();
            RenderTexture.active = rt;
            ss.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            ss.Apply();
            myCamera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            string filePath = Path.Combine(Application.temporaryCachePath, "sharedimg.png");
            File.WriteAllBytes(filePath, ss.EncodeToPNG());
            // To avoid memory leaks
            Destroy(ss);
            SetActiveAllButton(true);
            new NativeShare().AddFile(filePath).SetSubject("Certificate").AddTarget("com.facebook.katana").Share();
            isDelayShare = false;
        }
    }
    public void SetActiveAllButton(bool status)
    {
        saveBtn.SetActive(status);
        shareBtn.SetActive(status);
        closeBtn.SetActive(status);
        inforBtn.SetActive(status);
        playBtn.SetActive(status);
    }
    public void OnClickSave()
    {
        AndroidRuntimePermissions.Permission perm = AndroidRuntimePermissions.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE");
        if (perm == AndroidRuntimePermissions.Permission.Granted)
        {
            APIController.Instance.LogEventCertiAction("save_cert", resID);
            SaveImage();
        }
        else
        {
            AndroidRuntimePermissions.RequestPermission("android.permission.WRITE_EXTERNAL_STORAGE");
        }
    }
    public void SaveImage()
    {
        SetActiveAllButton(false);
        var myCamera = FindObjectOfType<Camera>();
        int resWidth = Screen.width;
        int resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        myCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        myCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        myCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "CookingLove", "cer_screenshot.png", null);
        string filename = Path.Combine(Application.temporaryCachePath, "cer_screenshot.png");
        System.IO.File.WriteAllBytes(filename, bytes);
        SetActiveAllButton(true);
        var scs = Instantiate(screenshotImage, transform);
        scs.Init(filename);
        StartCoroutine(IMove(scs.gameObject, saveBtn.transform.position, 1, () => { Destroy(scs.gameObject); }));
        Destroy(screenShot);
    }
    private void OnCompleteCertificate()
    {
        fullCertificateLayer.OnCompleteCertificate();
        AudioController.Instance.PlaySfx(completeAudio);
        if (FB.IsLoggedIn)
            FirebaseServiceController.Instance.PushCertificateData(resID.ToString(), winRate);
    }
    public void OnClickPlay()
    {
        if (DataController.Instance.IsRestaurantUnlocked(resID))
        {
            StartCoroutine(DelayShowLevelDetailPanel());
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
    IEnumerator DelayShowLevelDetailPanel()
    {
        DataController.Instance.currentChapter = resID;
        var mainmenu = FindObjectOfType<MainMenuController>();
        if (mainmenu != null)
        {
            Destroy(profilePanel.gameObject);
            animator.Play("Disappear");
            UIController.Instance.PopUiOutStack();
            mainmenu.ShowUI();
            yield return new WaitForSeconds(0.4f);
            mainmenu.detailPanel.OpenDetailPanel(1);
            Destroy(gameObject);
        }

    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed, Action action = null)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f, (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(true);
        while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 2;
            gameObject.transform.localScale = new Vector2(1 / (time * 4), 1 / (time * 4));
            yield return null;
        }
        if (action != null)
            action.Invoke();
    }

    public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t1;
        float tt = t1 * t1;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t1 * p1;
        p += tt * p2;
        return p;
    }
    public void OnClose()
    {
        OnHide();
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        UIController.Instance.PopUiOutStack();
        profilePanel?.ShowPanel();
        Destroy(gameObject, 0.3f);
    }
}
