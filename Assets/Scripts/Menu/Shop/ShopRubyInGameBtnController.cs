using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Purchasing;

public class ShopRubyInGameBtnController : MonoBehaviour
{
    [SerializeField] private GameObject rubyPropPrefab;
    [SerializeField] private Transform rubyIcon;
    [SerializeField] private int rubyAmount;
    bool isOnclicked;
    public void OnClickBuy()
    {
        if (!isOnclicked && gameObject.activeInHierarchy)
        {
            isOnclicked = true;
            DOVirtual.DelayedCall(0.1f, () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject item = Instantiate(rubyPropPrefab, new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), -0.1f), Quaternion.identity, transform.parent.parent.parent);
                    StartCoroutine(IMove(item));
                }
            });
            APIController.Instance.LogEventEarnRuby(rubyAmount, "buy_iap");
            DataController.Instance.Ruby += rubyAmount;
            DataController.Instance.SaveData();
            var iapBtn = GetComponentInChildren<CookingIAPButton>();
            string iap_log = "";
            if (iapBtn != null)
                iap_log = IAPPackHelper.GetContentPack(iapBtn.productId);
            IapMessageLog iapMessageLog = new IapMessageLog("log_inapp_done", iap_log);
            string request = JsonUtility.ToJson(iapMessageLog);
            APIController.Instance.PostData(request, Url.IapMessageLog);
            isOnclicked = false;
        }
        var pausePanel = FindObjectOfType<PausePanelController>();
        if (pausePanel != null)
            pausePanel.UpdateRubyAmount();
    }
    public IEnumerator IMove(GameObject go)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        float time = 0;
        Vector2 targetPos = rubyIcon.position;
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
