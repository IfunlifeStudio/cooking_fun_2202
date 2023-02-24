using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetWaringPanel : UIView
{
    [SerializeField] private Animator animator;
    void Start()
    {
        UIController.Instance.PushUitoStack(this);
        animator.Play("Appear");
    }
    public void OnClickClose()
    {
        UIController.Instance.PopUiOutStack();
        animator.Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
    public override void OnHide()
    {
        OnClickClose();
    }
}
