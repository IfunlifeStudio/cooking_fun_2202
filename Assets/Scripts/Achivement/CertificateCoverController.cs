using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CertificateCoverController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] Image coverImg, coverIconImg, bgImg;
    [SerializeField] Sprite[] coverSprites, coverIconSprites;
    [SerializeField] GameObject Layer0;
    Action onCloseCallback;
    Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Appear");
    }
    public void Init(int resID, Vector2 targetPos, Action onCloseCallback)
    {
        this.targetPos = targetPos;
        this.onCloseCallback = onCloseCallback;
        coverIconImg.sprite = coverIconSprites[resID - 1];
        coverImg.sprite = GetCoverImgSprite(resID);
        var Certidata = DataController.Instance.GetGameData().GetCertificateDataAtRes(resID);
        if (Certidata.WinRate >= 90)
            coverIconImg.color = new Color32(255, 255, 0, 255);
        else if (Certidata.WinRate >= 80)
            coverIconImg.color = new Color32(171, 56, 237, 255);
        else if (Certidata.WinRate >= 70)
            coverIconImg.color = new Color32(29, 126, 225, 255);
        else
            coverIconImg.color = new Color32(4, 177, 0, 255);
        string cerStr = AchivementController.Instance.GetWinrateCerti(resID);
        if (resID == 0 || cerStr == "") return;
        APIController.Instance.LogEventCertificateTracking(cerStr, resID);
    }
    public override void OnHide()
    {
        Layer0.SetActive(false);
        bgImg.enabled = false;
        StartCoroutine(IMove(coverImg.gameObject, targetPos));
        //animator.Play("Disappear");

    }
    private Sprite GetCoverImgSprite(int resId)
    {
        if (resId <= 4)
            return coverSprites[0];
        else if (resId <= 8)
            return coverSprites[1];
        else if (resId <= 13)
            return coverSprites[2];
        else return coverSprites[3];
    }
    private IEnumerator IMove(GameObject gameObject, Vector2 targetPos)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + targetPos.x) / 2f, (gameObject.transform.position.y + targetPos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector2 vt = new Vector2(1, 1);
        while (Vector2.Distance(gameObject.transform.position, targetPos) > 1.5f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, targetPos);
            time += Time.deltaTime * 3;
            vt.x = vt.y = (1 - (time / 1.5f));
            gameObject.transform.localScale = vt;
            yield return null;
        }
        if (onCloseCallback != null)
            onCloseCallback.Invoke();
        Destroy(this.gameObject);
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
