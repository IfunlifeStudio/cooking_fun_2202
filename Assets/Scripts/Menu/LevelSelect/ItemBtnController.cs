using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ItemBtnController : MonoBehaviour
{
    [SerializeField] private int itemId;
    [SerializeField] private GameObject tick, recommendTag, plusIcon, helperHand, boostSelected;
    [SerializeField] private Sprite[] boostSprites;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private UnityEngine.Events.UnityEvent onClickBuy;
    [SerializeField] private Image boosterImage;
    [SerializeField] private ItemSelectTutorialPanel itemBuyTutorial, itemEquipTutorial;
    private bool isActive = false;
    private int quatity;
    private void OnEnable()
    {
        isActive = false;
        tick.SetActive(LevelDataController.Instance.CheckItemActive(itemId));
        boostSelected.SetActive(LevelDataController.Instance.CheckItemActive(itemId));
        quatity = DataController.Instance.GetItemQuantity(itemId);
        bool isBoosterUnlocked = DataController.Instance.IsItemUnlocked(itemId);
        itemCount.transform.parent.gameObject.SetActive(!LevelDataController.Instance.CheckItemActive(itemId) && isBoosterUnlocked);
        if (!isBoosterUnlocked)
        {
            boosterImage.sprite = boostSprites[0];
            plusIcon.SetActive(false);
            recommendTag.SetActive(false);
        }
        else
        {
            boosterImage.sprite = boostSprites[1];
            plusIcon.SetActive(quatity <= 0);
            int recommendItem = LevelDataController.Instance.GetRecommendItemId();
            recommendTag.SetActive(recommendItem == itemId);
            StartCoroutine(UpdateDisplay());
            if (quatity > 0)
            {
                itemCount.text = quatity.ToString();
            }
            if (PlayerPrefs.GetInt("item_equip_tut" + itemId, 0) == 1)//handle old data
            {
                if (!DataController.Instance.GetTutorialData().Contains(itemId))
                {
                    DataController.Instance.GetTutorialData().Add(itemId);
                }
                PlayerPrefs.SetInt("item_equip_tut" + itemId, 1);

            }
            if (PlayerPrefs.GetInt("item_buy_tut" + itemId, 0) == 1)//handle old data
            {
                if (!DataController.Instance.GetTutorialData().Contains(itemId + 1))
                {
                    DataController.Instance.GetTutorialData().Add(itemId + 1);
                }
                PlayerPrefs.SetInt("item_buy_tut" + itemId, 1);
            }
            if (!DataController.Instance.GetTutorialData().Contains(itemId + 1))
            {
                if (itemBuyTutorial != null)
                    itemBuyTutorial.Spawn(OnClick);
                DataController.Instance.GetTutorialData().Add(itemId + 1);
                return;
            }
            if (!DataController.Instance.GetTutorialData().Contains(itemId) && DataController.Instance.GetTutorialData().Contains(itemId + 1))
            {
                if (itemEquipTutorial != null)
                {
                    itemEquipTutorial.Spawn(OnClick);
                    FindObjectOfType<MainMenuController>().setIndexTab(5);
                }
                DataController.Instance.GetTutorialData().Add(itemId);
            }
            DataController.Instance.SaveData(false);
        }

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (helperHand != null && helperHand.activeInHierarchy)
                helperHand.SetActive(false);
    }
    private IEnumerator UpdateDisplay()
    {
        var delay = new WaitForSecondsRealtime(0.1f);
        while (true)
        {
            string itemCountText = "+";
            quatity = DataController.Instance.GetItemQuantity(itemId);
            if (quatity > 0)
            {
                itemCountText = quatity.ToString();
                plusIcon.SetActive(false);
            }
            else plusIcon.SetActive(true);
            itemCount.text = itemCountText;
            int recommendItem = LevelDataController.Instance.GetRecommendItemId();
            recommendTag.SetActive(recommendItem == itemId);
            yield return delay;
        }
    }
    public void OnClick()
    {
        isActive = LevelDataController.Instance.CheckItemActive(itemId);
        isActive = !isActive;
        if (quatity < 1 || !DataController.Instance.GetTutorialData().Contains(itemId))
            isActive = false;
        //tick.SetActive(isActive);
        bool isBoosterUnlocked = DataController.Instance.IsItemUnlocked(itemId);
        if (isBoosterUnlocked)
        {
            itemCount.transform.parent.gameObject.SetActive(!isActive);
            tick.SetActive(isActive);
            boostSelected.SetActive(isActive);
            if (isActive)
            {
                LevelDataController.Instance.AddItem(itemId);

            }
            else
            {
                bool hasShowTut = DataController.Instance.GetTutorialData().Contains(itemId);
                if (quatity < 1 || !hasShowTut)
                {
                    onClickBuy.Invoke();
                    if (!hasShowTut)
                        FindObjectOfType<ShopController>().onShopClose = ShowEquipTutorial;
                    FindObjectOfType<MainMenuController>().setIndexTab(5);
                    return;
                }
                LevelDataController.Instance.RemoveItem(itemId);
            }
        }
        else
        {
            GetComponent<TooltipController>().InitWithMessage(DataController.Instance.GetItemUnlockMessage(itemId));
        }

    }
    public void OnSelectLevelChange()
    {
        if (isActive)
        {
            isActive = false;
            LevelDataController.Instance.RemoveItem(itemId);
            itemCount.transform.parent.gameObject.SetActive(!isActive);
            tick.SetActive(isActive);
            boostSelected.SetActive(isActive);
        }
    }
    public void ShowHelperHandTutorial()
    {
        if (helperHand != null)
            helperHand.SetActive(recommendTag.activeInHierarchy);
    }
    private void ShowEquipTutorial()
    {
        if (itemEquipTutorial != null)
        {
            itemEquipTutorial.Spawn(OnClick);
            FindObjectOfType<MainMenuController>().setIndexTab(5);
        }
        DataController.Instance.GetTutorialData().Add(itemId);
        DataController.Instance.SaveData(false);
    }
    public int ItemId
    {
        get { return itemId; }
    }
}
