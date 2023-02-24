using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/LimitTutorial")]
public class TimeLimitTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private int limit = 1;
    [SerializeField] private ConditionTutorialPanelController conditionTutorialPrefab;
    public override void Handle(Message message)
    {
        if ((int)message.data[1] == limit)//Handle condition tutorial
        {
            UIController.Instance.CanBack = false;
            isCompleted = true;
            string tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_time_limit");
            conditionTutorialPrefab.Spawn(tutorialMessage);//instance a tutorial here
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
            TimeLimitTutorial tmp = new TimeLimitTutorial();
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
