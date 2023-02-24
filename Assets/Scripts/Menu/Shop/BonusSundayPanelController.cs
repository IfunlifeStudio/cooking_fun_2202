using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusSundayPanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI timeLeft;
    [SerializeField] GameObject[] salePacks;
    [SerializeField] Transform packGroup;
    [SerializeField] RectTransform ContentRect;
    double duration;
    bool isScroll;
    int targetScrollValue;
    void Start()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime tomorrow = now.AddDays(1).Date;
        duration = (tomorrow - now).TotalSeconds;
        animator.Play("Appear");
        StartCoroutine(CoundownTime());
        if (DataController.ConvertToUnixTime(System.DateTime.Now) - DataController.Instance.BonusSundayTimeStamp > 86400)
            ResetData();
        InitSalePackage();
        ContentRect = packGroup.GetComponentInChildren<ScrollRect>().content;
        MainMenuController.isFirstOpenDailyOffer = false;
        isScroll = true;
    }
    private void Update()
    {
        if (isScroll)
        {
            float deltaPos = targetScrollValue - ContentRect.anchoredPosition.x;
            float sign = deltaPos / Mathf.Abs(deltaPos);
            float baseSpeed = Mathf.Max(600f, 2 * deltaPos * sign);
            if (Mathf.Abs(deltaPos) > 20)
                ContentRect.anchoredPosition += new Vector2(sign * Time.deltaTime * baseSpeed, 0);
            else
                isScroll = false;
        }
    }
    private void ResetData()
    {
        DataController.Instance.BonusSundayTimeStamp = DataController.ConvertToUnixTime(System.DateTime.Now);
        PlayerPrefs.SetString("has_bought_sunday", "");
    }
    public void InitSalePackage()
    {
        int userRank = DataController.Instance.Rank;
        switch (userRank)
        {
            case 0:
            case 1:
            case 2:
                Instantiate(salePacks[0], packGroup);
                break;
            case 3:
                Instantiate(salePacks[1], packGroup);
                break;
            case 4:
            case 5:
                Instantiate(salePacks[2], packGroup);
                break;
        }
    }
    IEnumerator CoundownTime()
    {
        var delay = new WaitForSecondsRealtime(1);
        while (true)
        {
            if (duration > 0)
            {
                duration -= 1;
                if (duration < 0) duration = 86400;
                timeLeft.text = System.String.Format("{0:D2}:{1:D2}:{2:D2}", (int)duration / 3600, (int)(duration / 60) % 60, (int)duration % 60);
            }
            yield return delay;
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        FindObjectOfType<DailyOfferController>().CanDisplayDailyOffer();
        animator.Play("Disappear");
        FindObjectOfType<MainMenuController>().DislayDailyReward();
        Destroy(gameObject, 0.3f);
    }
}
