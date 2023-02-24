using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPackIngameBtnController : MonoBehaviour
{
    [SerializeField] private GameObject rubyPropPrefab, itemPropPrefab;
    [SerializeField] private Transform packIcon;
    [SerializeField] private int rubyAmount;
    [SerializeField] private int limitedEnergy = 0;//minutes
    [SerializeField] private int[] itemIds;
    [SerializeField] private int[] itemCounts;
    public void OnClickBuy()
    {
        if (rubyAmount > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject ruby = Instantiate(rubyPropPrefab, new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), -0.1f), Quaternion.identity, transform.parent.parent.parent);
                StartCoroutine(IMove(ruby));
            }
            DataController.Instance.Ruby += rubyAmount;
        }
        for (int i = 0; i < itemIds.Length; i++)
        {
            if (itemCounts[i] > 0)
                DataController.Instance.AddItem(itemIds[i], itemCounts[i]);
        }
        if (limitedEnergy > 0)
        {
            DataController.Instance.AddUnlimitedEnergy(limitedEnergy);
        }
        DataController.Instance.SaveData();
        for (int i = 0; i < 5; i++)
        {
            GameObject item = Instantiate(itemPropPrefab, new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), -0.1f), Quaternion.identity, transform.parent.parent.parent);
            StartCoroutine(IMove(item));
        }
        DataController.Instance.SaveData();
        APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_IAP");
    }
    public IEnumerator IMove(GameObject go)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        float time = 0;
        Vector2 targetPos = packIcon.position;
        Vector2 midlePos = new Vector2((go.transform.position.x + targetPos.x) / 2f, (go.transform.position.y + targetPos.y) / 3f);
        Vector2 tempPos = go.transform.position;
        yield return new WaitForSecondsRealtime(0.1f);
        while (Vector2.Distance(go.transform.position, targetPos) > 0.4f)
        {
            go.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, targetPos);
            time += Time.unscaledDeltaTime * 4;
            yield return null;
        }
        Destroy(go);
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
