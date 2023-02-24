using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class UpgradeWithRubyPanel : UIView
{
    private int upgradeGold;
    private int ruby;
    private UnityAction aUpgradeUsingRuby;
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI goldAmount, rubyAmount;
    [SerializeField] private AudioClip popUpClip;
    string upgradeDestination = "";
    public void Init(int upgradeGold, UnityAction callback, string _upgradeDes)
    {
        UIController.Instance.PushUitoStack(this);
        upgradeDestination = _upgradeDes;
        AudioController.Instance.PlaySfx(popUpClip);
        anim.Play("Appear");
        goldAmount.text = DataController.Instance.Gold + "/" + upgradeGold;
        ruby = Mathf.Max(1, (upgradeGold - DataController.Instance.Gold) / 20);
        rubyAmount.text = ruby.ToString();
        aUpgradeUsingRuby = callback;
    }
    public void OnClickUpgrade()
    {
        if (DataController.Instance.Ruby >= ruby)
        {

            DataController.Instance.Ruby -= ruby;
            DataController.Instance.Gold = 0;
            aUpgradeUsingRuby.Invoke();
            OnHide();
            DataController.Instance.SaveData(false);
            APIController.Instance.LogEventSpentRuby(ruby, upgradeDestination);
        }
        else
        {
            FindObjectOfType<ShopController>().OpenShopPanel(3);
            OnHide();
        }

    }
    public override void OnHide()
    {
        StartCoroutine(DelayDeactive());
        PlayerPrefs.SetInt("close_ruby_upgrade", 1);
    }
    private IEnumerator DelayDeactive()
    {
        UIController.Instance.PopUiOutStack();
        anim.Play("Disappear");
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
