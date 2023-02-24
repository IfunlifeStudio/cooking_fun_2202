using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;
using DigitalRuby.Tween;

public class WinPanelController : UIView
{
    [SerializeField] private string adsName = "x2Win";
    [SerializeField] private SkeletonGraphic skeletonAnimation, pigAnim;
    private Spine.AnimationState spineAnimationState;
    [SerializeField] private GameObject endPanelFrame, piggyBankPanel, certiPanel;
    [SerializeField] private TextMeshProUGUI goldText /*expText*/, x2CoinText, piggyGoldBonusTxt, cer_ComboTxt, cer_LikeTxt, cer_ServeTxt;
    [SerializeField] private ParticleSystem confettiBlast, confettiDrop;
    [SerializeField] private Button videoRewardBtn, continueBtn;
    [SerializeField] private ComboTextController goldTextPrefab;
    [SerializeField] Image certiIcon, bookIcon;
    [SerializeField] Sprite[] certiIconSprites, bookIconSprites;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioClip IncreaseGoldAudio;
    [Header("Battle Pass")]
    [SerializeField] Image battlepassProgressImg;
    [SerializeField] private TextMeshProUGUI battlepassProgressTxt;
    [SerializeField] private GameObject sockBtn;
    private GameController gameController;
    bool isClick = false;
    bool isClaimX2;
    public void Init(int goldCollected, int cer_combo, int cer_like, int cer_serveFood)
    {
        isClaimX2 = false;
        gameController = FindObjectOfType<GameController>();
        cer_ComboTxt.text = cer_combo.ToString();
        cer_LikeTxt.text = cer_like.ToString();
        cer_ServeTxt.text = cer_serveFood.ToString();
        sockBtn.SetActive(DataController.Instance.IsShowNoel == 1);
        if (!LevelDataController.Instance.IsExtraJob && DataController.Instance.IsShowNoel == 1)
        {
            int totalKey = DataController.Instance.GetTotalLevelsPerRestaurant(gameController.levelData.chapter);
            int totalKeyGranted = DataController.Instance.GetRestaurantKeyGranted(gameController.levelData.chapter);
            if (BattlePassDataController.Instance.CanClaimExp)
            {
                int level = BattlePassDataController.Instance.CurrentBpLevel;
                int nextLevel = Mathf.Min(level + 1, BattlePassDataController.BP_MAX_LEVEL);
                var nextElementData = BattlePassDataController.Instance.GetBattlePassElementData(nextLevel);
                int currentExp = BattlePassDataController.Instance.CurrentBpExp;
                int requireExp = nextElementData.Exp;
                //battlepassProgressTxt.text = currentExp + "/" + requireExp;
                //float bpProgress = currentExp * 1f / requireExp;
                //battlepassProgressImg.fillAmount = Mathf.Min(bpProgress, 1);
                LevelDataController.Instance.IsSockLevel = true;
                int bonusValue = 5;
                if (gameController.levelData.id > 10 && gameController.levelData.id <= 20)
                    bonusValue = 10;
                else if (gameController.levelData.id > 20 && gameController.levelData.id <= 40)
                    bonusValue = 15;
                FillProgress(currentExp, bonusValue, requireExp);
            }
            else
            {
                battlepassProgressImg.transform.parent.gameObject.SetActive(false);
            }
            certiIcon.enabled = totalKeyGranted == totalKey;
            certiPanel.SetActive(true);
            bookIcon.transform.parent.gameObject.SetActive(true);
            var certidata = DataController.Instance.GetGameData().GetCertificateDataAtRes(gameController.levelData.chapter);
            certidata.Combo += cer_combo;
            certidata.Like += cer_like;
            certidata.Food += cer_serveFood;
            certidata.WinGame++;
            float winRate;
            try
            {
                winRate = ((float)certidata.WinGame / (float)certidata.PlayedGame) * 100;
                if (winRate > 0)
                    certidata.WinRate = (int)winRate;
            }
            catch { Debug.LogError("not divide"); }
            if (totalKeyGranted >= totalKey)
            {
                if (certidata.WinRate >= 90)
                    certiIcon.color = new Color32(255, 255, 0, 255);
                else if (certidata.WinRate >= 80)
                    certiIcon.color = new Color32(171, 56, 237, 255);
                else if (certidata.WinRate >= 70)
                    certiIcon.color = new Color32(29, 126, 225, 255);
                else
                    certiIcon.color = new Color32(4, 177, 0, 255);
            }
            bookIcon.sprite = bookIconSprites[gameController.levelData.chapter - 1];
            certiIcon.sprite = certiIconSprites[gameController.levelData.chapter - 1];
        }
        else
        {
            certiPanel.SetActive(false);
            bookIcon.transform.parent.gameObject.SetActive(false);
           // battlepassProgressImg.transform.parent.gameObject.SetActive(false);
        }
        var profileDatas = DataController.Instance.GetGameData().profileDatas;
        profileDatas.TotalCombo += cer_combo;
        profileDatas.TotalLike += cer_like;
        profileDatas.TotalServeFood += cer_serveFood;
        profileDatas.TotalWin += 1;
        UIController.Instance.PushUitoStack(this);
        gameObject.SetActive(true);
        goldText.text = goldCollected.ToString();
        goldText.gameObject.SetActive(false);
        x2CoinText.text = "+" + goldCollected;
        confettiBlast.Play();
        confettiDrop.Play();
        StartCoroutine(DelayAnimation());
    }
    public void FillProgress(int currentValue, int bonusValue, int requireValue)
    {
        float currentPercent = currentValue * 1f / requireValue;
        battlepassProgressImg.fillAmount = currentPercent;
        battlepassProgressTxt.text = currentValue + "/" + requireValue;
        Action<ITween<float>> updateProgress = (t) =>
        {
            battlepassProgressTxt.text = (int)t.CurrentValue + "/" + requireValue;
            battlepassProgressImg.fillAmount = t.CurrentValue / requireValue; ;
        };
        Action<ITween<float>> completedProgress = (t) =>
        {
            battlepassProgressTxt.text = (currentValue + bonusValue) + "/" + requireValue;
            BattlePassDataController.Instance.CurrentBpExp += bonusValue;
            battlepassProgressImg.fillAmount = (currentValue + bonusValue) * 1f / requireValue;
        };
        TweenFactory.Tween("FillProgress", currentValue, currentValue + bonusValue, 2f, TweenScaleFunctions.Linear, updateProgress, completedProgress);
    }
    private IEnumerator DelayAnimation()
    {
        endPanelFrame.SetActive(false);
        videoRewardBtn.gameObject.SetActive(false);
        continueBtn.gameObject.SetActive(false);
        spineAnimationState = skeletonAnimation.AnimationState;
        var lastLevel = LevelDataController.Instance.currentLevel;
        bool canGetKey = (lastLevel.key - 1 == gameController.levelState);
        if (!canGetKey)
            spineAnimationState.SetAnimation(0, "start_1card", false);
        else
            spineAnimationState.SetAnimation(0, "start_2card", false);

        yield return new WaitForSeconds(0.2f);
        if (DataController.Instance.GetLevelState(1, 5) >= 1 && !DataController.Instance.IsFullPB())
        {
            piggyBankPanel.SetActive(true);
            pigAnim.AnimationState.SetAnimation(1, "jumpin_x", false);
            int bonusGoldPiggy = DataController.Instance.CurrentPbStorage / ((DataController.Instance.PbLevel * 2) + 3);
            bonusGoldPiggy += UnityEngine.Random.Range(-1, ((int)bonusGoldPiggy / 10) + 1);
            int averangeGold = DataController.Instance.CurrentPbStorage / ((((DataController.Instance.PbLevel * 2) + 3)) * 12);
            int tmp = 0;
            yield return new WaitForSeconds(1.3f);
            pigAnim.AnimationState.SetAnimation(1, "suckindiamond_x", false);
            yield return new WaitForSeconds(0.4f);
            var delay = new WaitForSeconds(0.03f);
            while (tmp < bonusGoldPiggy)
            {
                tmp += averangeGold;
                piggyGoldBonusTxt.text = "+" + tmp;
                AudioController.Instance.PlaySfx(IncreaseGoldAudio);
                yield return delay;

            }
            piggyGoldBonusTxt.text = "+" + bonusGoldPiggy;
            yield return new WaitForSeconds(0.8f);
            DataController.Instance.PbRuby += bonusGoldPiggy;
            if (DataController.Instance.IsFullPB() && DataController.Instance.PbTimeDuration <= 0)
            {
                DataController.Instance.PbTimeDuration = 7200;
                DataController.Instance.PbTimeStamp = DataController.ConvertToUnixTime(DateTime.Now);
                PlayerPrefs.SetInt("open_full_piggy", 0);
            }
            yield return new WaitForSeconds(0.7f);
            piggyBankPanel.SetActive(false);
        }

        endPanelFrame.SetActive(true);
        yield return new WaitForSeconds(0.35f);
        goldText.gameObject.SetActive(true);
        //yield return new WaitForSeconds(0.15f);

        //expText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        DataController.Instance.SetLevelState(lastLevel, lastLevel.chapter, lastLevel.id);
        bool canShowVideoReward = AdsController.Instance.IsRewardVideoAdsReady() && AdsController.Instance.CanShowAds(adsName);
        if (canShowVideoReward)
            APIController.Instance.LogEventShowAds(adsName);
        else
            continueBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 18);
        videoRewardBtn.gameObject.SetActive(canShowVideoReward);
        continueBtn.gameObject.SetActive(true);
        anim.Play("Appear");
        if (certiPanel.activeSelf)
        {
            StartCoroutine(IMove(cer_ComboTxt.transform.parent.gameObject));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(IMove(cer_LikeTxt.transform.parent.gameObject));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(IMove(cer_ServeTxt.transform.parent.gameObject));
        }
        yield return new WaitForSeconds(1.5f);
        if (!canGetKey)
            spineAnimationState.SetAnimation(0, "loop_1card", true);
        else
            spineAnimationState.SetAnimation(0, "loop_2card", true);
        //yield return new WaitForSeconds(0.4f);
        DataController.Instance.SaveData();

    }
    public void OnClickContinue()
    {
        gameController.LogFirstWinGSM(goldText.text, isClaimX2);
        if (AdsController.Instance.CanShowIntersAds(LevelDataController.Instance.currentLevel.chapter, LevelDataController.Instance.currentLevel.id, 1))
        {
            gameController.IsShowAds = true;
            AdsController.Instance.ShowInterstitial(() =>
            {
                if (isClick) return;
                isClick = true;
                SceneController.Instance.LoadScene("MainMenu");
            });
        }
        else
            SceneController.Instance.LoadScene("MainMenu");
        isClick = false;
    }
    public override void OnHide()
    {
        OnClickContinue();
    }
    IEnumerator DelayLoadMainmenu()
    {
        yield return new WaitForSeconds(0.4f);
        SceneController.Instance.LoadScene("MainMenu");
    }
    public void OnClickShowRewardedVideo()
    {
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
        AdsController.Instance.ResetTimeStamp();
        videoRewardBtn.interactable = false;
        gameController.IsShowAds = true;
    }
    public void OnCloseAds()
    {

    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        APIController.Instance.LogEventViewAds(adsName);
        isClaimX2 = true;
        StartCoroutine(DelayShowRewardAds());
    }
    private IEnumerator DelayShowRewardAds()
    {
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        videoRewardBtn.gameObject.SetActive(false);
        continueBtn.gameObject.SetActive(false);
        continueBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 18);
        yield return new WaitForSeconds(0.1f);
        int rewardGold = LevelDataController.Instance.collectedGold;
        LevelDataController.Instance.collectedGold += rewardGold;
        goldText.text = LevelDataController.Instance.collectedGold.ToString();
        goldTextPrefab.Spawn(goldText.transform.position, goldText.transform, "+" + rewardGold);
        continueBtn.gameObject.SetActive(true);
        anim.Play("Appear");
    }
    public IEnumerator IMove(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0;
        Vector2 targetPos = bookIcon.transform.position;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + targetPos.x) / 2f, (gameObject.transform.position.y + targetPos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(true);
        Vector2 vt = new Vector2(1, 1);
        while (Vector2.Distance(gameObject.transform.position, targetPos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, targetPos);
            time += Time.deltaTime * 2;
            vt.x = vt.y = (1 - (time / 2));
            gameObject.transform.localScale = vt;
            yield return null;
        }
        gameObject.SetActive(false);
        bookIcon.GetComponent<Animator>().Play("ObjectRotate");
    }

    public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t1;
        float tt = t1 * t1;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t1 * p1;
        p += tt * p2;
        return p;
    }

   /* public void FillProgress(int currentValue, int bonusValue, int requireValue)
    {
        float currentPercent = currentValue * 1f / requireValue;
        battlepassProgressImg.fillAmount = currentPercent;
        battlepassProgressTxt.text = currentValue + "/" + requireValue;
        Action<ITween<float>> updateProgress = (t) =>
        {
            battlepassProgressTxt.text = (int)t.CurrentValue + "/" + requireValue;
            battlepassProgressImg.fillAmount = t.CurrentValue / requireValue; ;
        };
        Action<ITween<float>> completedProgress = (t) =>
        {
            battlepassProgressTxt.text = (currentValue + bonusValue) + "/" + requireValue;
            BattlePassDataController.Instance.CurrentBpExp += bonusValue;
            battlepassProgressImg.fillAmount = (currentValue + bonusValue) * 1f / requireValue;
        };
        TweenFactory.Tween("FillProgress", currentValue, currentValue + bonusValue, 2f, TweenScaleFunctions.Linear, updateProgress, completedProgress);
    }*/
}
