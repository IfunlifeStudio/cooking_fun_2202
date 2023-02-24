using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPanelController : UIView
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject backdround;
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        anim.Play("Appear");
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayContinue());
    }
    private IEnumerator DelayContinue()
    {
        anim.Play("Disappear");
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void OnClickQuit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
