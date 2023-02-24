using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GiftCodePanelController : UIView
{
    private int checkCount = 0;
    [SerializeField] private TMP_InputField giftCodeIf;
    [SerializeField] private TextMeshProUGUI logTxt;
    [SerializeField] GameObject enterCodeLayer, failLayer, processingLayer,closeBtn;
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
    }
    public void OnClickClaim()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIController.Instance.ShowNotiFy(NotifyType.NoInternet);
            return;
        }
        if (giftCodeIf.text != "")
        {
            processingLayer.SetActive(true);
            APIController.Instance.CheckGiftCode(giftCodeIf.text, OnCheckSuccess, OnCheckFail);
        }
    }
    public void OnCheckSuccess(GiftPack giftPack)
    {
        processingLayer.SetActive(false);
        OnHide();
        FindObjectOfType<RewardPanelController>().Init(giftPack.GoldAmount, giftPack.RubyAmount, giftPack.UnlimitedEnergyTime, giftPack.ItemId, giftPack.ItemAmount);
    }
    public void OnCheckFail(string log)
    {
        checkCount++;
        processingLayer.SetActive(false);
        logTxt.text = log;
        closeBtn.SetActive(false);
        failLayer.SetActive(true);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.4f);
    }
    public void OnClickCloseFailLayer()
    {
        if (checkCount >= 5)
            OnHide();
        else
        {
            closeBtn.SetActive(true);
            failLayer.SetActive(false);
        }

    }
}
