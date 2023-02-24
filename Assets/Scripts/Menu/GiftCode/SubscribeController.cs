using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeController : MonoBehaviour
{
    [SerializeField] GameObject subscribeBtn;
    [SerializeField] SubcribePanelController subcribePanel;
    // Start is called before the first frame update
    void Start()
    {
        SetSubscribeBtnActive(DataController.Instance.Subcribed != 1 && DataController.Instance.GetLevelState(1, 7) > 0);
    }
    public void SetSubscribeBtnActive(bool status)
    {
        subscribeBtn.SetActive(status);
    }
    public void OnClickSubscribe()
    {
        Instantiate(subcribePanel, FindObjectOfType<MainMenuController>().cameraCanvas);
    }
}
