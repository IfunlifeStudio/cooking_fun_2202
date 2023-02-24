using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
using UnityEngine.Purchasing;
using System;

public class StarterPackController : MonoBehaviour
{
    public const string STARTER_PACK_HASH = "starter_pack", STARTER_PACK_OPEN_TIMESTAMP = "starter_pack_timestamp";
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject starterPackBtn;
    [SerializeField] private TextMeshProUGUI starterPackCountdown, saleTag;
    [SerializeField] private StarterPackPanelController[] starterPackPanels;
    [SerializeField] private Transform cameraCanvas;
    private float clickTimeStamp = 0, packLifeTime, packTimeStamp, timeStamp;
    private int starterPackLifeTime = 10800;
    private bool IsOpenStarterPack90;
    private void Start()
    {
        CalculateTimeAndDisplay();
    }
    public void CalculateTimeAndDisplay()
    {
        packTimeStamp = PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0);
        if (CanDisplayPack())
        {
            IsOpenStarterPack90 = ShouldOffer90PercentSale();
            if (IsOpenStarterPack90)
            {
                DateTime now = DateTime.Now;
                DateTime tomorrow = now.AddDays(1).Date;
                packLifeTime = (float)(tomorrow - now).TotalSeconds;
                saleTag.text = "80%\noff";
            }
            else
            {
                packLifeTime = (float)(packTimeStamp + starterPackLifeTime - (DataController.ConvertToUnixTime(System.DateTime.UtcNow)));
                saleTag.text = "50%\noff";
            }
            starterPackBtn.gameObject.SetActive(packLifeTime > 0);
        }
        else
            starterPackBtn.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Time.time - timeStamp > 1)
        {
            packLifeTime -= 1;
            if (packLifeTime < 0) packLifeTime = 0;
            int hours = (int)(packLifeTime / 3600);
            if (hours > 0)
                starterPackCountdown.text = System.String.Format(" {0:D2}:{1:D2}:{2:D2}", (int)packLifeTime / 3600, (int)(packLifeTime / 60) % 60, (int)packLifeTime % 60);
            else
                starterPackCountdown.text = System.String.Format(" {0:D2}:{1:D2}", (int)(packLifeTime / 60) % 60, (int)packLifeTime % 60);
            timeStamp = Time.time;
        }
    }
    public bool HasDisplayedPackToday()//check if has display this pack today
    {
        return System.DateTime.UtcNow.DayOfYear == DataController.ConvertFromUnixTime(PlayerPrefs.GetFloat(STARTER_PACK_OPEN_TIMESTAMP)).DayOfYear;
    }
    public bool CanDisplayPack()
    {
        var iap_value = DataController.Instance.GetGameData().userIapValue;
        if (iap_value != 0) return false;
        float starterPackTimeStamp = PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0);
        //if (iap_value != 0 && (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - starterPackTimeStamp >= 259200))
        //    return false;
        return starterPackTimeStamp != 0 && PlayerPrefs.GetInt("has_buy_starter_pack", 0) == 0 && iap_value == 0;
    }
    public void OnClickOpenPanel()
    {
        if (Time.time - clickTimeStamp < 0.5f) return;
        else
            clickTimeStamp = Time.time;
        if (IsOpenStarterPack90)
            OpenStarterPackSale90();
        else
            OpenPackPanel();
    }
    public void OpenPackPanel()
    {
        Instantiate(starterPackPanels[0], cameraCanvas);
        PlayerPrefs.SetFloat(STARTER_PACK_OPEN_TIMESTAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        AudioController.Instance.PlaySfx(popUpClip);
        FindObjectOfType<MainMenuController>()?.setIndexTab(5);
    }
    public void OpenStarterPackSale90()
    {
        Instantiate(starterPackPanels[1], cameraCanvas);
        PlayerPrefs.SetFloat(STARTER_PACK_OPEN_TIMESTAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        AudioController.Instance.PlaySfx(popUpClip);
        FindObjectOfType<MainMenuController>()?.setIndexTab(5);
    }
    private bool ShouldOffer90PercentSale()
    {
        float starterPackTimeStamp = PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0);
        return starterPackTimeStamp != 0 && DataController.ConvertToUnixTime(System.DateTime.UtcNow) - starterPackTimeStamp > starterPackLifeTime && DataController.Instance.GetGameData().userIapValue < 0.1f;
    }
    public void ShowIcon()
    {
        CalculateTimeAndDisplay();
    }
}
