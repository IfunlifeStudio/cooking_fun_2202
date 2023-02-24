using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum NotifyType
{
    NoInternet,
    FullEnergy,
    NotLoggedIn,
    CommingSoon,
    UnlockBorder
}

public class NotifyPanelController : UIView
{
    [SerializeField] TextMeshProUGUI titleTxt, descriptionTxt;
    [SerializeField] Animator animator;
    [SerializeField] GameObject closeBtn, signinBtn;
    Action onCloseCallback;
    public NotifyPanelController Spawn(NotifyType notifyType, Action callback)
    {
        onCloseCallback = callback;
        var go = Instantiate(gameObject).GetComponent<NotifyPanelController>();
        UIController.Instance.PushUitoStack(go);
        go.Init(notifyType);
        return go;
    }
    public void Init(NotifyType notifyType)
    {
        if (notifyType == NotifyType.NotLoggedIn)
        {
            closeBtn.transform.localPosition = new Vector2(-135,-100);
            signinBtn.SetActive(true);
        }
        titleTxt.text = GetNotifyTitle(notifyType);
        descriptionTxt.text = GetNotifyDescription(notifyType);
        animator.Play("Appear");
    }
    private string GetNotifyTitle(NotifyType notifyType)
    {
        string title = "";
        switch (notifyType)
        {
            case NotifyType.NoInternet:
                title = Lean.Localization.LeanLocalization.GetTranslationText("setting_account_warn_tittle");
                break;
            case NotifyType.NotLoggedIn:
            case NotifyType.CommingSoon:
            case NotifyType.FullEnergy:
            case NotifyType.UnlockBorder:
                title = Lean.Localization.LeanLocalization.GetTranslationText("notify_title");
                break;

        }
        return title;
    }
    private string GetNotifyDescription(NotifyType notifyType)
    {
        string description = "";
        switch (notifyType)
        {
            case NotifyType.NoInternet:
                description = Lean.Localization.LeanLocalization.GetTranslationText("des_no_internet");
                break;
            case NotifyType.FullEnergy:
                description = Lean.Localization.LeanLocalization.GetTranslationText("energy_full_des");
                break;
            case NotifyType.NotLoggedIn:
                description = Lean.Localization.LeanLocalization.GetTranslationText("try_login");
                break;
            case NotifyType.CommingSoon:
                description = Lean.Localization.LeanLocalization.GetTranslationText("feature_comingsoon");
                break;
            case NotifyType.UnlockBorder:
                description = Lean.Localization.LeanLocalization.GetTranslationText("unlock_border");
                break;
        }
        return description;
    }
    public void OnClickSignin()
    {
        DatabaseController.Instance.LoginWithFaceBook();
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        if (onCloseCallback != null)
            onCloseCallback.Invoke();
        UIController.Instance.PopUiOutStack();
        Destroy(gameObject, 0.3f);
    }
}
