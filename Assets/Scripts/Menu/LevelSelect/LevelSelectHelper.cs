using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelSelectHelper : MonoBehaviour
{
    private const string HELPER_TIMESTAMP = "helper_timestamp";
    [SerializeField] private UpgradeHelperController upgradeHelperPrefab;
    [SerializeField] private ItemHelperController itemHelperPrefab;
    [SerializeField] private Transform cameraCanvas;
    public bool CanShowHelper()
    {
        if (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - PlayerPrefs.GetFloat(HELPER_TIMESTAMP, 0) < 1800)
            return false;//30 minutes cooldowns
        if (!CanUpgradeMachine() && !CanUpgradeIngredients() && !ShouldUseBoost()) return false;
        return true;
    }
    public void OpenHelperPanel()
    {
        PlayerPrefs.SetFloat(HELPER_TIMESTAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        //if (CanUpgradeMachine() || CanUpgradeIngredients())
        //    Instantiate(upgradeHelperPrefab, cameraCanvas);
        //else
        if (ShouldUseBoost())
            Instantiate(itemHelperPrefab, cameraCanvas);
    }
    public bool CanUpgradeMachine()
    {
        if (DataController.Instance.GetLevelState(1, 7) < 1) return false;
        int totalMoney = DataController.Instance.Gold + DataController.Instance.Ruby * 20;
        List<int> machineIds = new List<int>();
        int chapterId = DataController.Instance.currentChapter;
        var machineDatas = DataController.Instance.GetRestaurantById(chapterId).machines;
        foreach (var machineData in machineDatas)
            machineIds.Add(machineData.id);
        for (int i = 0; i < machineIds.Count; i++)
            if (DataController.Instance.GetMachineUpgradeCost(machineIds[i]) < totalMoney && DataController.Instance.IsMachineUnlocked(machineIds[i]) && machineDatas[i].level < 3)
                return true;
        return false;
    }
    public bool CanUpgradeIngredients()
    {
        if (DataController.Instance.GetLevelState(1, 7) < 1) return false;
        int totalMoney = DataController.Instance.Gold + DataController.Instance.Ruby * 20;
        List<int> ingredientIds = new List<int>();
        int chapterId = DataController.Instance.currentChapter;
        var ingredientDatas = DataController.Instance.GetRestaurantById(chapterId).ingredients;
        foreach (var ingredientData in ingredientDatas)
            ingredientIds.Add(ingredientData.id);
        for(int i=0;i<ingredientIds.Count;i++)
            if(DataController.Instance.GetIngredientUpgradeCost(ingredientIds[i]) < totalMoney && DataController.Instance.IsIngredientUnlocked(ingredientIds[i]) && ingredientDatas[i].level<3)
                return true;
        return false;
    }
    private bool ShouldUseBoost()
    {
        int itemId = LevelDataController.Instance.GetRecommendItemId();
        int itemCount = 0;
        if (itemId > 0)
            itemCount = DataController.Instance.GetItemQuantity(itemId);
        return itemId != 0 && itemCount > 0;
    }
}
