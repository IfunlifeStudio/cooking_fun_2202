using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdsRewardBtnController : MonoBehaviour
{
    [SerializeField] Image rewardImage;
    [SerializeField] Sprite[] rewardSprites;
    [SerializeField] GameObject blur,tick;
    [SerializeField] TextMeshProUGUI itemQuantityText;
    bool canClaim;
    int itemId, itemQuantity = 0, goldQuantity = 0,rubyQuantity = 0,time=0;
    UnityAction onClosedCallback;
    // Start is called before the first frame update
    public void Init(int itemId, int quantity, bool canClaim)
    {
        this.canClaim = canClaim;
        tick.SetActive(canClaim);
        itemQuantityText.gameObject.SetActive(!canClaim);
        itemQuantityText.text = quantity.ToString();
        rewardImage.sprite = rewardSprites[GetIndexSprite(itemId,quantity)];
    }

    public int GetIndexSprite(int itemId, int quantity)
    {
        switch (itemId)
        {
            case 100400:
                this.itemId = itemId;
                goldQuantity = quantity;
                return 0;
            case 100500:
                this.itemId = itemId;
                rubyQuantity = quantity;
                return 1;
            case 100600:
                this.itemId = itemId;
                time = quantity;
                return 2;
            case 102000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 3;
            case 101000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 4;
            case 100000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 5;
            case 110000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 6;
            case 120000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 7;
            case 130000:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 8;
            case 100100:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 9;
            case 100200:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 10;
            case 100300:
                this.itemId = itemId;
                itemQuantity = quantity;
                return 11;
            default:
                this.itemId = 100400;
                goldQuantity = 100;
                return 0;
        }  
    }
    public void GetReward()
    {
        if (itemQuantity != 0)
            FindObjectOfType<RewardPanelController>().Init(goldQuantity, rubyQuantity, time, new int[1] { itemId }, new int[1] { itemQuantity }, false, null, null);
        else
            FindObjectOfType<RewardPanelController>().Init(goldQuantity, rubyQuantity, time, new int[0] { }, new int[0] { }, false, null, null);
    }
}
