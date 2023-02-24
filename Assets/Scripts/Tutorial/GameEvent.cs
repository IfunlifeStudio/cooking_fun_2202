using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent : ScriptableObject, IMessageHandle, System.IComparable<GameEvent>
{
    public int id;
    [SerializeField] protected CookingMessageType messageType;
    public bool isCompleted;
    public void Register()
    {
        MessageManager.Instance.AddSubcriber(messageType, this);
    }
    public void UnRegister()
    {
        SaveToJson();
        MessageManager.Instance.RemoveSubcriber(messageType, this);
    }
    public abstract void Reset();
    public abstract void Handle(Message message);
    public void SaveToJson()
    {
        if (isCompleted)
        {
            var completedTutorials = DataController.Instance.GetTutorialData();
            if (!completedTutorials.Contains(id))
            {
                completedTutorials.Add(id);
                DataController.Instance.SaveData();
            }
        }
    }
    public abstract void LoadFromJson();
    public int CompareTo(GameEvent other)
    {
        if (other == null) return 1;
        return id.CompareTo(other.id);
    }
}
