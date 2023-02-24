using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ConditionTutorialPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialMessage;
    [SerializeField] private AudioClip popUpClip;
    public void Spawn(string message)
    {
        UIController.Instance.CanBack = false;
        GameObject go = Instantiate(gameObject);
        go.GetComponent<ConditionTutorialPanelController>().Init(message);
    }
    public void Init(string message)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        tutorialMessage.text = message;
        Time.timeScale = 0;
    }
    public void OnClick()
    {
        UIController.Instance.CanBack = true;
        Time.timeScale = 1;
        Destroy(gameObject);
    }
}
