using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ShopController : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject shopPanel, itemPanel;
    [SerializeField] private GameObject[] rubyPanelPrefabs;
    [SerializeField] private GameObject rubyPanelFullWithoutAds;
    [SerializeField] private GameObject rubyPanelFullWithAds;
    [SerializeField] private GameObject rubyPanelContainer;
    private GameObject rubyPanel;
    public UnityAction onShopClose = null;
    int tempPanelIndex;
    bool isOpened;
    public override void OnShow()
    {
        shopPanel.SetActive(true);
        itemPanel.SetActive(tempPanelIndex == 2);
        if (rubyPanel != null) rubyPanel.SetActive(tempPanelIndex == 3);
        else
        if (tempPanelIndex == 3)
        {
            PlayerPrefs.SetString("shop_loaction", "shop_full_inhome");
            int index = PlayerClassifyController.Instance.GetPlayerClassifyLevel();
            rubyPanel = Instantiate(rubyPanelPrefabs[index], rubyPanelContainer.transform);
            //rubyPanel = Instantiate(rubyPanelFullWithoutAds, rubyPanelContainer.transform);

        }
        if (!isOpened)
        {
            isOpened = true;
            UIController.Instance.PushUitoStack(this);
        }
    }
    public void OnShowMore()
    {
        int index = PlayerClassifyController.Instance.GetPlayerClassifyLevel();
        Destroy(rubyPanel);
        if (index == 2)
        {
            rubyPanel = Instantiate(rubyPanelFullWithAds, rubyPanelContainer.transform);
        }
        else
        {
            rubyPanel = Instantiate(rubyPanelFullWithoutAds, rubyPanelContainer.transform);
        }
    }
    public void OpenShopPanel(int panelIndex)
    {
        shopPanel.SetActive(true);
        AudioController.Instance.PlaySfx(popUpClip);
        shopPanel.GetComponent<Animator>().Play("Appear");
        tempPanelIndex = panelIndex;
        OnShow();
    }
    public void OnClickItemPanel()
    {
        tempPanelIndex = 2;
        OnShow();
    }
    public void OnClickPackagePanel()
    {
        tempPanelIndex = 1;
        OnShow();
    }
    public void OnClickRubyPanel()
    {
        tempPanelIndex = 3;
        OnShow();
    }
    public override void OnHide()
    {
        StartCoroutine(DelayClose());
        UIController.Instance.PopUiOutStack();
        isOpened = false;
    }
    private IEnumerator DelayClose()
    {
        shopPanel.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.15f);
        shopPanel.SetActive(false);
        itemPanel.SetActive(false);
        Destroy(rubyPanel);
        if (onShopClose != null)
            onShopClose.Invoke();
        onShopClose = null;
    }
}
