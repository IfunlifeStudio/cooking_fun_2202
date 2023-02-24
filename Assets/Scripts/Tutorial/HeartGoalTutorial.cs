using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/HeartGoalTutorial")]
public class HeartGoalTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private HeartTutorialPanel heartTutorialPrefab;
    public override void Handle(Message message)
    {
        UIController.Instance.CanBack = false;
        isCompleted = true;
        heartTutorialPrefab.Spawn((Vector3)message.data[0]);//instance a tutorial here
        UnRegister();
    }
    public override void LoadFromJson()
    {
        var completedTutorials = DataController.Instance.GetTutorialData();
        if (completedTutorials.Contains(id))
        {
            isCompleted = true;
            return;
        }
        string savePath = Application.persistentDataPath + SAVE_FOLDER + name + ".gev";//handle old save data
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            HeartGoalTutorial tmp = new HeartGoalTutorial();
            JsonUtility.FromJsonOverwrite(data, tmp);
            if (tmp.isCompleted && !completedTutorials.Contains(id))
            {
                completedTutorials.Add(id);
                isCompleted = true;
            }
        }
    }
    public override void Reset()
    {
        isCompleted = false;
    }
}
