using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/ItemQuest")]
public class ItemQuest : Quest
{
    [SerializeField] private ItemType itemType = ItemType.Anti_OverCook;
    public int RewardThreshold
    {
        get { return rewardThreshold; }
    }
    public override void Handle(Message message)
    {
        if ((int)itemType == (int)message.data[0])
            count++;
    }
    public override void LoadFromJson()
    {
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER + name.Replace(' ', '_') + ".quest";//handle old save data
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            ItemQuest tmp = new ItemQuest();
            JsonUtility.FromJsonOverwrite(data, tmp);
            isActivated = tmp.isActivated;
            isClaimed = tmp.isClaimed;
            tmpCount = count = tmp.count;
        }
    }
    public override bool MeetUnlockRequirement()
    {
        return DataController.Instance.IsItemUnlocked((int)itemType);
    }
}
