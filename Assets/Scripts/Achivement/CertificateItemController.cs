using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CertificateItemController : MonoBehaviour
{
    [SerializeField] int resId;
    [SerializeField] Image resIcon, bookItem;
    [SerializeField] Material grayMaterial;
    [SerializeField] CertificationPanelController certificationPanel;
    [SerializeField] GameObject notificationIcon;
    bool isFullKey, isFullStage;
    // Start is called before the first frame update
    void Start()
    {
        int highestRes = DataController.Instance.HighestRestaurant;
        var Certidata = DataController.Instance.GetGameData().GetCertificateDataAtRes(resId);

        if (resId > highestRes)
        {
            GetComponent<Button>().interactable = false;
            gameObject.SetActive(true);
            bookItem.material = grayMaterial;
            resIcon.enabled = false;
        }
        else if (resId < highestRes && Certidata.WinRate != 0)
        {
            int totalLevelInRes = DataController.Instance.GetTotalLevelsPerRestaurant(resId);
            int totalStageInRes = totalLevelInRes * 3;
            int totalStageGranted = DataController.Instance.GetTotalStageGranted(resId);
            int totalkeyGranted = DataController.Instance.GetRestaurantKeyGranted(resId);
            isFullKey = (totalkeyGranted >= totalLevelInRes);
            isFullStage = (totalStageGranted >= totalStageInRes);
            if (!isFullKey) resIcon.enabled = false;
            gameObject.SetActive(true);
            if (Certidata.RewardRecord[2] == 1) Certidata.IsCompletedCertificate = true;
            //if (isFullKey && (Certidata.RewardRecord[2] == 0 || !Certidata.IsCompletedCertificate))
            //{
            if (Certidata.WinRate >= 90)
                resIcon.color = new Color32(255, 255, 0, 255);
            else if (Certidata.WinRate >= 80)
                resIcon.color = new Color32(171, 56, 237, 255);
            else if (Certidata.WinRate >= 70)
                resIcon.color = new Color32(29, 126, 225, 255);
            else
                resIcon.color = new Color32(4, 177, 0, 255);
            //}
        }
        else if (resId == highestRes)
        {
            //int totalKey = DataController.Instance.GetTotalLevelsPerRestaurant(resId);
            //int totalKeyGranted = DataController.Instance.GetRestaurantKeyGranted(resId);
            //if(to)
            gameObject.SetActive(true);
            resIcon.enabled = false;
            //resIcon.color = new Color32(255, 255, 255, 255);
        }
        else { gameObject.SetActive(false); }
    }
    public void ShowNotification()
    {
        notificationIcon.SetActive(true);
    }
    public void OnClickCertificate()
    {
        notificationIcon.SetActive(false);
        if (resId == PlayerPrefs.GetInt("has_profile_notify", 0))
            PlayerPrefs.SetInt("has_profile_notify", 0);
        var certi = Instantiate(certificationPanel, FindObjectOfType<MainMenuController>().cameraCanvas);
        certi.SetupData(resId, isFullKey, isFullStage, FindObjectOfType<ProfilePanelController>());
    }
}
