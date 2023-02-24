using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ChristmasSalePackController : MonoBehaviour
{
    [SerializeField] private GameObject rubyPropPrefab, itemPropPrefab;
    [SerializeField] Image packIcon, background;
    [SerializeField] Sprite[] packIconSprites, backgroundSprites;
    [SerializeField] private int rubyAmount;
    [SerializeField] private int limitedEnergy = 0;//minutes
    [SerializeField] private int[] itemIds;
    [SerializeField] private int[] itemCounts;
    public void SetupUI(bool isLagrePackage)
    {
        if (isLagrePackage)
        {
            packIcon.sprite = packIconSprites[0];
            background.sprite = backgroundSprites[0];
        }
        else
        {
            packIcon.sprite = packIconSprites[1];
            background.sprite = backgroundSprites[1];
        }
    }
    public void SetBlur(bool isBlur)
    {
        GetComponentInChildren<Button>().interactable = !isBlur;
    }
    public void OnClickBuy()
    {
        if (rubyAmount > 0)
        {
            DataController.Instance.Ruby += rubyAmount;
            for (int i = 0; i < 5; i++)
            {
                GameObject ruby = Instantiate(rubyPropPrefab, transform.position + new Vector3((Random.Range(-200f, 0f)), Random.Range(-200f, 0f), -1f), Quaternion.identity, packIcon.transform);
                StartCoroutine(IMove(ruby, packIcon.transform.position, 1));
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
            GameObject item = Instantiate(itemPropPrefab, transform.position + new Vector3((Random.Range(-200f, 0f)), Random.Range(-200f, 0f), -1f), Quaternion.identity, packIcon.transform);
            StartCoroutine(IMove(item, packIcon.transform.position, 1));
        }
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
        var iapBtn = GetComponentInChildren<CookingIAPButton>();
        string iap_log = "";
        if (iapBtn != null)
        {
            iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
            PlayerPrefs.SetInt("chris_" + iapBtn.productId, 1);
        }
        IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
        string request = JsonUtility.ToJson(iapMessageLog);
        APIController.Instance.PostData(request, Url.IapMessageLog);
        int temp = PlayerPrefs.GetInt("chris_bought_count_2022", 0);
        temp++;
        PlayerPrefs.SetInt("chris_bought_count_2022", temp);
        string userType = PlayerPrefs.GetString("paying_type", "f1");
        APIController.Instance.LogEventHalloween(userType, "shop", iapBtn.productId);
        SetBlur(true);
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
