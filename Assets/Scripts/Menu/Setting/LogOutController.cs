using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LogOutController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
    }
    public void OnClickLogOut()
    {
        StartCoroutine(DelayLogOut());
    }
    private IEnumerator DelayLogOut()
    {
        DatabaseController.Instance.LogOutFacebook();
        yield return StartCoroutine(DelayClosePanel());
        //SceneController.Instance.LoadScene("Login");
    }
    public void OnClickClose()
    {
        StartCoroutine(DelayClosePanel());
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
