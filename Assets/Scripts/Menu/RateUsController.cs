using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RateUsController : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Animator animator;
    private string storeUrl = "market://details?id=com.cscmobi.cooking.love.free";
#if UNITY_IOS
        private string storeUrl = "itms-apps://itunes.apple.com";
#endif
    Action onCloseCallback;
    public override void OnShow()
    {
        UIController.Instance.PushUitoStack(this);
        animator.Play("Appear");
        AudioController.Instance.PlaySfx(popUpClip);
    }
    public void OnClickContinue()
    {
        PlayerPrefs.SetInt("rateus", 5);
        Application.OpenURL(storeUrl);
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.3f);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        animator.Play("Disappear");
        this.onCloseCallback?.Invoke();
        Destroy(gameObject, 0.3f);
    }
    public void OnClickOpenStore()
    {
        PlayerPrefs.SetInt("rateus", 5);
        Application.OpenURL(storeUrl);
        OnHide();
    }

}
