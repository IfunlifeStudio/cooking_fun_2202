using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ConditionViolatePanelController : MonoBehaviour
{
    [SerializeField] private Transform mask;
    [SerializeField] private TextMeshProUGUI tutorialMessage;
    private GameObject mainCanvas;
    [SerializeField] private AudioClip popUpClip;
    bool canDestroy = false;
    public void Start()
    {
        DOVirtual.DelayedCall(3f, () => { OnClick(); });
    }
    public void Spawn(Vector3 maskPos)
    {
        UIController.Instance.CanBack = false;
        GameObject go = Instantiate(gameObject);
        go.GetComponent<ConditionViolatePanelController>().Init(maskPos);
    }
    public void Init(Vector3 maskPos)
    {
        mask.position = new Vector3(maskPos.x, maskPos.y, mask.position.z);
        //tutorialMessage.text = message;
        mainCanvas = GameObject.Find("Canvas");
        if (mainCanvas != null)
            mainCanvas.SetActive(false);
        AudioController.Instance.PlaySfx(popUpClip);
        DOVirtual.DelayedCall(1f, () => { canDestroy = true; });
        StartCoroutine(DelayStop());
    }
    public void OnClick()
    {
        if (canDestroy)
        {
            canDestroy = false;
            Time.timeScale = 1;
            UIController.Instance.CanBack = true;
            Destroy(gameObject);
            if (mainCanvas != null)
                mainCanvas.SetActive(true);
        }
    }
    private IEnumerator DelayStop()
    {
        yield return null;
        Time.timeScale = 0;
    }
}
