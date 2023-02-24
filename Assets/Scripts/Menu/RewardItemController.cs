using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardItemController : MonoBehaviour
{
    public Image itemImage;
    [SerializeField] private TextMeshProUGUI itemQuantityTxt;
    [SerializeField] private Sprite[] itemSprites;
    public void Init(int itemId, int quantity)
    {
        itemImage.sprite = itemSprites[GetItemIndex(itemId)];
        itemQuantityTxt.text = "x" + quantity;
    }
    public int GetItemIndex(int itemId)
    {
        switch (itemId)
        {
            case 100400:
                return 0;
            case 100500:
                return 1;
            case 100600:
                return 2;
            case 102000:
                return 3;
            case 101000:
                return 4;
            case 100000:
                return 5;
            case 110000:
                return 6;
            case 120000:
                return 7;
            case 130000:
                return 8;
            case 100100:
                return 9;
            case 100200:
                return 10;
            case 100300:
                return 11;
            case 4:
                return 12;
            case 5:
                return 13;
            case 6:
                return 14;
            case 7:
                return 15;
            case 8:
                return 16;
            case 9:
                return 16;
            case 2022:
                return 17;
            case 320000:
                return 18;


        }
        return 0;
    }
}
