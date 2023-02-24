using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSnapController : MonoBehaviour
{
    public RectTransform panel;
    public Transform content;
    public GameObject[] gOs;
    public RectTransform center;
    public float[] distance;
    public bool dragging = false;
    public int[] goDistance;
    public int minGoNum;
    public bool ForceToMove = false;

    public void ReDstance()
    {
        gOs = new GameObject[content.childCount];
        for (int i = 0; i < content.childCount; i++)
            gOs[i] = content.GetChild(i).gameObject;
        int goLength = gOs.Length;
        distance = new float[goLength];
        goDistance = new int[goLength];
        goDistance[0] = 0;
        for (int i = 1; i < goDistance.Length; i++)
            goDistance[i] = (int)Mathf.Abs(gOs[i].GetComponent<RectTransform>().anchoredPosition.x - gOs[0].GetComponent<RectTransform>().anchoredPosition.x - 250);
    }
    private void Update()
    {
        for (int i = 0; i < gOs.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - gOs[i].transform.position.x);
        }
        // if (!ForceToMove)
        // {
        //     float minDistance = Mathf.Min(distance);
        //     for (int i = 0; i < gOs.Length; i++)
        //         if (minDistance == distance[i])
        //             minGoNum = i;
        // }
        // if (!dragging)
        //     LerpToGo(-goDistance[minGoNum]);
        if (ForceToMove)
            LerpToGo(-goDistance[minGoNum]);
    }

    void LerpToGo(int pos)
    {
        float newX = Mathf.Lerp(panel.anchoredPosition.x, pos, Time.deltaTime * 2.5f);
        Vector2 newPos = new Vector2(newX, panel.anchoredPosition.y);
        panel.anchoredPosition = newPos;
        if (Mathf.Abs(panel.anchoredPosition.x - pos) < 20 && ForceToMove)
            ForceToMove = false;
    }
    public void MoveToObject(string objName)
    {

        for (int i = 0; i < gOs.Length; i++)
        {
            if (gOs[i].name == objName)
            {
                minGoNum = i;
                break;
            }
        }
        ForceToMove = true;
    }
    public void StarDrag()
    {
        dragging = true;
        ForceToMove = false;
    }

    public void EndDrag()
    {
        dragging = false;
    }
}
