using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DailyBtnController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayLabelText, quantityLabelText;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Sprite[] backgroundImages;
    [SerializeField] private Image itemSprite, highlightSprite, tickIconSprite, panelSprite;
    private UnityAction onCloseCallback;
    bool canDoubleReward;
    int itemId;
    int itemQuantity = 0;
    int goldQuantity = 0;
    int rubyQuantity = 0;
    int time = 0;
    bool canClamed;
    public void Init(int itemId, int quantity, int date, int ReceivedProgress, bool canDouble, UnityAction onCloseCallback = null)
    {

        dayLabelText.text = Lean.Localization.LeanLocalization.GetTranslationText("daily_reward_day") + date;
        itemSprite.sprite = itemSprites[GetItemIndex(itemId, quantity)];
        highlightSprite.gameObject.SetActive(ReceivedProgress + 1 == date);
        tickIconSprite.gameObject.SetActive(date < ReceivedProgress + 1);
        if (ReceivedProgress + 1 == date)
        {
            panelSprite.sprite = backgroundImages[1];
            canClamed = true;
        }
        else
        {
            if (date == 7 || date == 3)
                panelSprite.sprite = backgroundImages[2];
            else
                panelSprite.sprite = backgroundImages[0];
        }
        if ((date == 7 || date == 3))
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 335);
            if (itemId == 100500)
            {
                itemSprite.sprite = itemSprites[12];
            }
        }
        quantityLabelText.gameObject.SetActive(date >= ReceivedProgress + 1);
        quantityLabelText.text ="x"+ quantity.ToString();
        canDoubleReward = canDouble;
        this.onCloseCallback = onCloseCallback;
    }
    public int GetItemIndex(int itemId, int quantity)
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
        }
        this.itemId = 100400;
        return 0;
    }
    public void OnClick()
    {
        if (canClamed)
        {
            int received_progress = PlayerPrefs.GetInt("received_progress", 0);
            PlayerPrefs.SetInt("received_progress", received_progress + 1);
            if ((received_progress + 1) % 7 == 0)
            {
                PlayerPrefs.SetInt("received_progress", 0);
                int weeklyRewardProgress = PlayerPrefs.GetInt("weekly_reward_progress", 0);
                PlayerPrefs.SetInt("weekly_reward_progress", weeklyRewardProgress + 1);
            }
            PlayerPrefs.SetInt("last_received_date", System.DateTime.Now.DayOfYear);
            onCloseCallback?.Invoke();
            if (itemQuantity != 0)
                FindObjectOfType<RewardPanelController>().Init(goldQuantity, rubyQuantity, time, new int[1] { itemId }, new int[1] { itemQuantity }, canDoubleReward, "x2Daily", null);
            else
                FindObjectOfType<RewardPanelController>().Init(goldQuantity, rubyQuantity, time, new int[0] { }, new int[0] { }, canDoubleReward, "x2Daily", null);
        }
    }
}
