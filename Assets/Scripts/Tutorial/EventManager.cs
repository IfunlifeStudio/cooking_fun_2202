using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public List<GameEvent> registerEvents = new List<GameEvent>();
    private bool hasInit = false;
    private void Awake()
    {
        Application.runInBackground = true;
    }
    private void Start()
    {
            if (PlayerPrefs.GetInt("FirstTimeInitGameEvent", 0) == 0)
            {
                foreach (GameEvent ge in registerEvents)
                    ge.Reset();
                PlayerPrefs.SetInt("FirstTimeInitGameEvent", 1);
            }
            DontDestroyOnLoad(gameObject);       
    }
    private void UnRegisterGameEvent()
    {
        foreach (GameEvent ge in registerEvents)
            ge.UnRegister();
    }
    private void RegisterGameEvent()
    {
        foreach (GameEvent ge in registerEvents)
            if (!ge.isCompleted) ge.Register();
    }
    public void InitiateGameEvent()
    {
        if (hasInit) return;
        hasInit = true;
        UnRegisterGameEvent();
        LoadFromJson();
        RegisterGameEvent();
    }
    public void SaveToJson()
    {
        foreach (GameEvent ge in registerEvents)
            ge.SaveToJson();
    }
    private void LoadFromJson()
    {
        foreach (GameEvent ge in registerEvents)
            ge.LoadFromJson();
    }
}
