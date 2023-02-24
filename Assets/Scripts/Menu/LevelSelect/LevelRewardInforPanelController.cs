using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardInforPanelController : MonoBehaviour
{
    [SerializeField] RewardItemController rewardPrefabs;
    [SerializeField] Transform content;
    bool hasShowPanel;
    public void SetUpPanel(int rewardMilestone)
    {
        hasShowPanel = true;
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
        switch (rewardMilestone)
        {
            case 1:
                SpawnReward(0, 0, new int[] { 100200 }, new int[] { 1 });
                break;
            case 2:
                SpawnReward(0, 10, new int[] { 100100 }, new int[] { 1 });
                break;
            case 3:
                SpawnReward(0, 15, new int[] { 100300 }, new int[] { 1 });
                break;
            case 4:
                SpawnReward(0, 20, new int[] { 100200 }, new int[] { 2 });
                break;
            case 5:
                int currentChapter = DataController.Instance.currentChapter;
                if (currentChapter <= 4)
                    SpawnReward(40, 0, new int[] { 100200, currentChapter + 3 }, new int[] { 2, 1 });
                else
                    SpawnReward(40, 0, new int[] { 100200 }, new int[] { 2 });
                break;
        }
        TurnPanelOn();
    }
    public void SpawnReward(int rubyQuantity, int energyTime, int[] itemIds, int[] itemQuantities)
    {
        if (rubyQuantity > 0)
        {
            Instantiate(rewardPrefabs, content).Init(100500, rubyQuantity);
        }
        if (energyTime > 0)
            Instantiate(rewardPrefabs, content).Init(100600, energyTime);
        if (itemIds.Length > 0)
        {
            for (int i = 0; i < itemIds.Length; i++)
            {
                Instantiate(rewardPrefabs, content).Init(itemIds[i], itemQuantities[i]);
            }
        }
    }
    public void TurnPanelOff()
    {
        gameObject.SetActive(false);
    }
    public void TurnPanelOn()
    {
        gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && hasShowPanel)
        {
            TurnPanelOff();
        }
    }
}
