using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HeartTutorialPanel : MonoBehaviour
{
    private Vector3 maskPos;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Transform mask;
    [SerializeField] private TextMeshProUGUI tutorialMessage;
    private int clickCount;
    public void Spawn(Vector3 heartPos)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<HeartTutorialPanel>().Init(heartPos);
    }
    public void Init(Vector3 heartPos)
    {
        this.maskPos = heartPos;
        tutorialMessage.text = Lean.Localization.LeanLocalization.GetTranslationText("tut_heart_1");
        AudioController.Instance.PlaySfx(popUpClip);
        //tutorialMessage.text = "You need to collect likes from customer to pass this level.";
        clickCount = 0;
        Time.timeScale = 0;
    }
    public void OnClick()
    {
        if (clickCount == 0)
        {
            clickCount = 1;
            mask.position = maskPos;
            tutorialMessage.text = Lean.Localization.LeanLocalization.GetTranslationText("tut_heart_2");
        }
        else
        {
            UIController.Instance.CanBack = true;
            Time.timeScale = 1;
            Destroy(gameObject);
        }
    }
}
