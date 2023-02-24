using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent/PlayBtnTutorial")]
public class PlayBtnTutorial : GameEvent
{
    private const string SAVE_FOLDER = "/GameEvent/Tutorials/";
    [SerializeField] private int chapter, level;
    [SerializeField] private GameObject playBtnTutorialPrefab;
    public override void Handle(Message message)
    {
        if ((int)message.data[0] == chapter && (int)message.data[1] == level)
        {
            UIController.Instance.CanBack = false;
            isCompleted = true;
            Transform camCanvas = FindObjectOfType<MainMenuController>().cameraCanvas;
            Instantiate(playBtnTutorialPrefab,camCanvas);
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
            PlayBtnTutorial tmp = new PlayBtnTutorial();
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
