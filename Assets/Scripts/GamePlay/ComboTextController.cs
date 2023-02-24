using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ComboTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    public void Spawn(Vector3 spawnPos, Transform parent, string comboValue)
    {
        GameObject go = Instantiate(gameObject, spawnPos, Quaternion.identity, parent);
        go.GetComponent<ComboTextController>().Init(comboValue);
    }
    public void Init(string comboValue)
    {
        comboText.text = comboValue;
        Destroy(gameObject, 2f);
    }
}
