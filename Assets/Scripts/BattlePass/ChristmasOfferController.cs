using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChristmasOfferController : MonoBehaviour
{
    [SerializeField] private GameObject christmasBtn, christmasPanel;
    //[SerializeField] private TextMeshProUGUI eventBtnCountdown;
    [SerializeField] private MainMenuController mainMenu;
    [SerializeField] private ShopController shopController;
    bool canShowOffer;
    private void Start()
    {
        if (DataController.Instance.IsShowNoel == 1)
            CanShowOffer();
    }
    public bool CanShowOffer()
    {
        if (!BattlePassDataController.Instance.HasUnlockEvent)
        {
            christmasBtn.SetActive(false);
            return false;
        }
        var today = DateTime.UtcNow;
        TimeSpan timeSpan = BattlePassDataController.EndTime - today;
        if (PlayerPrefs.GetInt("chris_bought_count_2022", 0) >= 2 || timeSpan.TotalSeconds < 0)
        {
            canShowOffer = false;
            return false;
        }
        else
        {
            canShowOffer = true;
            return true;
        }
    }
    //private IEnumerator UpdateCountDown(int leftTime)
    //{
    //    var delay = new WaitForSecondsRealtime(60);
    //    double deltaTime = 0;
    //    string eventRemain = "";
    //    if (leftTime <= 0)
    //    {
    //        while (true)
    //        {
    //            deltaTime = DataController.ConvertToUnixTime(endDay.ToUniversalTime()) - DataController.ConvertToUnixTime(DateTime.UtcNow);
    //            eventRemain = (int)(deltaTime / 84600) + "d" + (int)((deltaTime % 84600) / 3600) + "h " + ((int)(deltaTime % 3600) / 60) + "m";//set the count down
    //            eventBtnCountdown.text = eventRemain;
    //            yield return delay;
    //        }
    //    }
    //}
    public void OnClickOpenPanel()
    {
        if (canShowOffer)
            Instantiate(christmasPanel, mainMenu.cameraCanvas);
        else
            shopController.OnClickRubyPanel();
    }
}
