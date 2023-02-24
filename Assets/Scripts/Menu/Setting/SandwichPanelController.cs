using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichPanelController : MonoBehaviour
{
    [SerializeField] private GameObject sandwichNoti, settingNoti;
    [SerializeField] private SettingPanelController settingPanel;
    [SerializeField] private Transform cameraCanvas;
    private Animator anim;
    private float clickTimeStamp;
    private bool isSandwichOpen = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        settingNoti.SetActive(DataController.Instance.CheckVersion());
        sandwichNoti.SetActive(DataController.Instance.CheckVersion());
    }
    public void OnHideAchivementNotification(bool active)
    {
        sandwichNoti.SetActive(DataController.Instance.CheckVersion());
    }
    public void OnClickSandwichBtn()
    {
        if (Time.time - clickTimeStamp < 0.15f) return;
        clickTimeStamp = Time.time;
        isSandwichOpen = !isSandwichOpen;
        string index = "_0";
        if (DataController.Instance.GetLevelState(1, 6) == 0)
            index = "_1";
        if (isSandwichOpen)
        {
            anim.Play("Appear" + index);
            UIController.Instance.IsShowSandwichBtn = true;
        }
        else
            anim.Play("Disappear" + index);
    }
    public void OnClickSettingBtn()
    {
        if (Time.time - clickTimeStamp < 0.5f) return;
        clickTimeStamp = Time.time;
        settingPanel.OnShow();
    }
}
