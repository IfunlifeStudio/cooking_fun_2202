using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLevelBtnContainer : MonoBehaviour
{
     public EventLevelBtnController[] levelBtns;
    [SerializeField] private EventLevelBoxController keyBoxPrefab;
    public GameObject Spawn(int chapter, int[] levelsIndex, Transform parent, bool isLastContainer)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<EventLevelBtnContainer>().Init(chapter, levelsIndex, isLastContainer);
        return go;
    }
    public void Init(int chapter, int[] levelsIndex, bool isLastContainer)
    {
        for (int i = 0; i < levelBtns.Length; i++)
        {
            if (i < levelsIndex.Length)
            {
                levelBtns[i].gameObject.SetActive(true);
                levelBtns[i].Init(chapter, levelsIndex[i]);
            }
            else
                levelBtns[i].gameObject.SetActive(false);
        }
        if (isLastContainer)//if lastContainer, display a key box
            keyBoxPrefab.Spawn(levelBtns[levelsIndex.Length].transform.localPosition, transform);
    }
    public EventLevelBtnController GetLevelBtn(int chapter, int id)
    {
        foreach (var levelBtn in levelBtns)
            if (levelBtn.chapter == chapter && levelBtn.index == id)
                return levelBtn;
        return null;
    }
}
