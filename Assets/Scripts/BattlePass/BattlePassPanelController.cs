using DG.Tweening;
using DigitalRuby.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassPanelController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ChristmassTreeController christmassTree;
    [SerializeField] private BattlePassProgressController battlePassProgress;
    [SerializeField] private BattlePassScrollViewController bpScrollview;
    [SerializeField] private Button decorBtn, activatebtn;
    [SerializeField] private Sprite notDecorSrpite;
    [SerializeField] private TextMeshProUGUI countdownTxt;
    [SerializeField] private BattlePassTutorialController bpTut;
    [SerializeField] private AudioClip levelupAudio;
    private BattlePassElementData nextBpElementData;
    bool canDecor = false;
    // Start is called before the first frame update
    private void Start()
    {
        activatebtn.gameObject.SetActive(!BattlePassDataController.Instance.HasUnlockVip2022);
        HandleData();
        if (PlayerPrefs.GetInt("battlepass_show_tut", 0) == 0)
            OnShowTut();
        StartCoroutine(CoundowntBattlePassTime());
    }
    public void HandleData()
    {
        canDecor = true;
        int bpLevel = BattlePassDataController.Instance.CurrentBpLevel;
        int nextBpLevel = Mathf.Min(bpLevel + 1, BattlePassDataController.BP_MAX_LEVEL);
        if (BattlePassDataController.Instance.CurrentBpLevel == BattlePassDataController.BP_MAX_LEVEL)
            decorBtn.gameObject.SetActive(false);
        nextBpElementData = BattlePassDataController.Instance.GetBattlePassElementData(nextBpLevel);
        battlePassProgress.HandleData(nextBpElementData, BattlePassDataController.Instance.CurrentBpLevel, BattlePassDataController.Instance.CurrentBpExp);
        bpScrollview.Init(BattlePassDataController.Instance.battlePassLevelData.BattlePassElementDatas, bpLevel);
        if (BattlePassDataController.Instance.CurrentBpExp < nextBpElementData.Exp || bpLevel >= BattlePassDataController.BP_MAX_LEVEL)
        {
            decorBtn.image.sprite = notDecorSrpite;
            canDecor = false;
        }
        else if (BattlePassDataController.Instance.CurrentBpLevel == BattlePassDataController.BP_MAX_LEVEL)
        {
            decorBtn.gameObject.SetActive(false);
        }
        christmassTree.InitTree(bpLevel);
    }
    IEnumerator CoundowntBattlePassTime()
    {
        DateTime endTime = BattlePassDataController.EndTime;
        TimeSpan timeSpan;
        var delay = new WaitForSeconds(60f);
        timeSpan = endTime - DateTime.UtcNow;
        while (timeSpan.TotalSeconds > 0)
        {
            timeSpan = endTime - DateTime.UtcNow;
            countdownTxt.text = String.Format("{0}d {1}h {2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            yield return delay;
        }
    }
    public void OnClickDecor()
    {
        if (canDecor)
        {
            christmassTree.OnLevelUp(nextBpElementData.Level);
            BattlePassDataController.Instance.CurrentBpLevel++;
            BattlePassDataController.Instance.CurrentBpExp -= nextBpElementData.Exp;
            AudioController.Instance.PlaySfx(levelupAudio);
            HandleData();
            DataController.Instance.SaveData(false);
        }
    }
    public void OnShowTut()
    {
        PlayerPrefs.SetInt("battlepass_show_tut", 1);
        Instantiate(bpTut, transform);
    }
    public void OnClickPlay()
    {
        OnClickClose();
        DOVirtual.DelayedCall(0.4f, () =>
        {
            FindObjectOfType<MainMenuController>().OnOpenMaxResInZone();
        });
    }
    public void OnClickClose()
    {
        animator.Play("Disappear");
        Destroy(gameObject, 0.4f);
    }
    public void OnClickActivate()
    {
        FindObjectOfType<XmasPassController>().OnClickOpenXmasPass();
        APIController.Instance.LogEventClickBuyBP("button_active", BattlePassDataController.Instance.CurrentBpLevel.ToString());
        OnClickClose();
    }
    public void TweenObject(RectTransform go, Vector3 startPos, Vector3 endPos)
    {
        System.Action<ITween<Vector3>> updateFoodProp = (t) =>
        {
            go.localPosition = t.CurrentValue;
        };

        System.Action<ITween<Vector3>> completedPropMovement = (t) =>
        {
            go.localPosition = endPos;
        };
        TweenFactory.Tween("TweenObjBP" + Time.time, startPos, endPos, 2f, TweenScaleFunctions.Linear, updateFoodProp, null);
    }
}
