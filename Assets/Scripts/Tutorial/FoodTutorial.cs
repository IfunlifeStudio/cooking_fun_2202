using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/FoodTutorial")]
public class FoodTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private int foodId = 1;
    [SerializeField] private FoodTutorialPanel foodTutorialPrefab;
    public override void Handle(Message message)
    {
        if ((int)message.data[0] == foodId)
        {
            foodTutorialPrefab.Spawn((Vector3)message.data[1]);
            isCompleted = true;
            UnRegister();
        }
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
            FoodTutorial tmp = new FoodTutorial();
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
