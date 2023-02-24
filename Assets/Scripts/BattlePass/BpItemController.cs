using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BpItemController : MonoBehaviour, IBattlePassItem
{
    private int level;
    private int itemId;
    private int itemQuantity;
    [SerializeField] private CanvasGroup bgCanvasgroup;
    [SerializeField] private Image itemImg;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private TextMeshProUGUI quantityTxt;
    [SerializeField] private Button claimBtn;
    [SerializeField] private GameObject tickGo, lockBtn;
    [SerializeField] private TooltipController tooltipController;
    public void SetupUi(BattlePassItemData bpData, int level, bool canClaim, bool hasClaimed)
    {
        this.level = level;
        this.itemId = bpData.ItemId;
        this.itemQuantity = bpData.ItemQuantity;
        quantityTxt.text = "x" + itemQuantity;
        //claimBtn.interactable = (canClaim && !hasClaimed);
        float alphaRatio = hasClaimed ? 0.6f : 1;
        bgCanvasgroup.alpha = alphaRatio;
        claimBtn.gameObject.SetActive(canClaim && !hasClaimed);
        tickGo.SetActive(hasClaimed);
        lockBtn.SetActive(!canClaim);
        itemImg.sprite = itemSprites[GetItemSpriteIndex(itemId)];
    }
    public int GetItemSpriteIndex(int itemId)
    {
        switch (itemId)
        {
            case 100400://gold
                return 0;
            case 100500://diamond
                return 1;
            case 100600://unlimit-energy
                return 2;
            case 102000://ice-cream
                return 3;
            case 101000://pudding
                return 4;
            case 100000://anti-overcook
                return 5;
            case 110000://double coin
                return 6;
            case 120000://instant-cook
                return 7;
            case 130000://auto-serve
                return 8;
            case 100100://add 3 customer
                return 9;
            case 100200://add more time
                return 10;
            case 100300:// second chance
                return 11;
            case 320000: //skin
                return 12;
            case 2022: //avatar
                return 13;
        }
        return 0;
    }

    public void OnClaim()
    {
        tickGo.SetActive(true);
        claimBtn.gameObject.SetActive(false);
        bgCanvasgroup.alpha = 0.6f;
        BattlePassDataController.Instance.ClaimFreeBpIttem(level);
        DataController.Instance.SaveData(false);
        FindObjectOfType<RewardPanelController>().Init(0, 0, 0, new int[] { itemId }, new int[] { itemQuantity }, false);
        APIController.Instance.LogEventClaimBpReward("free_" + BattlePassDataController.Instance.CurrentBpLevel.ToString());
    }
    public void OnClickShowVipMessage()
    {
        tooltipController.InitWithMessage(Lean.Localization.LeanLocalization.GetTranslationText("bp_tooltip_vip_lock"));
        transform.SetAsLastSibling();
    }
    public void OnClickShowLockMessage()
    {
        tooltipController.InitWithMessage(Lean.Localization.LeanLocalization.GetTranslationText("bp_tooltip_lock"));
        transform.SetAsLastSibling();
    }
    public void OnClickShowClaimMessage()
    {
        tooltipController.InitWithMessage(Lean.Localization.LeanLocalization.GetTranslationText("bp_tooltip_hasclaimed"));
        transform.SetAsLastSibling();
    }
}
