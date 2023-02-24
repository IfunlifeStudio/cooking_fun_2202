using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FriendGiftItemController : FriendItem
{
    [SerializeField] Image medal, requestBtnImg;
    [SerializeField] Sprite[] medalSprites, btnSprites;//0:energy, 1:ruby
    [SerializeField] Transform avatarTransform;
    FireStoreColection currentAction;
    [SerializeField] TextMeshProUGUI desText, amount;
    public override void Init(ProfileData frData, int order, FireStoreColection action, Transform parent)
    {
        transform.parent = parent;
        if (action == FireStoreColection.GiveEnergy)
        {
            desText.text = Lean.Localization.LeanLocalization.GetTranslationText("ft_gift_energy");
            DisplayForEnergy();
        }
        else if (action == FireStoreColection.Help)
        {
            desText.text = Lean.Localization.LeanLocalization.GetTranslationText("ft_gift_help");
            DisplayForEnergy();
        }
        else if (action == FireStoreColection.GiveRuby)
        {
            desText.text = Lean.Localization.LeanLocalization.GetTranslationText("ft_gift_ruby");
            DisplayForRuby();
        }
        else if (action == FireStoreColection.Wave)
        {
            desText.text = Lean.Localization.LeanLocalization.GetTranslationText("ft_gift_wave");
            DisplayForRuby();
        }
        currentAction = action;
        Id = frData.FBUserID;
        nameText.text = frData.FullName;
        progressText.text = frData.KeyGranted.ToString();
        if (order < 4 && frData.FBUserID != "9999")
        {
            medal.gameObject.SetActive(true);
            medal.sprite = medalSprites[order - 1];
        }
        else
            medal.gameObject.SetActive(false);
        var avatar = Instantiate(avatarPrefab, avatarTransform);
        avatar.Init(frData);
    }

    void DisplayForEnergy()
    {
        requestBtnImg.sprite = btnSprites[0];
        amount.text = "x1";
    }
    void DisplayForRuby()
    {
        requestBtnImg.sprite = btnSprites[1];
        amount.text = "x20";
    }
    public override void SendRequest()
    {
        if (APIController.Instance.CanConnetInternet())
            CanRequest();
    }
    void OnClaimEnergy()
    {
        //FindObjectOfType<MainMenuController>().IncreaseEnergy(transform.position, 1);
        FindObjectOfType<EnergyController>().IncreaseOneEnergy();
    }
    void OnClaimRuby(int ruby)
    {
        FindObjectOfType<MainMenuController>().IncreaseGem(transform.position, ruby);
        DataController.Instance.Ruby += ruby;
    }
    public override void SetRequestBtnActive(bool state)
    {
        requestBtn.interactable = state;
    }

    public bool CanRequest()
    {
        switch (currentAction)
        {
            case FireStoreColection.Help:
                if (DataController.Instance.Energy < 5 && DataController.Instance.UnlimitedEnergyDuration <= 0)
                {
                    gameObject.SetActive(false);
                    OnClaimEnergy();
                    FirebaseServiceController.Instance.SendEnergyToFriend(Id);
                    APIController.Instance.LogEventFBFriendAction("claim_energy", 1);
                    DataController.Instance.frNeedHelpList.Remove(Id);
                    DataController.Instance.SaveData(false);
                    return true;
                }
                else
                    UIController.Instance.ShowNotiFy(NotifyType.FullEnergy);
                break;
            case FireStoreColection.GiveEnergy:
                if (DataController.Instance.Energy < 5 && DataController.Instance.UnlimitedEnergyDuration <= 0)
                {
                    gameObject.SetActive(false);
                    OnClaimEnergy();
                    FirebaseServiceController.Instance.DeleteFriendInAction(FireStoreColection.GiveEnergy.ToString(), Id);
                    DataController.Instance.frGivenEnergyList.Remove(Id);
                    DataController.Instance.SaveData(false);
                    return true;
                }
                else
                    UIController.Instance.ShowNotiFy(NotifyType.FullEnergy);
                break;
            case FireStoreColection.Wave:
                gameObject.SetActive(false);
                int rubyValue = PlayerPrefs.GetInt("wave_ruby_value", 1);
                OnClaimRuby(rubyValue);
                APIController.Instance.LogEventFBFriendAction("claim_wave", rubyValue);
                APIController.Instance.LogEventEarnRuby(rubyValue, "wave");
                FirebaseServiceController.Instance.SendRubyToFriend(Id);
                DataController.Instance.frWaveList.Remove(Id);
                DataController.Instance.SaveData(false);
                return true;
            case FireStoreColection.GiveRuby:
                gameObject.SetActive(false);
                int rubyValue1 = PlayerPrefs.GetInt("wave_ruby_value", 1);
                OnClaimRuby(rubyValue1);
                FirebaseServiceController.Instance.DeleteFriendInAction(FireStoreColection.GiveRuby.ToString(), Id);
                DataController.Instance.frGivenRubyList.Remove(Id);
                APIController.Instance.LogEventEarnRuby(rubyValue1, "wave");
                if (DataController.Instance.GetGameData().WavedFriendList.Contains(Id))
                    DataController.Instance.GetGameData().WavedFriendList.Remove(Id);
                DataController.Instance.SaveData(false);
                return true;
        }
        return false;
    }
}
