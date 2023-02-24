using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClearAllDataController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
    }
    public void OnClickYes()
    {
        DatabaseController.Instance.DeleteAccount();
    }
    public void OnClickNo()
    {
        StartCoroutine(DelayDestroy());
    }
    private IEnumerator DelayDestroy()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }    
}
