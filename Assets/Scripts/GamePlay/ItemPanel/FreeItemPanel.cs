using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class FreeItemPanel : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite[] itemSprites;
    private int itemId;
    private bool isTouch;
    public void Init(int itemId)
    {
        GetComponent<Animator>().Play("Appear");
        UIController.Instance.PushUitoStack(this);
        Time.timeScale = 0;
        this.itemId = itemId;
        isTouch = false;
        AudioController.Instance.PlaySfx(popUpClip);
        string message = null;
        switch (itemId)
        {
            case 100100:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add3cus");
                break;
            case 100200:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_add20sec");
                break;
            case 100300:
                message = Lean.Localization.LeanLocalization.GetTranslationText("gp_item_second_chance");
                break;
        }
        this.description.text = message;
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
    }
    public void OnClickClaimFreeItem()
    {
        if (!isTouch)
        {
            isTouch = true;
            UIController.Instance.PopUiOutStack();
            DataController.Instance.AddItem(itemId, 3);
            DataController.Instance.ClaimFree(itemId);
            DataController.Instance.SaveData(false);
            StartCoroutine(HidePanel());
        }
    }
    public override void OnHide()
    {
        OnClickClaimFreeItem();
    }
    private IEnumerator HidePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSecondsRealtime(0.2f);
        FindObjectOfType<EndGameItemPanel>().UseItem(itemId);
        Destroy(gameObject);
    }
}
