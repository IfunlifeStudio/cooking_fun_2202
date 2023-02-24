using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class RestaurantUnlockPanel : UIView
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Image resImage;
    [SerializeField] private Sprite[] resSprites;
    [SerializeField] private TextMeshProUGUI resName, unlockCondition;
    private int chapter;
    public void Init(int chapter)
    {
        UIController.Instance.PushUitoStack(this);
        this.chapter = chapter;
        RestaurantData restaurant;
        GetComponent<Animator>().Play("Appear");
        AudioController.Instance.PlaySfx(popUpClip);
        if (chapter <= 10)
        {
            resImage.sprite = resSprites[chapter - 2];
            restaurant = DataController.Instance.GetRestaurantById(chapter);
            resName.text = restaurant.restaurantName;
            unlockCondition.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("res_unlock_message", "You have {0}/{1} keys."), DataController.Instance.GetTotalKeyGranted(), restaurant.keyRequired);
        }
        else if (chapter <= 13)
        {
            restaurant = DataController.Instance.GetRestaurantById(chapter);
            resName.text = restaurant.restaurantName;
            unlockCondition.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("res_unlock_message", "You have {0}/{1} keys."), DataController.Instance.GetTotalKeyGranted(), restaurant.keyRequired);
            resImage.sprite = resSprites[chapter - 3];
        }
        else if (chapter <= 17)
        {
            restaurant = DataController.Instance.GetRestaurantById(chapter);
            resName.text = restaurant.restaurantName;
            unlockCondition.text = string.Format(Lean.Localization.LeanLocalization.GetTranslationText("res_unlock_message", "You have {0}/{1} keys."), DataController.Instance.GetTotalKeyGranted(), restaurant.keyRequired);
            resImage.sprite = resSprites[chapter - 4];
        }
        else
        {
            resName.text = Lean.Localization.LeanLocalization.GetTranslationText("res_comming_soon", "Coming soon");
            unlockCondition.text = "";
        }
    }
    public void OnClickClose()
    {
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.25f);
    }
    public override void OnHide()
    {
        OnClickClose();
    }
    public void OnClickFind()
    {
        GetComponent<Animator>().Play("Disappear");
        FindObjectOfType<MainMenuController>().OpenLastestRestaurant();
        Destroy(gameObject, 0.25f);
    }
}
