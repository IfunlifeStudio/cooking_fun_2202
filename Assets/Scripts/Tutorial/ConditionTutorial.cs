using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/ConditionTutorial")]
public class ConditionTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private int condition = 1;
    [SerializeField] private ConditionTutorialPanelController conditionTutorialPrefab;
    public override void Handle(Message message)
    {
        if ((int)message.data[2] == condition)//Handle condition tutorial
        {
            isCompleted = true;
            string tutorialMessage = "";
            if ((int)message.data[2] == 1)//burn food
            {
                tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_burned_condidtion");
            }
            else if ((int)message.data[2] == 2)//throw food
            {
                tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_throw_condidtion");
            }
            else if ((int)message.data[2] == 3)//customer leave
            {
                tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_cus_leave_condition");
            }
            conditionTutorialPrefab.Spawn(tutorialMessage);
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
            ConditionTutorial tmp = new ConditionTutorial();
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
