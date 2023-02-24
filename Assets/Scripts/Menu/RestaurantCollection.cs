using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
public class RestaurantCollection : MonoBehaviour
{
    public ScrollRect resBackground;
    public TextMeshProUGUI keyBanner,activeUser;
    public SkeletonGraphic iceCreamCar, icecreamHuman;
    public GameObject iceCreamNoti;
    [SerializeField] private GameObject switchZoneBtn, zoneTutorialPrefab, zoneIntroductionPrefab;
    public RestaurantButton[] restaurantButtons;
    [SerializeField] private Material[] resUIMats;
    public void InitiateResBtns()
    {
        foreach (var resBtn in restaurantButtons)
        {
            if (DataController.Instance.IsRestaurantUnlocked(resBtn.resIndex))
            {
                if (resBtn.resIndex == 11)
                    resBtn.GetComponentInChildren<Spine.Unity.SkeletonGraphic>().material = resUIMats[2];
                else
                {
                    resBtn.GetComponent<Image>().material = resUIMats[0];
                    resBtn.SetUpMaterial(resUIMats[0]);
                }
            }
            else
            {
                if (resBtn.resIndex == 11)
                    resBtn.GetComponentInChildren<Spine.Unity.SkeletonGraphic>().material = resUIMats[3];
                else
                {
                    resBtn.GetComponent<Image>().material = resUIMats[1];
                    resBtn.SetUpMaterial(resUIMats[1]);
                }
            }
        }
        if (PlayerPrefs.GetInt(MainMenuController.ZONE_INDEX, -1) == 1)
        {
            if (DataController.Instance.GetTutorialData().Contains(204)) return;
            DataController.Instance.GetTutorialData().Add(204);
            StartCoroutine(PlayZoneIntroduction());
        }
        if (PlayerPrefs.GetInt(MainMenuController.ZONE_INDEX, -1) == 2)
        {
            if (DataController.Instance.GetTutorialData().Contains(208)) return;
            DataController.Instance.GetTutorialData().Add(208);
            StartCoroutine(PlayZoneIntroduction());
        }
        //if (switchZoneBtn == null) return;
        //if (DataController.Instance.IsRestaurantUnlocked(5))
        //{
        //    switchZoneBtn.GetComponent<Image>().material = resUIMats[0];
        //}
        //else
        //{
        //    switchZoneBtn.GetComponent<Image>().material = resUIMats[1];
        //}
    }
    private IEnumerator PlayZoneIntroduction()
    {
        GameObject go = Instantiate(zoneIntroductionPrefab);
        go.GetComponentInChildren<Animator>().Play("Appear");
        yield return new WaitForSeconds(1.8f);
        go.GetComponentInChildren<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(go);
    }
    public Transform GetRestaurantById(int resIndex)
    {
        foreach (var resBtn in restaurantButtons)
        {
            if (resBtn.resIndex == resIndex)
                return resBtn.transform;
        }
        if (resIndex == 4 && switchZoneBtn != null) return switchZoneBtn.transform;
        return null;
    }
    public void OnClickSwitchZone()
    {
        FindObjectOfType<MainMenuController>().OnClickSwitchZone();
    }
    public void OnClickIceCreamCar()
    {
        FindObjectOfType<IceCreamController>().OnBtnIceCreamCarClick();
    }
    public void ShowSwitchZoneTutorial()
    {
        UIController.Instance.CanBack = false;
        int zoneIndex = PlayerPrefs.GetInt(MainMenuController.ZONE_INDEX, -1);
        if (zoneIndex == 0 && DataController.Instance.GetTutorialData().Contains(205)) return;
        if (zoneIndex == 1 && DataController.Instance.GetTutorialData().Contains(207)) return;
        if (zoneIndex == 0)
            DataController.Instance.GetTutorialData().Add(205);
        else if (zoneIndex == 1)
            DataController.Instance.GetTutorialData().Add(207);
        Transform zoneCanvas = FindObjectOfType<MainMenuController>().zoneCanvas;
        GameObject go = Instantiate(zoneTutorialPrefab, zoneCanvas);
        go.GetComponentInChildren<Button>().onClick.AddListener(
                        () =>
                        {
                            OnClickSwitchZone();
                            UIController.Instance.CanBack = true;
                            Destroy(go);
                        });
    }
}
