using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Lean.Localization;
public class ItemAchivement : MonoBehaviour
{
    public Image imgMedal;
    public Sprite[] sprMedals;
    public Image[] imgStars;
    public Sprite sprStarBackGround, sprStarEnable;
    public TextMeshProUGUI txtName, txtDes, txtRuby, txtComplete;
    public GameObject BtnClaim, BtnClaimDisable;
    public Image progressBar;
    public Transform imgRuby;
    public GameObject Reward;
    public void SetUpData()
    {
        //AchivementID achivementID = (AchivementID)int.Parse(gameObject.name);
        //var aData = AchivementController.Instance.GetAchivementData(achivementID);
        //var aDataDefault = AchivementController.Instance.GetAchivementDataDefault(achivementID);
        //txtName.text = LeanLocalization.GetTranslationText("achi_Name_" + achivementID.ToString());
        //int current = aData.idDone;
        //if (current > 2)
        //    current = 2;
        //txtRuby.text = aDataDefault.Reward[current].ToString();

        //imgMedal.sprite = sprMedals[aData.idDone];
        //for (int i = 0; i < 3; i++)
        //{
        //    imgStars[i].sprite = i < aData.idDone ? sprStarEnable : sprStarBackGround;
        //}
        //Reward.SetActive(aData.idDone < 3);
        //BtnClaim.SetActive(aData.idDone <= 3 && aData.CurrentValue >= aDataDefault.Target[current]);
        //BtnClaimDisable.SetActive(aData.CurrentValue < aDataDefault.Target[current]);
        //if (aData.idDone >= 3 || aData.CurrentValue >= aDataDefault.Target[current])
        //{
        //    txtComplete.text = LeanLocalization.GetTranslationText("achi_complete");
        //    progressBar.fillAmount = 1;
        //}
        //else
        //{
        //    txtComplete.text = aData.CurrentValue + "/" + aDataDefault.Target[current];
        //    progressBar.fillAmount = (aData.CurrentValue * 1f) / aDataDefault.Target[current];
        //}
        // Debug.Log("achi_Des_" + achivementID.ToString());
        // Debug.Log(LeanLocalization.GetTranslationText("achi_Des_" + achivementID.ToString()));
        //txtDes.text = string.Format(LeanLocalization.GetTranslationText("achi_Des_" + achivementID.ToString()), aDataDefault.Target[current]);
    }

    public void OnBtnClaimClick()
    {
        //AchivementID achivementID = (AchivementID)int.Parse(gameObject.name);
        //var aData = AchivementController.Instance.GetAchivementData(achivementID);
        //var aDataDefault = AchivementController.Instance.GetAchivementDataDefault(achivementID);
        ////int current = aData.idDone;
        ////FindObjectOfType<MainMenuController>().IncreaseGem(imgRuby.position, aDataDefault.Reward[current]);
        ////DataController.Instance.Ruby += aDataDefault.Reward[current];
        //DataController.Instance.SaveData();
        ////aData.idDone += 1;
        //SetUpData();
        //ReSortItemAchivement();
    }

    public void ReSortItemAchivement()
    {
        //AchivementID achivementID = (AchivementID)int.Parse(gameObject.name);
        //var aData = AchivementController.Instance.GetAchivementData(achivementID);
        //var aDataDefault = AchivementController.Instance.GetAchivementDataDefault(achivementID);
        //int current = aData.idDone;
        //if (aData.idDone > 2)
        //{
        //    gameObject.transform.SetAsLastSibling();
        //}
        //else if (aData.CurrentValue >= aDataDefault.Target[current])
        //{
        //    gameObject.transform.SetAsFirstSibling();
        //}
    }
}
