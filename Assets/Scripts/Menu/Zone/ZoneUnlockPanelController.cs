using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneUnlockPanelController : UIView
{
    private void Start()
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
    }
    public void Init(int _zoneindex)
    {        
        PlayerPrefs.SetInt("switch_zone_index", _zoneindex);
    }
    public void OnClickGo()
    {
        UIController.Instance.PopUiOutStack();
        int zoneIndex = PlayerPrefs.GetInt("switch_zone_index", 0);
        FindObjectOfType<MainMenuController>().OnChooseZone(zoneIndex);
    }
    public void OnClickFind()
    {
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        FindObjectOfType<MainMenuController>().OpenLastestRestaurant();
        Destroy(gameObject, 0.25f);
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.25f);
    }
}
