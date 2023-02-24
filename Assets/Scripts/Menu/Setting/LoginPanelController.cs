using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPanelController : UIView
{
    [SerializeField] private GameObject signinFacebookBtn;
    [SerializeField] private GameObject signinAppleIdBtn;
    bool isAndroidOS;
    bool isClaimed = false;
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
        SettingDisplayButton();
    }
    public void OnClickLogIn()
    {
        DatabaseController.Instance.LoginWithFaceBook();
        StartCoroutine(DelayClosePanel());
    }
    public void OnClickLogOut()
    {
        DatabaseController.Instance.LogOutFacebook();
        StartCoroutine(DelayClosePanel());
    }
    public void OnClickSignInWithAppleId()
    {
        DatabaseController.Instance.LoginWithAppleID();
        StartCoroutine(DelayClosePanel());
    }
    public void OnClickClaim()
    {
        if (isClaimed == false)
        {
            isClaimed = true;
            DataController.Instance.GetGameData().hasClaimLoginReward = 1;
            DataController.Instance.Ruby += 50;
            DataController.Instance.AddUnlimitedEnergy(30);
            DataController.Instance.SaveData();
            FindObjectOfType<MainMenuController>().IncreaseGem(new Vector3(0, 0, -1), 50);
            FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
            StartCoroutine(DelayClosePanel());
            APIController.Instance.LogEventEarnRuby(50, "reward");
            UIController.Instance.PopUiOutStack();
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClosePanel());
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void SettingDisplayButton()
    {
#if UNITY_ANDROID
        isAndroidOS = true;
        if (signinFacebookBtn != null)
            signinFacebookBtn.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -225, 0);
#endif
        bool isAppleConnected = (PlayerPrefs.GetInt("LOGIN_TYPE") == 3);
        bool isFbConnected = FB.IsLoggedIn;
        if (signinFacebookBtn != null)
            signinFacebookBtn.SetActive(!isFbConnected && !isAppleConnected);
        if (signinAppleIdBtn != null)
            signinAppleIdBtn.SetActive(!isAppleConnected && !isAndroidOS);
    }
}

