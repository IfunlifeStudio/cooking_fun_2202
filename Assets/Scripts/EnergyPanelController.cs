using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class EnergyPanelController : UIView
{
    public string adsName = "";
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Button videoRewardBtn, refillBtn;
    [SerializeField] private Animator buyEnergyAnim;
    [SerializeField] private GameObject endgameRubyPanelPrefab;
    public UnityAction onBuyCompleted;
    public EnergyPanelController Spawn(Transform parent, UnityAction onBuyCallback)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<EnergyPanelController>().onBuyCompleted = onBuyCallback;
        UIController.Instance.PushUitoStack(go.GetComponent<EnergyPanelController>());
        return go.GetComponent<EnergyPanelController>();
    }
    void Start()
    {
        bool isReadyAds = AdsController.Instance.IsRewardVideoAdsReady() && AdsController.Instance.CanShowAds(adsName);
        if (isReadyAds)
        {
            APIController.Instance.LogEventShowAds(adsName);
        }
        else
            refillBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -215f);
        videoRewardBtn.interactable = isReadyAds;
        videoRewardBtn.gameObject.SetActive(isReadyAds);
        refillBtn.interactable = true;
        AudioController.Instance.PlaySfx(popUpClip);
        buyEnergyAnim.Play("Appear");
    }
    public void OnClickBuyEnergy()
    {
        if (DataController.Instance.Ruby >= 40)
        {
            DataController.Instance.Ruby -= 40;
            APIController.Instance.LogEventSpentRuby(40, "buy_energy");
            DataController.Instance.Energy = 5;
            DataController.Instance.SaveData();
            if (onBuyCompleted != null)
                onBuyCompleted.Invoke();
            StartCoroutine(DelayClosePanel());
        }
        else
        {
            if (onBuyCompleted != null)
                FindObjectOfType<ShopController>().OpenShopPanel(3);
            else
            {
                Instantiate(endgameRubyPanelPrefab, transform.parent);
            }
        }
    }
    public void OnClickShowVideo()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
            gameController.IsShowAds = true;
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
        videoRewardBtn.interactable = false;
        refillBtn.interactable = false;
    }
    public void OnCloseAds()
    {

    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        DelayRewardPlayer();
    }
    private void DelayRewardPlayer()
    {
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        DataController.Instance.IncreaseOneEnergy();
        APIController.Instance.LogEventViewAds(adsName);
        if (onBuyCompleted != null)
        {
            onBuyCompleted.Invoke();
        }
        StartCoroutine(DelayClosePanel());
    }
    public override void OnHide()
    {        
        StartCoroutine(DelayClosePanel());
    }
    private IEnumerator DelayClosePanel()
    {
        UIController.Instance.PopUiOutStack();
        buyEnergyAnim.Play("Disappear");
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
