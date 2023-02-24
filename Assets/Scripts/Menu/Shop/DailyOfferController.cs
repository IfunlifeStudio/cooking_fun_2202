using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyOfferController : MonoBehaviour
{
    [SerializeField] private GameObject dailyOfferBtn/*, specialWeekendBtn, bonusSundayBtn*/;
    [SerializeField] private TextMeshProUGUI coundownTime;
    [SerializeField] private GameObject dailyOfferPrefab/*, specialWeekendPrefab, bonusSundayPrefab*/;
    double duration;
    int dayOfWeek;
    bool CanShowDailyOffer = false/*, canShowSWOffer = false, canShowBSOffer*/;
    void Start()
    {
        if (CanDisplayDailyOffer())
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.AddDays(1).Date;
            duration = (tomorrow - now).TotalSeconds;
            StartCoroutine(CoundownTime());
        }
    }
    IEnumerator CoundownTime()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (duration > 0)
        {
            duration -= 1;
            if (duration < 0) duration = 86400;
            coundownTime.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", (int)duration / 3600, (int)(duration / 60) % 60, (int)duration % 60);
            yield return delay;
        }
        coundownTime.transform.parent.gameObject.SetActive(false);
    }
    public bool CanDisplayDailyOffer()
    {
        bool result = false;
        result = DataController.Instance.GetLevelState(1, 7) > 0;
        dailyOfferBtn.transform.parent.gameObject.SetActive(result);
        if (result)
            DisplayOfferBtn();
        return result;
    }
    public void DisplayOfferBtn()
    {
        CanShowDailyOffer = (((PlayerPrefs.GetInt("watched_daily_ads", 0) == 0) || PlayerPrefs.GetInt("DailyOffer0", 0) == 0 || PlayerPrefs.GetInt("DailyOffer1", 0) == 0)
            || PlayerPrefs.GetInt("day_of_year", 0) != System.DateTime.Now.DayOfYear);
        dailyOfferBtn.SetActive(CanShowDailyOffer);
        if (!CanShowDailyOffer)
            dailyOfferBtn.transform.parent.gameObject.SetActive(false);
    }
    public void OnClickShowDailyOfferPanel()
    {
        OnClickShowDailyPanel();
    }
    public void OnClickShowDailyPanel()
    {
        MainMenuController mainmenu = FindObjectOfType<MainMenuController>();
        Instantiate(dailyOfferPrefab, mainmenu.cameraCanvas);
        mainmenu.setIndexTab(5);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
