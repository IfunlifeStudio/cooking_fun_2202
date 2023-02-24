using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmasCollapseController : MonoBehaviour
{
    [SerializeField] private GameObject xmasPackage;
    [SerializeField] private Transform christmasGroup;
    [SerializeField] string[] packIds;
    [SerializeField] GameObject[] shopPackages;
    [SerializeField] ChristmasSalePackController[] christmasPacks;
    // Start is called before the first frame update
    void Start()
    {
        bool canShowXmasPack = !BattlePassDataController.Instance.HasUnlockVip2022 && BattlePassDataController.Instance.CanClaimExp;
        xmasPackage.gameObject.SetActive(canShowXmasPack);
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
    public void OnClickGetNow()
    {
        FindObjectOfType<ShopController>().OnHide();
        FindObjectOfType<XmasPassController>().OnClickOpenXmasPass();
        APIController.Instance.LogEventClickBuyBP("button_getnow", BattlePassDataController.Instance.CurrentBpLevel.ToString());
    }
    private void SpawnPackage(int packIndex, bool islargePack)
    {
        Instantiate(shopPackages[packIndex], christmasGroup);
        Instantiate(christmasPacks[packIndex], christmasGroup).SetupUI(islargePack);
    }
    public void CheckAndSpawnPackage(int packIndex1, int packIndex2)
    {
        bool hasBoughtPack1 = PlayerPrefs.GetInt("chris_" + packIds[packIndex1], 0) == 1;
        bool hasBoughtPack2 = PlayerPrefs.GetInt("chris_" + packIds[packIndex2], 0) == 1;
        if (!hasBoughtPack2) //Hien thi goi sale christmas 2
        {
            SpawnPackage(packIndex2, true);
            return;
        }
        else if (!hasBoughtPack1) //Hien thi goi sale christmas 1
        {
            SpawnPackage(packIndex1, false);
            return;
        }
    }
}
