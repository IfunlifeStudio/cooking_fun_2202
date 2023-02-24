using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassPosterController : UIView
{
    [SerializeField] Animator animator;
    private void Start()
    {
        animator.Play("Appear");
        UIController.Instance.PushUitoStack(this);
    }
    public void OnClickLetgo()
    {
        FindObjectOfType<XmasPassController>().OnOpenBattlePass();
        OnHide();
    }
    public void OnClickShowLevelPopup()
    {
        OnHide();
        FindObjectOfType<MainMenuController>().OnOpenMaxResInZone();
    }
    public void OnClickClose()
    {
        OnHide();
        FindObjectOfType<MainMenuController>().ShowEarlyOffer();
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        UIController.Instance.PopUiOutStack();
        Destroy(gameObject, 0.4f);
    }
}
