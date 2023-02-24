using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CertificateRewardPanel : UIView
{

    [SerializeField] Transform rewardLayer;
    [SerializeField] Animator animator;
    [SerializeField] RewardItemController rewardItem;
    List<Items> itemList = new List<Items>();
    List<GameObject> rewardList = new List<GameObject>();
    Action OnCloseCallback;
    public void Init(int tier, Action onCloseCallback)
    {
        this.OnCloseCallback = onCloseCallback;
        rewardLayer.transform.parent.gameObject.SetActive(true);
        switch (tier)
        {
            case 0:
                var go = Instantiate(rewardItem, rewardLayer);
                int id = DataController.Instance.GetRandomItemId();
                go.Init(id, 1);
                rewardList.Add(go.gameObject);
                break;
            case 1:
                for (int i = 0; i < 2; i++)
                {
                    var go2 = Instantiate(rewardItem, rewardLayer);
                    int id2 = DataController.Instance.GetRandomItemId();
                    if (i == 1)
                    {
                        itemList.Add(new Items(id2, 1));
                        go2.Init(id2, 1);
                        rewardList.Add(go2.gameObject);
                    }
                    else
                    {
                        itemList.Add(new Items(100500, 10));
                        go2.Init(100500, 10);
                        rewardList.Add(go2.gameObject);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    var go2 = Instantiate(rewardItem, rewardLayer);
                    int id2 = DataController.Instance.GetRandomItemId();
                    if (i == 0)
                    {
                        go2.Init(100500, 40);
                        itemList.Add(new Items(100500, 40));
                        rewardList.Add(go2.gameObject);
                    }
                    else
                    {
                        go2.Init(id2, 1);
                        itemList.Add(new Items(id2, 1));
                        rewardList.Add(go2.gameObject);
                    }
                }
                break;
        }
    }
    public override void OnHide()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemId == 100500)
                DataController.Instance.Ruby += itemList[i].itemQuantity;
            else
                DataController.Instance.AddItem(itemList[i].itemId, itemList[i].itemQuantity);
            Destroy(rewardList[i]);
        }
        itemList.Clear();
        rewardList.Clear();
        rewardLayer.transform.parent.gameObject.SetActive(false);
        DataController.Instance.SaveData(false);
        if (OnCloseCallback != null)
            OnCloseCallback.Invoke();
    }
}
