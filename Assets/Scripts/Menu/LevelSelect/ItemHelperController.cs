using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemHelperController : MonoBehaviour
{
    [SerializeField] private Sprite[] itemIcons;
    [SerializeField] private Image itemImage;
    private int itemId;
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
        itemId = LevelDataController.Instance.GetRecommendItemId();
        int index = 0;
        switch (itemId)
        {
            case (int)ItemType.Anti_OverCook:
                index = 0;
                break;
            case (int)ItemType.Double_Coin:
                index = 1;
                break;
        }
        itemImage.sprite = itemIcons[index];
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void OnClickContinue()
    {
        FindObjectOfType<LevelSelectorController>().OnClickPlay();
        StartCoroutine(DelayClosePanel());
    }
    public void OnClickEquipAndContinue()
    {
        var items = FindObjectsOfType<ItemBtnController>();
        foreach (var item in items)
            if(item.ItemId== itemId)
                item.OnClick();
        StartCoroutine(DelayClosePanel());
    }
}
