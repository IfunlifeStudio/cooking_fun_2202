using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/CommonQuest")]
public class CommonQuest : Quest
{
    public int RewardThreshold
    {
        get { return rewardThreshold; }
    }
    public override void Handle(Message message)
    {
        count++;
    }
    public override void LoadFromJson()
    {
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER + name.Replace(' ', '_') + ".quest";//handle old save data
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            CommonQuest tmp = new CommonQuest();
            JsonUtility.FromJsonOverwrite(data, tmp);
            isActivated = tmp.isActivated;
            isClaimed = tmp.isClaimed;
            tmpCount = count = tmp.count;
        }
    }
    public override bool MeetUnlockRequirement()
    {
        return true;
    }
}
