using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/LevelQuest")]
public class LevelQuest : Quest
{
    [SerializeField] private int questType = 0;//level quest type: 0 normal, 1 no overcook, 2 no trash,3 serve all customers, 4 full stars
    public int RewardThreshold
    {
        get { return rewardThreshold; }
    }
    public override void Handle(Message message)
    {
        if ((questType == 0) ||//Pass level
        (questType == 1 && (int)message.data[1] == 0) ||//Pass level without overcooking
        (questType == 2 && (int)message.data[2] == 0) ||//Pass level without trash 
        (questType == 3 && (int)message.data[3] == 0) ||//Pass level with serving all customers
        (questType == 4 && (int)message.data[0] == 3))//Pass levels with full stars
            count++;
    }
    public override void LoadFromJson()
    {
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER + name.Replace(' ', '_') + ".quest";//handle old save data
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            LevelQuest tmp = new LevelQuest();
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
