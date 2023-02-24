using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using DigitalRuby.Tween;
public class RewardPanelController : UIView
{
    public string adsName = "";
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Transform shopIcon, topRewardLayer, bottomRewardLayer;
    [SerializeField] private Button claimBtn, claim1Btn, videoRewardBtn;
    [SerializeField] private RewardItemController itemReward;
    [SerializeField] private GameObject itemEffectPrefabs;
    [SerializeField] private AudioClip popUpClip;
    private UnityAction onCloseCallback;
    bool canClaimReward;
    int multiple = 1;
    List<GameObject> goList;
    List<Items> tmpItemIds;
    public void Init(int _goldAmount, int _rubyAmount, int _unlimitEnergyTime, int[] _itemIds, int[] _itemAmounts, bool canDouble = false, string adsName = null, UnityAction closeCallBack = null)//maximun display 3 rewards
    {
        transform.SetAsLastSibling();
        multiple = 1;
        tmpItemIds = new List<Items>();
        goList = new List<GameObject>();
        canClaimReward = true;
        FindObjectOfType<MainMenuController>().setIndexTab(5);
        AudioController.Instance.PlaySfx(popUpClip);
        this.onCloseCallback = closeCallBack;
        if (_goldAmount > 0)
            tmpItemIds.Add(new Items(100400, _goldAmount));
        if (_rubyAmount > 0)
            tmpItemIds.Add(new Items(100500, _rubyAmount));
        if (_unlimitEnergyTime > 0)
            tmpItemIds.Add(new Items(100600, _unlimitEnergyTime));
        if (_itemIds.Length > 0)
            for (int i = 0; i < _itemIds.Length; i++)
                tmpItemIds.Add(new Items(_itemIds[i], _itemAmounts[i]));
        if (tmpItemIds.Count > 3)
        {
            bottomRewardLayer.gameObject.SetActive(true);
            int idHaflLeftList = tmpItemIds.Count / 2;
            for (int i = 0; i < idHaflLeftList; i++)
            {
                var go = Instantiate(itemReward, topRewardLayer);
                go.Init(tmpItemIds[i].itemId, tmpItemIds[i].itemQuantity);
                goList.Add(go.gameObject);
            }
            for (int i = idHaflLeftList; i < tmpItemIds.Count; i++)
            {
                var go = Instantiate(itemReward, bottomRewardLayer);
                go.Init(tmpItemIds[i].itemId, tmpItemIds[i].itemQuantity);
                goList.Add(go.gameObject);
            }
        }
        else
        {
            bottomRewardLayer.gameObject.SetActive(false);
            for (int i = 0; i < tmpItemIds.Count; i++)
            {
                var go = Instantiate(itemReward, topRewardLayer);
                go.Init(tmpItemIds[i].itemId, tmpItemIds[i].itemQuantity);
                goList.Add(go.gameObject);
            }
        }

        this.adsName = adsName;
        bool canShowVideo = canDouble && AdsController.Instance.IsRewardVideoAdsReady();
        claimBtn.gameObject.SetActive(!canShowVideo);
        claim1Btn.gameObject.SetActive(canShowVideo);
        videoRewardBtn.gameObject.SetActive(canShowVideo);
        videoRewardBtn.interactable = canShowVideo;
        rewardPanel.SetActive(true);
        rewardPanel.GetComponent<Animator>().Play("Appear");
        if (canShowVideo)
        {
            APIController.Instance.LogEventShowAds(adsName);
        }
        UIController.Instance.PushUitoStack(this);
    }
    public void OnClickWatchAds()
    {
        AdsController.Instance.ShowVideoReward(OnEarnReward, OnCloseAds);
    }
    public void OnCloseAds()
    {

    }
    public void OnEarnReward()
    {
        APIController.Instance.LogEventRewarded();
        StartCoroutine(DelayRewardPlayer());
        APIController.Instance.LogEventViewAds(adsName);
    }
    private IEnumerator DelayRewardPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        multiple = 2;
        AdsController.Instance.OnWatchAdsCompleted(adsName);
        OnClickClaim();
    }
    public void OnClickClaim()
    {
        if (canClaimReward)
        {
            int ingameItemQuantity = 0;
            canClaimReward = false;
            for (int i = 0; i < tmpItemIds.Count; i++)
            {
                if (tmpItemIds[i].itemId == 100400)
                    IncreaseGold(tmpItemIds[i].itemQuantity);
                else if (tmpItemIds[i].itemId == 100500)
                    IncreaseRuby(tmpItemIds[i].itemQuantity);
                else if (tmpItemIds[i].itemId == 100600)
                    IncreaseEnergy(tmpItemIds[i].itemQuantity);
                else if (tmpItemIds[i].itemId == 2022)
                    DataController.Instance.AddAvatarToCollection(2022);
                else if (tmpItemIds[i].itemId == 320000)
                {
                    DataController.Instance.AddCustomerSkin(320000);
                    PlayerPrefs.SetInt("showoff_skin_2022", 1);
                }
                else
                {
                    if (tmpItemIds[i].itemId <= 9)
                        DataController.Instance.GetGameData().AddBorderId(tmpItemIds[i].itemId);
                    else
                        InCreaseItem(tmpItemIds[i].itemId, tmpItemIds[i].itemQuantity);
                    ingameItemQuantity++;
                }
            }
            ingameItemQuantity = Mathf.Min(5, ingameItemQuantity);
            for (int i = 0; i < ingameItemQuantity; i++)
            {
                GameObject itemProp = Instantiate(itemEffectPrefabs, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, transform.parent);
                StartCoroutine(IMove(itemProp, shopIcon.transform.position, 1));

            }
            DataController.Instance.SaveData();
            Claim();
        }
    }
    public void IncreaseGold(int gold)
    {
        gold *= multiple;
        APIController.Instance.LogEventEarnGold(gold, "reward");
        FindObjectOfType<MainMenuController>().IncreaseCoin(transform.position, gold);
        DataController.Instance.Gold += gold;
    }
    public void IncreaseRuby(int ruby)
    {
        ruby *= multiple;
        APIController.Instance.LogEventEarnRuby(ruby, "reward");
        FindObjectOfType<MainMenuController>().IncreaseGem(transform.position, ruby);
        DataController.Instance.Ruby += ruby;
    }
    public void IncreaseEnergy(int time)
    {
        time *= multiple;
        FindObjectOfType<EnergyController>().StopAllCoroutines();
        DataController.Instance.AddUnlimitedEnergy(time);
        FindObjectOfType<EnergyController>().PlayUnlimitedEnergyEffect();
    }
    public void InCreaseItem(int itemId, int quantity)
    {
        quantity *= multiple;
        DataController.Instance.AddItem(itemId, quantity);
    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f + (Random.Range(-6f, 0f)), (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 2;
            yield return null;
        }
        DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
        {
            Destroy(gameObject);
        });
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
    private void Claim()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            Destroy(goList[i]);
        }
        goList.Clear();

        HidePopup();
        if (onCloseCallback != null) onCloseCallback.Invoke();
    }
    IEnumerator DelayHide()
    {
        UIController.Instance.PopUiOutStack();
        rewardPanel.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        rewardPanel.SetActive(false);
    }
    void HidePopup()
    {
        StartCoroutine(DelayHide());
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        if (canClaimReward)
            OnClickClaim();
        else
            HidePopup();
    }
}
