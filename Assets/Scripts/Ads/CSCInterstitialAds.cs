using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class CSCInterstitialAds : MonoBehaviour
{
    private AdsElement adsData;
    [SerializeField] private RawImage adsImage;
    [SerializeField] GameObject closeBtn;
    [SerializeField] Text countDownText;
    [SerializeField] Image countDownCircle;
    private bool shouldUpdateAdsStatus = false;
    public System.Action onCloseAds;
    private float eslapseTime = 0, waitTime = 5;
    public void Spawn(AdsElement element, System.Action onClose)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<CSCInterstitialAds>().Init(element, onClose);
    }
    public void Init(AdsElement element, System.Action onClose)
    {
        this.adsData = element;
        onCloseAds = onClose;
    }
    void Start()
    {
        if (File.Exists(adsData.GetAdsPath()))
        {
            var fileData = File.ReadAllBytes(adsData.GetAdsPath());
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            adsImage.texture = tex;
        }
        else adsImage.gameObject.SetActive(false);
        StartCoroutine(DelayEnableBtn());
    }
    IEnumerator DelayEnableBtn()
    {
        var delay = new WaitForSecondsRealtime(1);
        eslapseTime = 0;
        closeBtn.SetActive(false);
        countDownCircle.gameObject.SetActive(true);
        while (eslapseTime < waitTime)
        {
            countDownText.text = Mathf.FloorToInt(waitTime - eslapseTime).ToString();
            countDownCircle.fillAmount = 1 - eslapseTime * 1f / waitTime;
            yield return delay;
            eslapseTime++;
        }
        countDownCircle.gameObject.SetActive(false);
        closeBtn.SetActive(true);
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && shouldUpdateAdsStatus)
        {
            shouldUpdateAdsStatus = false;
            CSCAdsController.Instance.UpdateAdsCampaign(adsData.id, 2, int.Parse(adsData.type));
        }
    }
    public void OnClickAds()
    {
        shouldUpdateAdsStatus = true;
    }
    public void OnClickClose()
    {
        AudioController.Instance.UnPauseMusic();
        if (onCloseAds != null)
            onCloseAds.Invoke();
        Destroy(gameObject);
    }
}
