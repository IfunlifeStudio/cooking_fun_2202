using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubcribePanelController : UIView
{ 
     bool isConfirm=false;
   // [SerializeField] string subUrl = "https://csccampstorage.nyc3.cdn.digitaloceanspaces.com/subscribe/p29pcookinglove.html";//paid
    string subUrl = "https://csccampstorage.nyc3.cdn.digitaloceanspaces.com/subscribe/p29fcookinglove.html";//nopaid
    string policyUrl = "https://cscmobi.com/privacy-policy";
    [SerializeField] Button openLinkBtn;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("Appear");
        UIController.Instance.PushUitoStack(this);
    }
    public void OnClickConfirm(bool state)
    {
        if (state)
        {
            isConfirm = true;
            openLinkBtn.interactable = true;
        }
        else
        {
            isConfirm = false;
            openLinkBtn.interactable = false;
        }
    }
    public void OnClickSubcribeLink()
    {
        Application.OpenURL(subUrl);
    }
    public void OnClickPolicy()
    {
        Application.OpenURL(policyUrl);
    }
    public override void OnHide()
    {
        StartCoroutine(DelayHide());
        UIController.Instance.PopUiOutStack();
    }
    public IEnumerator DelayHide()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
