using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UseItemPanel : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private TextMeshProUGUI tittle, itemQuantity, reasonText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite[] itemSprites;
    private int itemId;
    private bool isTouch;
    public void Init(int itemId)
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
        Time.timeScale = 0;
        isTouch = false;
        AudioController.Instance.PlaySfx(popUpClip);
        this.itemId = itemId;
        string message = null;
        switch (itemId)
        {
            case 100100:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add3cus");
                reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add3cus");
                break;
            case 100200:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add20sec");
                reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_add20sec");
                break;
            case 100300:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_second_chance");
                if (LevelDataController.Instance.currentLevel.ConditionType() == 1)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_burn");

                }
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 2)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_threw");
                }
                else if (LevelDataController.Instance.currentLevel.ConditionType() == 3)
                {
                    reasonText.text = Lean.Localization.LeanLocalization.GetTranslationText("reason_item_cusleft");
                }
                break;
        }
        this.tittle.text = message;
        switch (itemId)
        {
            case 100100:
                itemIcon.sprite = itemSprites[0];
                break;
            case 100200:
                itemIcon.sprite = itemSprites[1];
                break;
            case 100300:
                itemIcon.sprite = itemSprites[2];
                break;
        }
        itemQuantity.text = DataController.Instance.GetItemQuantity(itemId).ToString();
    }
    public void OnClickUseItem()
    {
        if (!isTouch)
        {
            isTouch = true;
            GameController gameController = FindObjectOfType<GameController>();
            DataController.Instance.UseItem(itemId, 1);
            DataController.Instance.SaveData(false);
            switch (itemId)
            {
                case 100100:
                    gameController.OnClickAddCustomer(3);
                    break;
                case 100200:
                    gameController.OnClickAddSeconds(20);
                    break;
                case 100300:
                    gameController.OnClickSecondChance();
                    break;
            }
            string itemName = DataController.Instance.GetItemName(itemId);
            string para_leveldata = gameController.levelData.chapter + "_" + gameController.levelData.id + "_" + gameController.levelState;
            APIController.Instance.LogEventUseItem(itemName, para_leveldata);
            HidePanel();
            isTouch = false;
        }
    }
    public void OnClickClose()
    {
        UIController.Instance.PopUiOutStack();
        GameController gameController = FindObjectOfType<GameController>();
        gameController.OnPlayerCancelItemPanel();
        HidePanel();
    }
    public override void OnHide()
    {
        OnClickClose();
    }
    private void HidePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
}
