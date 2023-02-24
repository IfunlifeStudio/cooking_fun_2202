using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class CSCAdsIcon : MonoBehaviour
{
    private AdsElement adsData;
    [SerializeField] private RawImage adsImage;
    private bool shouldUpdateAdsStatus = false;
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1.5f);//wait for in house ads to init
        var rewardElements = CSCAdsController.Instance.GetAdsElementWithType("1");
        if (rewardElements.Count == 0)
        {
            adsImage.transform.parent.gameObject.SetActive(false);
            yield break;
        }
        adsData = rewardElements[UnityEngine.Random.Range(0, rewardElements.Count)];
        var star = DataController.Instance.GetLevelState(1, 7);
        bool canShowAds = star > 0;
        if (canShowAds && File.Exists(adsData.GetAdsPath()))
        {
            var fileData = File.ReadAllBytes(adsData.GetAdsPath());
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            adsImage.texture = tex;
            CSCAdsController.Instance.UpdateAdsCampaign(adsData.id, 1, int.Parse(adsData.type));
            adsImage.transform.parent.gameObject.SetActive(true);
        }
        else adsImage.transform.parent.gameObject.SetActive(false);
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
        Application.OpenURL(adsData.targetUrl);
    }
}
