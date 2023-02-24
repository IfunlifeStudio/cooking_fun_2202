using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EndGameItemPanel : MonoBehaviour
{
    [SerializeField] private string adsName;
    [SerializeField] private UseItemPanel useItemPanelPrefab;
    [SerializeField] private BuyOrWatchAdsItemPanel buyOrWatchPanelPrefab;
    [SerializeField] private GamePlayBuyItemPanelController buyItemPanelPrefab;
    [SerializeField] private FreeItemPanel freeItemPrefab;
    public bool hasWatchAds = false;
    public void Init(int itemId)
    {
        bool canShowAdsName = AdsController.Instance.CanShowAds(adsName);
        int itemQuantity = DataController.Instance.GetItemQuantity(itemId);
        if (!DataController.Instance.HasClaimFree(itemId))
        {
            FreeItemPanel freeItemPanel = Instantiate(freeItemPrefab, transform.parent);
            freeItemPanel.Init(itemId);
        }
        else
        {
            if (canShowAdsName)
            {
                SpawnBuyOrWatchAdsClaimItemPanel(itemId);
            }
            else
            {
                if ((itemQuantity > 0))
                    UseItem(itemId);
                else
                    SpawnBuyItemPanel(itemId);
            }
        }
    }
    public void SpawnBuyItemPanel( int itemId)
    {
        GamePlayBuyItemPanelController buyItemPanel =  Instantiate(buyItemPanelPrefab, transform.parent);
        buyItemPanel.Init(itemId);
    }
    public void SpawnBuyOrWatchAdsClaimItemPanel(int itemId)
    {
        BuyOrWatchAdsItemPanel buyItemPanel = Instantiate(buyOrWatchPanelPrefab, transform.parent);
        buyItemPanel.Init(itemId);
    }
    public void UseItem(int itemId)
    {
        UseItemPanel useItemPanel = Instantiate(useItemPanelPrefab, transform.parent);
        useItemPanel.Init(itemId);
    }
}
