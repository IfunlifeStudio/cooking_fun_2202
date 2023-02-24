using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNewRestaurantController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] Sprite[] resIconArray;
    [SerializeField] Image resIcon;
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        animator.Play("Appear");
    }
    public void Init(int chapter)
    {
        for (int i = 0; i < resIconArray.Length; i++)
            resIcon.sprite = resIconArray[chapter - 1];
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        FindObjectOfType<RewardPanelController>().Init(0, 0, 15, new int[0], new int[0], false, "default");
        animator.Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
}