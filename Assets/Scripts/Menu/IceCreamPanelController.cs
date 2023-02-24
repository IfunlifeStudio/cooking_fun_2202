using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamPanelController : UIView
{
    [SerializeField] private Animator animator;
    // Start is called before the first frame update
    public override void OnShow()
    {
        gameObject.SetActive(true);
        animator.Play("Appear");
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        StartCoroutine(IEHidPanel1());
    }
    IEnumerator IEHidPanel1()
    {
        UIController.Instance.PopUiOutStack();
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
