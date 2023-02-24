using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackController : UIView
{
    public const string EMAIL_ADDRESS = "games@cscmobi.com", SUBJECT = "Cooking%20Love%20F%20Feedback_id_", BODY = "Your%20feedback%20here:";
    [SerializeField] private AudioClip popUpClip;
    private void Start()
    {
        AudioController.Instance.PlaySfx(popUpClip);
        GetComponent<Animator>().Play("Appear");
    }
    public void Spawn(Transform parent)
    {
        Instantiate(gameObject, parent);
    }
    public void OnClickSendEmail()
    {
        string data = DataController.Instance.GetGameData().userID + SystemInfo.deviceUniqueIdentifier + SystemInfo.deviceModel+PlayerClassifyController.Instance.GetPlayerClassifyLevel();
        Application.OpenURL("mailto:" + EMAIL_ADDRESS + "?subject=" + SUBJECT + data + "&body=" + BODY);
        gameObject.SetActive(false);
        UIController.Instance.PopUiOutStack();
    }
    public override void OnHide()
    {
        GetComponent<Animator>().Play("Disappear");
        gameObject.SetActive(false);
        UIController.Instance.PopUiOutStack();
    }
}
