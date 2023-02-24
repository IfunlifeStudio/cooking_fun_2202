using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmasShopFullController : MonoBehaviour
{
    [SerializeField] string[] packIds;
    [SerializeField] ChristmasSalePackController[] christmasPacks;
    [SerializeField] GameObject xmasPackage;
    [SerializeField] Transform christmasGroup;
    // Start is called before the first frame update
    void Start()
    {
        bool canShowXmasPack = !BattlePassDataController.Instance.HasUnlockVip2022 && BattlePassDataController.Instance.CanClaimExp;
        xmasPackage.gameObject.SetActive(canShowXmasPack);
        int userType = PlayerPrefs.GetInt("chris_user_type", 0);
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
        if (hasBoughtPack1 && hasBoughtPack2) christmasGroup.gameObject.SetActive(false);
        else
        {
            if (!hasBoughtPack2) //Hien thi goi sale christmas 2
            {
                SpawnPackage(packIndex2, true);
            }
            if (!hasBoughtPack1) //Hien thi goi sale christmas 1
            {
                SpawnPackage(packIndex1, false);
            }
        }
    }
    private void SpawnPackage(int packIndex, bool isLargePackage)
    {
        Instantiate(christmasPacks[packIndex], christmasGroup).SetupUI(isLargePackage);
    }
    public void OnClickGetNow()
    {
        FindObjectOfType<ShopController>().OnHide();
        FindObjectOfType<XmasPassController>().OnClickOpenXmasPass();
    }
}
