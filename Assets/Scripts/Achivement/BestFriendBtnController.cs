using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BestFriendBtnController : MonoBehaviour
{
    [SerializeField] int achievement_ID;
    string certi_Title;
    string certi_Color;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        certi_Title = GetAchievement(achievement_ID);
        certi_Color = GetAchieveColor(achievement_ID);
        if (certi_Title != "")
        {
            gameObject.name = certi_Title;
            string name = Lean.Localization.LeanLocalization.GetTranslationText(certi_Title);
            if (certi_Color != "" && name != "")
                GetComponentInChildren<TextMeshProUGUI>().text = "<color=#" + certi_Color + ">" + name + "</color> ";
            gameObject.SetActive(true);
        }
    }
    private string GetAchievement(int id)
    {
        switch (id)
        {
            case 0:

                if (DataController.Instance.HighestRestaurant > 1)
                {
                    int totalStageInRes = DataController.Instance.GetTotalLevelsPerRestaurant(DataController.Instance.HighestRestaurant - 1) * 3;
                    int totalStageGranted = DataController.Instance.GetTotalStageGranted(DataController.Instance.HighestRestaurant - 1);
                    if (totalStageGranted == totalStageInRes)
                        return AchivementController.Instance.GetWinrateCerti(DataController.Instance.HighestRestaurant - 1);
                }
                return "";
            case 1:
                var profileDatas = DataController.Instance.GetGameData().profileDatas;
                if (profileDatas.TotalCombo >= 2000)
                    return "combo_legend";
                else if (profileDatas.TotalCombo >= 1000)
                    return "combo_master";
                else if (profileDatas.TotalCombo >= 500)
                    return "combo_expert";
                else if (profileDatas.TotalCombo >= 100)
                    return "combo_beginner";
                else return "";
            case 2:
                if (DataController.Instance.HighestRestaurant > 1)
                    return AchivementController.Instance.GetUnlockResCerti(DataController.Instance.HighestRestaurant);
                return "";
            case 3:
                return AchivementController.Instance.GetFlashHandCerti();
            case 4:
                if (DataController.Instance.GetGameData().userIapValue >= 1000)
                    return "best_friend";
                return "";
            case 5:
                if (PlayerPrefs.GetInt("has_buy_starter_pack", 0) != 0)
                    return "play_boy";
                return "";
            case 6:
                PlayerPrefs.GetInt("ruby_spent", 0);
                float rb_spent_time = PlayerPrefs.GetFloat("ruby_spent_time", 0);
                if (DataController.ConvertToUnixTime(System.DateTime.UtcNow) - rb_spent_time < 604800 && PlayerPrefs.GetInt("ruby_spent", 0) >= 1000)
                    return "rick_kid";
                return "";
        }
        return "";
    }
    public string GetAchieveColor(int id)
    {
        switch (id)
        {
            case 0:
                return AchivementController.Instance.GetWinrateCerti(DataController.Instance.HighestRestaurant - 1, false);
            //return AchivementController.Instance.GetAchievementColorWithResID(DataController.Instance.HighestRestaurant - 1, AchivementID.Achivement_1);
            case 1:
                var profileDatas = DataController.Instance.GetGameData().profileDatas;
                if (profileDatas.TotalCombo >= 2000)
                    return "ff8024";
                else if (profileDatas.TotalCombo >= 1000)
                    return "9637d8";
                else if (profileDatas.TotalCombo >= 500)
                    return "0282dc";
                else if (profileDatas.TotalCombo >= 100)
                    return "00bb30";
                else return " ";
            case 2:
                return AchivementController.Instance.GetUnlockResCerti(DataController.Instance.HighestRestaurant, false);
            case 3:
                return "0282dc";
            case 4:
                return "FF8024";
            case 9:
                return "FF8024";
            case 10:
                return "9637D8";
            case 11:
                return "9637D8";
        }
        return "9637D8";
    }
    public void OnClickTogge(bool state)
    {
        if (state)
        {
            if (certi_Title != "" && certi_Title != null)
                DataController.Instance.GetGameData().profileDatas.CertiTile = certi_Title;
            if (certi_Color != "" && certi_Color != null)
                DataController.Instance.GetGameData().profileDatas.CertiColor = certi_Color;
        }
    }
}
