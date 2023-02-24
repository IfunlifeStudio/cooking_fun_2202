using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/ConditionViolateTutorial")]
public class ConditionViolateTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private int condition = 1;
    [SerializeField] private ConditionViolatePanelController conditionViolateTutorialPrefab;
    public override void Handle(Message message)
    {
        if ((int)message.data[0] == condition)//Handle condition tutorial
        {
            //string tutorialMessage = "";
            //if ((int)message.data[0] == 1)//burn food
            //    tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_burn_violate");
            //else if ((int)message.data[0] == 2)//throw food
            //    tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_throw_violate");
            //else if ((int)message.data[0] == 3)//customer leave
            //    tutorialMessage = Lean.Localization.LeanLocalization.GetTranslationText("tut_cus_leave_violate");
            conditionViolateTutorialPrefab.Spawn((Vector3)message.data[1]);
        }
    }
    public override void LoadFromJson()
    {
        //var completedTutorials = DataController.Instance.GetTutorialData();
        //if (completedTutorials.Contains(id))
        //{
        //    isCompleted = true;
        //    return;
        //}
        //string savePath = Application.persistentDataPath + SAVE_FOLDER + name + ".gev";//handle old save data
        //if (System.IO.File.Exists(savePath))
        //{
        //    string data = System.IO.File.ReadAllText(savePath);
        //    ConditionViolateTutorial tmp = new ConditionViolateTutorial();
        //    JsonUtility.FromJsonOverwrite(data, tmp);
        //    if (tmp.isCompleted && !completedTutorials.Contains(id))
        //    {
        //        completedTutorials.Add(id);
        //        isCompleted = true;
        //    }
        //}
    }
    public override void Reset()
    {
        isCompleted = false;
    }
}
