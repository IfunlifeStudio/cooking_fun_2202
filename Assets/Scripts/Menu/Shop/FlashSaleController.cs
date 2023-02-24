using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSaleController : MonoBehaviour
{
    [SerializeField] GameObject[] FS_01, FS_02, FS_03, FS_04;
    public bool CanShowFlashSale()
    {
        float waitTime = PlayerPrefs.GetFloat("fs_waittime");
        float packTime = PlayerPrefs.GetFloat("FLASH_SALE", 0);
        double remainTime = DataController.ConvertToUnixTime(System.DateTime.UtcNow) - packTime;
        if (DataController.Instance.GetLevelState(2, 5) > 0 && remainTime > waitTime)
        {
            var playedLevelData = LevelDataController.Instance.playedLevelData;
            if (playedLevelData == null) return false;
            List<Items> usedItemList = new List<Items>();
            if (playedLevelData.result == 1)// win game
            {

                if (playedLevelData.UsedBoosterList != null && playedLevelData.UsedBoosterList.Count > 0)
                {

                    for (int i = 0; i < playedLevelData.UsedBoosterList.Count; i++)
                    {
                        int itemId = playedLevelData.UsedBoosterList[i];
                        usedItemList.Add(new Items(itemId, DataController.Instance.GetItemQuantity(itemId)));
                    }
                    int id = GetIdOfSmallestQuantity(usedItemList);
                    DisplayFlashSale(id);
                }
                else
                {
                    int[] itemArray = DataController.Instance.GetAllItemIds();
                    for (int i = 0; i < itemArray.Length; i++)
                    {
                        int itemId = itemArray[i];
                        usedItemList.Add(new Items(itemId, DataController.Instance.GetItemQuantity(itemId)));
                    }
                    int id = GetIdOfSmallestQuantity(usedItemList);
                    DisplayFlashSale(id);
                }
                return true;
            }
            else
            {
                if (playedLevelData.loseType == 0)
                {
                    int[] itemArray = DataController.Instance.GetAllItemIds();
                    for (int i = 0; i < itemArray.Length; i++)
                    {
                        int itemId = itemArray[i];
                        usedItemList.Add(new Items(itemId, DataController.Instance.GetItemQuantity(itemId)));
                    }
                    int id = GetIdOfSmallestQuantity(usedItemList);
                    DisplayFlashSale(id);
                }
                else
                    DisplayFlashSale(playedLevelData.loseType);
                return true;
            }
        }
        return false;
    }
    public int GetIdOfSmallestQuantity(List<Items> itemList)
    {
        int quantity = itemList[0].itemQuantity;
        int id = itemList[0].itemId;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].itemQuantity <= quantity)
                id = itemList[i].itemId;
        }
        return id;
    }
    public void DisplayFlashSale(int itemId)
    {
        int salePackIndex = GetSalePackIndex();

        GameObject go;

        switch (itemId)
        {
            case 100100:
            case 100000:
                go = FS_01[salePackIndex];
                break;
            case 100200:
            case 110000:
                go = FS_02[salePackIndex];
                break;
            case 100300:
            case 120000:
                go = FS_03[salePackIndex];
                break;
            case 13000:
                go = FS_04[salePackIndex];
                break;
            default:
                go = FS_01[salePackIndex];
                break;
        }
        //UIController.Instance.HideAllPanel();
        PlayerPrefs.SetFloat("FLASH_SALE", (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        Instantiate(go, FindObjectOfType<MainMenuController>().cameraCanvas);
    }
    public int GetSalePackIndex()
    {
        int userRank = DataController.Instance.Rank;
        if (userRank <= 1)
            return 0;
        else if (userRank == 2)
            return 1;
        else
            return 2;
    }
}

