using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmasPassPanelController : UIView
{
    [SerializeField] private GameObject maxPriceBtn;
    [SerializeField] private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Appear");
        UIController.Instance.PushUitoStack(this);
        maxPriceBtn.SetActive(true);
    }
    public void OnClickBuy()
    {
        BattlePassDataController.Instance.HasUnlockVip2022 = true;
        APIController.Instance.LogEventBuySuccessBP(BattlePassDataController.Instance.CurrentBpLevel.ToString());
        DataController.Instance.SaveData();
        OnHide();
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        UIController.Instance.PopUiOutStack();
        Destroy(gameObject, 0.4f);
    }
}
