using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CookingMessageType
{
    OnGameStart = 0,
    OnGameEnd = 1,
    OnWinGame = 2,
    OnLoseGame = 3,
    OnCombo3 = 4,
    OnCombo4 = 5,
    OnCombo5 = 6,
    OnCoinChange = 7,
    OnConditionViolate = 11,
    HeartGoal = 12,
    OnUseItem = 13,
    OnUpgradeIngredient = 14,
    OnUpgradeMachine = 15,
    OnFoodOrder = 101,
    OnFoodServed = 151,
    OnOrderCompleted = 152,
    OnHeartReceived = 153,
    OnOpenLevelSelect = 201,
    OnFocusLevel = 333,
    OnFoodBurned = 301
}
public class Message
{
    public CookingMessageType type;
    public object[] data;
    public Message(CookingMessageType type)
    {
        this.type = type;
    }
    public Message(CookingMessageType type, object[] data)
    {
        this.type = type;
        this.data = data;
    }
}
public interface IMessageHandle
{
    void Handle(Message message);
}
public class MessageManager : MonoBehaviour, ISerializationCallbackReceiver
{
    private static MessageManager instance = null;
    [HideInInspector] public List<CookingMessageType> _keys = new List<CookingMessageType>();
    [HideInInspector] public List<List<IMessageHandle>> _values = new List<List<IMessageHandle>>();
    private Dictionary<CookingMessageType, List<IMessageHandle>> subcribers = new Dictionary<CookingMessageType, List<IMessageHandle>>();
    public static MessageManager Instance { get { return instance; } }
    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void AddSubcriber(CookingMessageType type, IMessageHandle handle)
    {
        if (!subcribers.ContainsKey(type))
            subcribers[type] = new List<IMessageHandle>();
        if (!subcribers[type].Contains(handle))
            subcribers[type].Add(handle);
    }
    public void RemoveSubcriber(CookingMessageType type, IMessageHandle handle)
    {
        if (subcribers.ContainsKey(type))
            if (subcribers[type].Contains(handle))
                subcribers[type].Remove(handle);
    }
    public void SendMessage(Message message)
    {
        if (subcribers.ContainsKey(message.type))
            for (int i = subcribers[message.type].Count - 1; i > -1; i--)
                subcribers[message.type][i].Handle(message);
    }
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var element in subcribers)
        {
            _keys.Add(element.Key);
            _values.Add(element.Value);
        }
    }
    public void OnAfterDeserialize()
    {
    }
}