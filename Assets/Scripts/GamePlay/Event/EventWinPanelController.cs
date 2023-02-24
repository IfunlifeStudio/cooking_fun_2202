using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
public class EventWinPanelController : WinPanelController
{
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI[] levelsText;
    [SerializeField] private Image[] levelsIcon, ticks, focusFrames;
    [SerializeField] private Sprite[] levelIconSprites;
    public new void Init(int goldCollected, int xpReward)
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().Play("Appear");
        //float eventTimeStamp = PlayerPrefs.GetFloat(EventDataController.EVENT_TIME_STAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        // int deltaTime = (int)(3 * 24 * 60 * 60 - DataController.ConvertToUnixTime(System.DateTime.UtcNow) + eventTimeStamp);
        // countDownText.text = deltaTime / (60 * 60) + "h " + (deltaTime / 60) % 60 + "m";
        LevelData levelData = LevelDataController.Instance.currentLevel;
        LevelDataController.Instance.lastestPassedLevel = levelData;
        EventDataController.Instance.SetLevelState(levelData.chapter, levelData.id, 1);
        int levelOffset = ((levelData.id - 1) / 5) * 5;
        for (int i = 1; i < 6; i++)
        {
            levelsText[i - 1].text = (levelOffset + i).ToString();
            ticks[i - 1].gameObject.SetActive(false);
            if (EventDataController.Instance.GetLevelState(levelData.chapter, levelOffset + i) > 0)
            {
                levelsIcon[i - 1].sprite = levelIconSprites[0];
                ticks[i - 1].gameObject.SetActive(true);
            }
            else if (EventDataController.Instance.GetNextLevel(levelData.chapter) == levelOffset + i)
            {
                levelsIcon[i - 1].transform.parent.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                levelsIcon[i - 1].sprite = levelIconSprites[1];
                focusFrames[i - 1].gameObject.SetActive(true);
            }
            else
                levelsIcon[i - 1].sprite = levelIconSprites[2];
        }
    }
}
