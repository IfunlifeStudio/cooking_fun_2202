using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GamePlayItemBtn : MonoBehaviour
{
    [SerializeField] private int itemId;
    [SerializeField] private GameObject tick, recommendTag, plusIcon;
    [SerializeField] private Sprite[] boostSprites;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private UnityEngine.Events.UnityEvent onClickBuy;
    [SerializeField] private Image boosterImage;
    [SerializeField] private TooltipController tipPanel;
    private bool isActive = false, activeFromMenu;
    private int quatity;
    private void OnEnable()
    {
        activeFromMenu = LevelDataController.Instance.CheckItemActive(itemId);
        isActive = false;
        tick.SetActive(false);
        quatity = DataController.Instance.GetItemQuantity(itemId);
        bool isBoosterUnlocked = DataController.Instance.IsItemUnlocked(itemId);
        if (activeFromMenu)
        {
            tick.SetActive(true);
            plusIcon.SetActive(false);
            recommendTag.SetActive(false);
            itemCount.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            if (!isBoosterUnlocked)
            {
                boosterImage.sprite = boostSprites[0];
                itemCount.transform.parent.gameObject.SetActive(false);
                plusIcon.SetActive(false);
                recommendTag.SetActive(false);
            }
            else
            {
                boosterImage.sprite = boostSprites[1];
                itemCount.transform.parent.gameObject.SetActive(true);
                if (quatity > 0)
                {
                    itemCount.text = quatity.ToString();
                    plusIcon.SetActive(false);
                }
                else plusIcon.SetActive(true);
                int recommendItem = LevelDataController.Instance.GetRecommendItemId();
                recommendTag.SetActive(recommendItem == itemId);
                StartCoroutine(UpdateDisplay());
            }
        }
    }
    private IEnumerator UpdateDisplay()
    {
        var delay = new WaitForSecondsRealtime(1);
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
        if (activeFromMenu) return;
        isActive = !isActive;
        if (quatity < 1)
            isActive = false;
        //tick.SetActive(isActive);
        bool isBoosterUnlocked = DataController.Instance.IsItemUnlocked(itemId);
        if (isBoosterUnlocked)
        {
            if (isActive)
            {
                FindObjectOfType<PausePanelController>().AddItem(itemId);
                itemCount.transform.parent.gameObject.SetActive(false);
                tick.SetActive(true);

            }
            else
            {
                if (quatity < 1)
                    onClickBuy.Invoke();
                FindObjectOfType<PausePanelController>().RemoveItem(itemId);
                itemCount.transform.parent.gameObject.SetActive(true);
                tick.SetActive(false);
            }
        }
        else
            tipPanel.InitWithMessage(DataController.Instance.GetItemUnlockMessage(itemId));
    }
}
