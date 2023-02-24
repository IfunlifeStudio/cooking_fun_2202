using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GDPRPopup : MonoBehaviour
{
    public const string GDPR = "gdpr";
    private string privacy = "https://cscmobi.com/privacy.htm";
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("Appear");
    }
    public void OnclickPrivacy()
    {
         Application.OpenURL(privacy);
    }
    public void OnClickAgree()
    {
        GetComponent<Animator>().Play("Disappear");
        PlayerPrefs.SetInt(GDPR, 1);
        IronsourceAdsController.Instance.Init();
        Destroy(gameObject, 0.5f);
    }
    public void OnClickDeny()
    {
        GetComponent<Animator>().Play("Disappear");
        PlayerPrefs.SetInt(GDPR, 0);
        IronsourceAdsController.Instance.Init();
        Destroy(gameObject, 0.5f);
    }
    public static bool CanDisplayGDPRPopup()
    {
        var timeZone = System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).TotalHours;
        return PlayerPrefs.GetInt(GDPR, -1) == -1 && timeZone >= 0 && timeZone <= 3;
    }
    public static bool IsUserAllowAdsConsent()
    {
        var timeZone = System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).TotalHours;
        return PlayerPrefs.GetInt(GDPR, -1) == 1 || timeZone > 3 || timeZone < 0;
    }
}
