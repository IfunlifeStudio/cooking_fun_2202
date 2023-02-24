using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/CoinQuest")]
public class CoinQuest : Quest
{
    public int RewardThreshold
    {
        get { return rewardThreshold; }
    }
    public override void Handle(Message message)
    {
        var goldVal = (int)message.data[0];
        if (goldVal > 0)
            count += goldVal;
    }
    public override void LoadFromJson()
    {
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER + name.Replace(' ', '_') + ".quest";//handle old save data
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            CoinQuest tmp = new CoinQuest();
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
