using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendWavePanelController : FriendController
{
    [SerializeField] GameObject descriptionTxtGo;
    List<FriendItem> inActiveFrList = new List<FriendItem>();
    bool hasInit = false;
    public override void Init(List<ProfileData> friendDataList)
    {
        if (!hasInit)
        {
            DatabaseController.Instance.SpawnLoading();
            hasInit = true;
            int order = 0;
            foreach (var fr in friendDataList)
            {
                string userID = DataController.Instance.GetGameData().profileDatas.FBUserID;
                if (fr.FBUserID != userID && fr.FBUserID != "9999")
                    if (!DataController.Instance.GetGameData().WavedFriendList.Contains(fr.FBUserID))
                    {
                        double timenow = DataController.ConvertToUnixTime(DateTime.UtcNow);
                        double waveTimeCache = 0;
                        try
                        {
                            waveTimeCache = Convert.ToDouble(PlayerPrefs.GetString("wave_time_cache", timenow.ToString()));
                        }
                        catch { Debug.LogError("Not convert wave time to double"); }                  
                        //var value = await FirebaseServiceController.Instance.GetFriendProfileFieldValue(fr.FBUserID, "LastActiveTime");
                        if (timenow - fr.LastActiveTime >= 259200 && waveTimeCache <= timenow)
                        {
                            order++;
                            PlayerPrefs.SetString("wave_time_cache", timenow.ToString());
                            FriendWaveItemController go = SpawnFriend(fr, order, FireStoreColection.Help).GetComponent<FriendWaveItemController>();
                            go.GetComponentInChildren<Button>().onClick.AddListener(RemoveFriendFromList);
                            inActiveFrList.Add(go);
                        }
                    }
            }
            descriptionTxtGo.SetActive(order == 0);
            DatabaseController.Instance.DespawnLoadingOverlay();
        }
    }
    public void RemoveFriendFromList()
    {
        inActiveFrList.RemoveAt(inActiveFrList.Count - 1);
        descriptionTxtGo.SetActive(inActiveFrList.Count == 0);
    }
}
