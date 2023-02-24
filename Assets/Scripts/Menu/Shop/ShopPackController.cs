using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Purchasing;

public class ShopPackController : MonoBehaviour
{
    [SerializeField] private GameObject rubyPropPrefab, itemPropPrefab;
    [SerializeField] private Transform rubyIcon, itemIcon;
    [SerializeField] private int rubyAmount;
    [SerializeField] private int limitedEnergy = 0;//minutes
    [SerializeField] private int[] itemIds;
    [SerializeField] private int[] itemCounts;
    private float timeStamp;
    private void Update()
    {
        float deltaTime = Time.time - timeStamp;//update top bar coin progress
        if (deltaTime < 0.1f)
        {
            float curentScale = Mathf.Lerp(1.25f, 1, 3.34f * Mathf.Sqrt(deltaTime));
            rubyIcon.localScale = new Vector3(curentScale, curentScale, 1);
            itemIcon.localScale = new Vector3(curentScale, curentScale, 1);
        }
        else
        {
            if (deltaTime < 0.2f)
            {
                rubyIcon.localScale = Vector3.one;
                itemIcon.localScale = Vector3.one;
            }
        }
    }
    public void OnClickBuy()
    {

        if (rubyAmount > 0)
        {
            DataController.Instance.Ruby += rubyAmount;
            for (int i = 0; i < 5; i++)
            {
                GameObject ruby = Instantiate(rubyPropPrefab, transform.position + new Vector3((Random.Range(-200f, 0f)), Random.Range(-200f, 0f), -1f), Quaternion.identity, rubyIcon.parent.parent);
                StartCoroutine(IMove(ruby, rubyIcon.transform.position, 1));
            }
        }
        int totalItem = 0;
        for (int i = 0; i < itemIds.Length; i++)
        {
            if (itemCounts[i] > 0)
                DataController.Instance.AddItem(itemIds[i], itemCounts[i]);
            totalItem += itemCounts[i];
        }
        if (limitedEnergy > 0)
        {
            DataController.Instance.AddUnlimitedEnergy(limitedEnergy);
            FindObjectOfType<EnergyController>()?.PlayUnlimitedEnergyEffect();
        }
        DataController.Instance.SaveData();
        for (int i = 0; i < 10; i++)
        {
            GameObject item = Instantiate(itemPropPrefab, transform.position + new Vector3((Random.Range(-200f, 0f)), Random.Range(-200f, 0f), -1f), Quaternion.identity, itemIcon.parent.parent);
            StartCoroutine(IMove(item, itemIcon.transform.position, 1));
        }
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
    }
    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f + (Random.Range(-200f, 0f)), (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        while (Vector2.Distance(gameObject.transform.position, pos) > 17f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 2;
            yield return null;
        }
        DG.Tweening.DOVirtual.DelayedCall(0.05f, () =>
        {
            Destroy(gameObject);
        });
    }

    public Vector3 CalculateQuadraticBezierPoint(float t1, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t1;
        float tt = t1 * t1;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t1 * p1;
        p += tt * p2;
        return p;
    }
}
