using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassScrollViewController : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private BpElementController battlePassElementPrefab;
    private int[] goDistance;
    private int minGoNum;
    private bool ForceToMove = false;
    private List<BpElementController> elementList;
    public void Init(BattlePassElementData[] bpLevelDatas, int currentBpLevel)
    {
        if (elementList == null || elementList.Count == 0)
        {
            elementList = new List<BpElementController>();
            for (int i = 0; i < bpLevelDatas.Length; i++)
            {
                var go = Instantiate(battlePassElementPrefab, content);
                bool activeLockBar = bpLevelDatas[i].Level == currentBpLevel;
                go.InItBpElement(bpLevelDatas[i], activeLockBar);
                elementList.Add(go);
            }
            Setup();
        }
        else
        {
            for (int i = 0; i < elementList.Count; i++)
            {
                bool activeLockBar = (bpLevelDatas[i].Level == BattlePassDataController.BP_MAX_LEVEL) ? false : bpLevelDatas[i].Level == currentBpLevel;
                elementList[i].InItBpElement(bpLevelDatas[i], activeLockBar);
            }
        }
        MoveToElementIndex(currentBpLevel);
    }
    public void Setup()
    {
        var gOs = new GameObject[content.childCount];
        for (int i = 0; i < content.childCount; i++)
            gOs[i] = content.GetChild(i).gameObject;
        int goLength = gOs.Length;
        goDistance = new int[goLength];
        goDistance[0] = 0;
        for (int i = 1; i < goDistance.Length; i++)
        {
            goDistance[i] = (int)Mathf.Abs(gOs[i].GetComponent<RectTransform>().rect.height * (1 - i));
        }

    }
    private void Update()
    {
        if (ForceToMove)
        {
            LerpToGo(goDistance[minGoNum]);
        }
    }
    void LerpToGo(int pos)
    {
        float newY = Mathf.Lerp(content.anchoredPosition.y, pos, Time.deltaTime * 4.5f);
        Vector2 newPos = new Vector2(content.anchoredPosition.x, newY);
        content.anchoredPosition = newPos;
        if (Mathf.Abs(content.anchoredPosition.y - pos) < 20 && ForceToMove)
            ForceToMove = false;
    }
    public void MoveToElementIndex(int elementIndex)
    {
        elementIndex = elementIndex == BattlePassDataController.BP_MAX_LEVEL ? elementIndex - 1 : elementIndex;
        minGoNum = Mathf.Max(elementIndex - 1, 0);
        ForceToMove = true;
    }
}
