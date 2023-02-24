using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendListPanelController : FriendController
{
    List<FriendItem> friendList = new List<FriendItem>();
    [SerializeField] GameObject claimAllBtn;
    bool hasInit = false;
    // Start is called before the first frame update
    public override async void Init(List<ProfileData> friendDataList)
    {
        if (!hasInit)
        {
            hasInit = true;
            string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;

            int order = 0;
            foreach (var fr in friendDataList)
            {
                order++;
                var go = SpawnFriend(fr, order, FireStoreColection.Request).GetComponent<FriendListItemController>();

                if (fr.FBUserID != userID)
                {
                    DateTime reqDateTime;
                    if (fr.FBUserID == "9999")
                    {
                        go.IsBot = true;
                        go.InactiveProfileBtn();
                        string timeTempStr = PlayerPrefs.GetString("bot_energy_timestamp", DataController.ConvertToUnixTime(DateTime.Today.AddDays(-1)).ToString());
                        double timeTemp = Convert.ToDouble(timeTempStr);
                        reqDateTime = DataController.ConvertFromUnixTime(timeTemp);
                    }
                    else
                    {
                        double reqSendTime = await FirebaseServiceController.Instance.GetRequestTime(fr.FBUserID);
                        reqDateTime = DataController.ConvertFromUnixTime(reqSendTime);
                    }
                    bool canReq = CanRequestFriend(reqDateTime);
                    go.SetRequestBtnActive(canReq);
                    if (canReq)
                        friendList.Add(go);
                }
                else
                {
                    go.InactiveRequestBtn();
                    go.InactiveProfileBtn();
                }
            }
            claimAllBtn.GetComponent<Button>().interactable = (friendList.Count != 0);
        }
    }
    private bool CanRequestFriend(DateTime reqDateTime)
    {
        DateTime nowDateTime = DateTime.UtcNow;
        if (nowDateTime.Date > reqDateTime.Date || nowDateTime.Month > reqDateTime.Month || nowDateTime.Year > reqDateTime.Year)
            return true;
        return false;
    }
    public void SendEnergyAll()
    {
        for (int i = 0; i < friendList.Count; i++)
            friendList[i].SendRequest();
        claimAllBtn.GetComponent<Button>().interactable = false;
    }
}
