using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        //StartCoroutine(DelayLoadLogin());
        //animator.Play("Logo");
    }
    IEnumerator DelayLoadLogin()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Splash");
        //SceneController.Instance.LoadScene("Splash", false);
    }
    public void LoadSplashScene()
    {
        SceneManager.LoadScene("Login");
    }
}
