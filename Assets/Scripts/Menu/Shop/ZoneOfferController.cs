using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneOfferController : MonoBehaviour
{
    [SerializeField] GameObject zoneOfferBtn;
    [SerializeField] GameObject[] zoneOfferPanel;
    [SerializeField] Image zoneOfferBtnIcon;
    [SerializeField] Sprite[] iconSprites;
    // Start is called before the first frame update
    void Start()
    {
        ShowZoneOfferBtn();
    }
    public bool CanShowZoneOfferPanel()
    {
        var lastPassLevel = LevelDataController.Instance.lastestPassedLevel;
        if (lastPassLevel != null)
            if (lastPassLevel.chapter > 1 && lastPassLevel.id == 1 && DataController.Instance.GetLevelState(lastPassLevel.chapter, lastPassLevel.id) == 1)
                return true;
        return false;
    }
    public void OnClickZoneOfferBtn()
    {
        MainMenuController mainmenu = FindObjectOfType<MainMenuController>();
        Instantiate(zoneOfferPanel[GetSalePackIndex()], mainmenu.cameraCanvas);
        mainmenu.setIndexTab(5);
    }
    public void ShowZoneOfferBtn()
    {
        int highestUnlockRestaurant = DataController.Instance.HighestRestaurant;
        if (PlayerPrefs.GetInt("has_bought_zone_offer", 1) < highestUnlockRestaurant)
        {
            zoneOfferBtn.SetActive(true);
            zoneOfferBtnIcon.sprite = iconSprites[highestUnlockRestaurant - 2];
        }
        else
            zoneOfferBtn.SetActive(false);
    }
    public int GetSalePackIndex()
    {
        switch (DataController.Instance.Rank)
        {
            case 0:
            case 1:
            case 2: return 0;
            case 3: return 1;
            case 4: return 2;
            case 5: return 3;
        }
        return 0;
    }
}
