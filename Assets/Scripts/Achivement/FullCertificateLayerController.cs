using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class FullCertificateLayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI certificateText, signatureTxt, timeText;
    string certificateStr, timeStr;
    int resID;
    public void Init(bool isFullVer, int resID, string certificate, string time)
    {
        this.resID = resID;
        certificateStr = certificate;
        if (time == "")
            time = DateTime.Now.Date.ToString("MM/dd/yyyy");
        timeStr = time;
        if (isFullVer)
        {
            certificateText.text = certificateStr;
            signatureTxt.text = DataController.Instance.GetRestaurantById(resID).restaurantName;
            timeText.text = timeStr;
            animator.Play("FullVersion");
        }
        else
            animator.Play("MissingVersion");
    }
    public void OnCompleteCertificate()
    {
        animator.Play("FillCertificate");
        var profiledata = DataController.Instance.GetGameData().profileDatas;
        if (profiledata.CertiTile == "king_chef" || profiledata.CertiTile == "master_chef" || profiledata.CertiTile == "talent_chef" || profiledata.CertiTile == "protential_chef")
        {
            profiledata.CertiTile = AchivementController.Instance.GetWinrateCerti(resID);
            profiledata.CertiColor = AchivementController.Instance.GetWinrateCerti(resID, false);
        }
    }
    public void FillCertificate()
    {
        certificateText.gameObject.SetActive(true);
        certificateText.text = certificateStr;
        var vra = certificateText.GetComponent<VertexRevealAnimation>();
        vra.StartAnimateVertexFadeInColors();
    }
    public void FillSignature()
    {
        signatureTxt.gameObject.SetActive(true);
        signatureTxt.text = DataController.Instance.GetRestaurantById(resID).restaurantName;
        var vra = signatureTxt.GetComponent<VertexRevealAnimation>();
        vra.StartAnimateVertexFadeInColors();
    }
    public void FillTime()
    {
        timeText.gameObject.SetActive(true);
        timeText.text = timeStr;
        var vra = timeText.GetComponent<VertexRevealAnimation>();
        vra.StartAnimateVertexFadeInColors();
    }

    public IEnumerator IMove(GameObject gameObject, Vector2 pos, float speed, Action action = null)
    {
        float time = 0;
        Vector2 midlePos = new Vector2((gameObject.transform.position.x + pos.x) / 2f, (gameObject.transform.position.y + pos.y) / 3f);
        Vector2 tempPos = gameObject.transform.position;
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(true);
        while (Vector2.Distance(gameObject.transform.position, pos) > 0.3f)
        {
            gameObject.transform.position = CalculateQuadraticBezierPoint(time, tempPos, midlePos, pos);
            time += Time.deltaTime * speed * 2;
            gameObject.transform.localScale = new Vector2(1 / (time * 4), 1 / (time * 4));
            yield return null;
        }
        if (action != null)
            action.Invoke();
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
