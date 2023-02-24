using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasOfferPanelController : UIView
{
    [SerializeField] Animator animator;
    [SerializeField] string[] packIds;
    [SerializeField] ChristmasItemOfferController[] christmasPacks;
    [SerializeField] Transform itemPanel;
    // Start is called before the first frame update
    void Start()
    {
        animator.Play("Appear");
        UIController.Instance.PushUitoStack(this);

        ShowOffer();
    }
    void ShowOffer()
    {
        int userType = 0;
        if (!PlayerPrefs.HasKey("chris_user_type"))
        {
            userType = PlayerClassifyController.Instance.GetPlayerClassifyLevel();
            PlayerPrefs.SetInt("chris_user_type", userType);
        }
        else
            userType = PlayerPrefs.GetInt("chris_user_type", 0);
        switch (userType)
        {
            case 0:
            case 1:
            case 4:
                {
                    CheckAndSpawnPackage(1, 2);
                    break;
                }
            case 2:
            case 3:
                {
                    CheckAndSpawnPackage(0, 1);
                    break;
                }
            case 5:
                {
                    CheckAndSpawnPackage(1, 3);
                    break;
                }
            case 6:
                {
                    CheckAndSpawnPackage(2, 4);
                    break;
                }
            case 7:
                {
                    CheckAndSpawnPackage(3, 4);
                    break;
                }
            case 8:
            case 9:
                {
                    CheckAndSpawnPackage(3, 5);
                    break;
                }
        }
    }

    public void CheckAndSpawnPackage(int packIndex1, int packIndex2)
    {
        bool hasBoughtPack1 = PlayerPrefs.GetInt("chris_" + packIds[packIndex1], 0) == 1;
        bool hasBoughtPack2 = PlayerPrefs.GetInt("chris_" + packIds[packIndex2], 0) == 1;
        SpawnPackage(packIndex1, hasBoughtPack1);
        SpawnPackage(packIndex2, hasBoughtPack2);
    }
    private void SpawnPackage(int packIndex, bool hasBought)
    {
        var pack1 = Instantiate(christmasPacks[packIndex], itemPanel.position, Quaternion.identity, itemPanel);
        pack1.SetBlurActive(hasBought);
    }
    public override void OnHide()
    {
        animator.Play("Disappear");
        //var hlw_offer = FindObjectOfType<HalloweenOfferController>();
        //if (hlw_offer != null)
        //    hlw_offer.CanShowOffer();
        UIController.Instance.PopUiOutStack();
        Destroy(gameObject, 0.4f);
    }
}
