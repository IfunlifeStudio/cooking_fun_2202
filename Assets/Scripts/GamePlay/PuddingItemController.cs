using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PuddingItemController : MonoBehaviour
{
    [SerializeField] private float itemDurationEffect;
    [SerializeField] private GamePlayBuyItemPanelController buyItemPanelController;
    [SerializeField] private Transform overlayCanvas;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private GameObject plusSign;
    private bool canInteract = true;
    // Start is called before the first frame update
    void Start()
    {
        bool isPuddingActive = DataController.Instance.IsItemUnlocked((int)ItemType.Pudding);
        int[] itemUnlocks = DataController.Instance.GetItemDataById((int)ItemType.Pudding).GetUnlockCondition();
        if (DataController.Instance.GetLevelState(itemUnlocks[0], itemUnlocks[1]) < 1
        && LevelDataController.Instance.currentLevel.chapter <= itemUnlocks[0]
        && LevelDataController.Instance.currentLevel.id < itemUnlocks[1]
        && !DataController.Instance.HasClaimFree((int)ItemType.Pudding))
            isPuddingActive = false;
        gameObject.SetActive(isPuddingActive);
        int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.Pudding);
        plusSign.SetActive(_itemQuantity == 0);
        itemQuantity.text = _itemQuantity.ToString();
        canInteract = true;
    }
    public void OnClickPudding()
    {
        if (!canInteract) return;
        if (DataController.Instance.IsItemUnlocked((int)ItemType.Pudding))
        {
            if (DataController.Instance.GetItemQuantity((int)ItemType.Pudding) > 0)
            {
                DataController.Instance.UseItem((int)ItemType.Pudding, 1);
                DataController.Instance.SaveData(false);
                CustomerFactory.Instance.OnPuddingActive();
                FindObjectOfType<GameController>().isPuddingActive = true;
                int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.Pudding);
                plusSign.SetActive(_itemQuantity == 0);
                itemQuantity.text = _itemQuantity.ToString();
                StartCoroutine(DoPuddingEffect());
            }
            else
            {
                Time.timeScale = 0;
                buyItemPanelController.Spawn((int)ItemType.Pudding, overlayCanvas, OnReturnFromBuyPanel);
            }
        }
    }
    public void OnReturnFromBuyPanel()
    {
        Time.timeScale = 1;
        if (DataController.Instance.GetItemQuantity((int)ItemType.Pudding) > 0)
        {
            DataController.Instance.UseItem((int)ItemType.Pudding, 1);
            DataController.Instance.SaveData(false);
            CustomerFactory.Instance.OnPuddingActive();
            FindObjectOfType<GameController>().isPuddingActive = true;
            int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.Pudding);
            plusSign.SetActive(_itemQuantity == 0);
            itemQuantity.text = _itemQuantity.ToString();
            StartCoroutine(DoPuddingEffect());
        }
    }
    private IEnumerator DoPuddingEffect()
    {
        canInteract = false;
        itemQuantity.transform.parent.gameObject.SetActive(false);
        plusSign.SetActive(false);
        yield return new WaitForSecondsRealtime(itemDurationEffect);
        canInteract = true;
        int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.Pudding);
        itemQuantity.transform.parent.gameObject.SetActive(true);
        itemQuantity.text = _itemQuantity.ToString();
        plusSign.SetActive(_itemQuantity == 0);
        CustomerFactory.Instance.OnPuddingDeactive();
        FindObjectOfType<GameController>().isPuddingActive = false;
    }
}
