using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyRubyBtnController : MonoBehaviour
{
    [SerializeField] private int rubyAmount;
    [SerializeField] private GameObject rootPanel;
    public void OnClick()
    {
        DataController.Instance.Ruby += rubyAmount;
        DataController.Instance.SaveData();
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        Destroy(rootPanel);
    }
}
