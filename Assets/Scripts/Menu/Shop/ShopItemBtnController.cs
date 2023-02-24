using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.UI;
public class ShopItemBtnController : MonoBehaviour
{
    [SerializeField] private int itemId;
    [SerializeField] private Transform itemIcon;
    [SerializeField] private TextMeshProUGUI itemCount, costText;
    [SerializeField] private GameObject itemProp, rubyIcon;
    [SerializeField] private TooltipController tooltip;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private bool canClaimFree;
    private int currentItemQuantity = 0;
    private float timeStamp;
    private bool isOnclicked;
    [SerializeField] private FreeItemTutorialPanel freeItemTutorialPanel;
    Vector3 originScale;
    // Start is called before the first frame update
    void OnEnable()
    {
        originScale = transform.localScale;
        itemCount.text = DataController.Instance.GetItemQuantity(itemId).ToString();
        if (DataController.Instance.IsItemUnlocked(itemId))//only init item icon after item is unlocked
        {
            buyBtn.interactable = true;
            itemIcon.GetComponent<Image>().sprite = itemSprites[1];
            if (DataController.Instance.HasClaimFree(itemId) || !canClaimFree)
                costText.text = DataController.Instance.GetItemCost(itemId).ToString();
            else if (!DataController.Instance.HasClaimFree(itemId) && canClaimFree)
            {
                costText.text = "Free";
                rubyIcon.SetActive(false);
                LevelData currentLevel = LevelDataController.Instance.currentLevel;
                ItemData itemData = DataController.Instance.GetItemDataById(itemId);
                if (currentLevel.chapter == itemData.GetUnlockCondition()[0] && currentLevel.id == itemData.GetUnlockCondition()[1])
                {
                    Transform overlayCanvas = GameObject.Find("CanvasOverlay").transform;
                    if (itemId == (int)ItemType.Instant_Cook || itemId==(int)ItemType.AutoServe)
                        transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 110);
                    freeItemTutorialPanel.Spawn(itemId, overlayCanvas, OnClickBuy);
                }
            }
        }
        else
        {
            buyBtn.interactable = false;
            itemIcon.GetComponent<Image>().sprite = itemSprites[0];
        }
    }
    private void Update()
    {
        float deltaTime = Time.time - timeStamp;
        if (deltaTime < 0.1f)
        {
            float curentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            itemIcon.localScale = new Vector3(curentScale, curentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
                itemIcon.localScale = Vector3.one;
        }
    }
    public void OnClickInfo()
    {
        if (DataController.Instance.IsItemUnlocked(itemId))
            tooltip.OnClickNormalTip();
        else
            tooltip.InitWithMessage(DataController.Instance.GetItemUnlockMessage(itemId));
    }
    public void OnClickBuy()
    {
        if (!isOnclicked && gameObject.activeInHierarchy)
        {
            StartCoroutine(DelayOnClick());
        }
    }
    IEnumerator DelayOnClick()
    {
        isOnclicked = true;
        //Vector3 targetScale = transform.localScale * 0.85f;
        //System.Action<ITween<Vector3>> updateBtnScale = (t) =>
        //{
        //    transform.localScale = t.CurrentValue;
        //};
        //TweenFactory.Tween("BtnScale" + Random.Range(0, 100f) + Time.time, transform.localScale, targetScale, 0.03f, TweenScaleFunctions.QuinticEaseOut, updateBtnScale)
        //    .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateBtnScale));
        int cost = DataController.Instance.GetItemCost(itemId);
        if (!DataController.Instance.HasClaimFree(itemId) && canClaimFree) cost = 0;
        if (DataController.Instance.Ruby >= cost)
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("shop_boost", "buy", itemId);
            DataController.Instance.Ruby -= cost;
            currentItemQuantity = DataController.Instance.GetItemQuantity(itemId);
            if (cost == 0) DataController.Instance.ClaimFree(itemId);
            DataController.Instance.AddItem(itemId, 3);
            DataController.Instance.SaveData();
            for (int i = 0; i < 3; i++)
            {
                GameObject item = Instantiate(itemProp, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity,itemIcon.parent.parent);
                Vector3 target = item.transform.position + new Vector3(Random.Range(-100.5f, 100.5f), Random.Range(-100.5f, 100.5f), 0);//generate a random pos
                System.Action<ITween<Vector3>> updatePropPos = (t) =>
                {
                    item.transform.position = t.CurrentValue;
                };
                System.Action<ITween<Vector3>> completedPropMovement = (t) =>
                {
                    itemCount.text = DataController.Instance.GetItemQuantity(itemId).ToString();
                    Destroy(item);
                    timeStamp = Time.time;
                };
                TweenFactory.Tween("item" + i + Time.time, item.transform.position, target, 0.75f, TweenScaleFunctions.QuinticEaseOut, updatePropPos)
                .ContinueWith(new Vector3Tween().Setup(target, itemIcon.position, 0.25f + i * 0.06f, TweenScaleFunctions.QuadraticEaseIn, updatePropPos, completedPropMovement));
            }
            costText.text = DataController.Instance.GetItemCost(itemId).ToString();
            rubyIcon.SetActive(true);
            APIController.Instance.LogEventSpentRuby(cost, "buy_item");
            isOnclicked =false;
        }
        else
        {
            FindObjectOfType<ShopController>().OnClickRubyPanel();
            isOnclicked = false;
        }
        yield return new WaitForSeconds(0.5f);
        isOnclicked = false;
    }
}
