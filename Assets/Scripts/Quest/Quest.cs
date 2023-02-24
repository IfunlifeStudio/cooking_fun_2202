using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Quest : ScriptableObject, IMessageHandle, System.IComparable<Quest>
{
    public int id, tier = 1, coinReward, rewardThreshold, count = 0, tmpCount = 0;
    public bool isClaimed, isActivated;
    [SerializeField] protected string nameKey, defaultName, descriptionKey, description;
    [SerializeField] protected CookingMessageType messageType;
    public string Name
    {
        get { return Lean.Localization.LeanLocalization.GetTranslationText(nameKey, defaultName); }
    }
    public string Description
    {
        get { return string.Format(Lean.Localization.LeanLocalization.GetTranslationText(descriptionKey, description), rewardThreshold); }
    }
    public string Progress
    {
        get
        {
            if (IsCompleted()) return Lean.Localization.LeanLocalization.GetTranslationText("achi_complete", "Completed");
            return string.Format("{0}/{1}", count, rewardThreshold);
        }
    }
    public void Register()
    {
        MessageManager.Instance.AddSubcriber(messageType, this);
    }
    public void UnRegister()
    {
        MessageManager.Instance.RemoveSubcriber(messageType, this);
    }
    public void Active()
    {
        isActivated = true;
        SaveToJson();
    }
    public void Deactive()
    {
        UnRegister();
        Reset();
        SaveToJson();
    }
    public bool IsCompleted()
    {
        return count >= rewardThreshold;
    }
    public abstract bool MeetUnlockRequirement();
    public void Reset()
    {
        count = 0;
        isActivated = isClaimed = false;
    }
    public abstract void Handle(Message message);
    public void SaveToJson()
    {
        string data = JsonUtility.ToJson(this);
        string savePath = Application.persistentDataPath + QuestController.SAVE_FOLDER;
        if (!System.IO.File.Exists(savePath))
            System.IO.Directory.CreateDirectory(savePath);
        System.IO.File.WriteAllText(savePath + name.Replace(' ', '_') + ".quest", data);
    }
    public abstract void LoadFromJson();
    public int CompareTo(Quest other)
    {
        if (other == null) return 1;
        return id.CompareTo(other.id);
    }
}
