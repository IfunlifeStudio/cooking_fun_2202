using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using DigitalRuby.Tween;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class PiggyBankPanelController : UIView
{
    [SerializeField] private string packageId = "com.cscmobi.cooking.love.piggy1";
    [SerializeField] private TextMeshProUGUI rubyStorageText, rubyFullStorageText, timeLeftText, timeleftFullText, minRuby, maxRuby;
    [SerializeField] private Image storageProgress;
    [SerializeField] private SkeletonGraphic pigAnimator;
    [SerializeField] private Animator backgroundAnim;
    [SerializeField] private Button buyBtn;
    [SerializeField] private GameObject backGround, rewardLayer, inforLayer, fullPiggyPanel;
    [SerializeField] private AudioClip break_piggybank;
    public bool ForceOpen { get; set; }
    void Start()
    {
        backgroundAnim.Play("Appear");
        UIController.Instance.PushUitoStack(this);
        rubyStorageText.text = DataController.Instance.PbRuby.ToString();
        rubyFullStorageText.text = DataController.Instance.PbRuby.ToString();
        float currentFillAmount = (1f * DataController.Instance.PbRuby) / DataController.Instance.CurrentPbStorage;
        storageProgress.rectTransform.sizeDelta = new Vector2(currentFillAmount * 546f, 42f);
        buyBtn.interactable = currentFillAmount >= 0.6f;
        DataController.Instance.PbTimeDuration -= (int)((DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp));
        DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
        StartCoroutine(CountdownPbTime());
        minRuby.text = (DataController.Instance.CurrentPbStorage * 0.6f).ToString();
        maxRuby.text = DataController.Instance.CurrentPbStorage.ToString();
        backGround.SetActive(!DataController.Instance.IsFullPB());
        fullPiggyPanel.SetActive(DataController.Instance.IsFullPB());
    }
    private IEnumerator CountdownPbTime()
    {
        var delay = new WaitForSeconds(1);
        while (DataController.Instance.PbTimeDuration > 0)
        {
            if (DataController.Instance.PbTimeDuration > 0)
            {
                DataController.Instance.PbTimeDuration -= (int)((DataController.ConvertToUnixTime(DateTime.Now) - DataController.Instance.PbTimeStamp));
                DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
                timeleftFullText.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", DataController.Instance.PbTimeDuration / 3600, (DataController.Instance.PbTimeDuration / 60) % 60, DataController.Instance.PbTimeDuration % 60);
                timeLeftText.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", DataController.Instance.PbTimeDuration / 3600, (DataController.Instance.PbTimeDuration / 60) % 60, DataController.Instance.PbTimeDuration % 60);
            }
            else
            {
                timeleftFullText.gameObject.SetActive(false);
                timeLeftText.gameObject.SetActive(false);
            }
            yield return delay;
        }
    }
    public void OnClickBuy()
    {
        backGround.SetActive(false);
        fullPiggyPanel.SetActive(false);
        rewardLayer.SetActive(true);
        ClaimReward();
    }
    public void ClaimReward()
    {
        StartCoroutine(DelayClaimReward());
    }
    IEnumerator DelayClaimReward()
    {
        pigAnimator.AnimationState.SetAnimation(1, "jumpin", false);
        yield return new WaitForSeconds(1.5f);
        AudioController.Instance.PlaySfx(break_piggybank);
        yield return new WaitForSeconds(0.3f);
        pigAnimator.AnimationState.SetAnimation(1, "break", false);
        yield return new WaitForSeconds(4f);
        int ruby = DataController.Instance.PbRuby;
        FindObjectOfType<MainMenuController>().IncreaseGem(transform.position, ruby);
        DataController.Instance.Ruby += ruby;
        DataController.Instance.PbTimeDuration = 0;
        DataController.Instance.isFirstOpenPB = true;
        DataController.Instance.PbRuby = 0;
        DataController.Instance.PbLevel++;
        DataController.Instance.SaveData();
        FindObjectOfType<PiggyBankBtnController>().SetupData();
        APIController.Instance.LogEventEarnRuby(ruby, "buy_IAP");
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        APIController.Instance.LogEventPiggyBankTracking(DataController.Instance.PbLevel, ruby);
        Destroy(gameObject);
    }
    public void OnClickInforBtn()
    {
        inforLayer.SetActive(true);
    }
    public void OnHideInforLayer()
    {
        inforLayer.SetActive(false);
    }
    public override void OnHide()
    {
        backgroundAnim.Play("Disappear");
        StartCoroutine(DelayHide());
    }
    IEnumerator DelayHide()
    {
        UIController.Instance.PopUiOutStack();
        yield return new WaitForSeconds(0.2f);
        if (!ForceOpen && LevelDataController.Instance.lastestPassedLevel != null)
            FindObjectOfType<MainMenuController>().DisplayMenuPanel();
        Destroy(gameObject);
    }
}
