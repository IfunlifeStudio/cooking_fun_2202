using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ItemMessagePanel : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private TextMeshProUGUI itemMessage;
    [SerializeField] private Animator animator;
    public void Show(string message)
    {
        UIController.Instance.PushUitoStack(this);
        AudioController.Instance.PlaySfx(popUpClip);
        itemMessage.text = message;
        animator.Play("Appear");
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayHide());
    }
    private IEnumerator DelayHide()
    {
        animator.Play("Disappear");
        yield return new WaitForSecondsRealtime(0.25f);
        Destroy(gameObject);
    }
}
