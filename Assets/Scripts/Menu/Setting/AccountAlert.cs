using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AccountAlert : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI localGold, localRuby, /*localLevel,*/ remoteGold, remoteRuby/*, remoteLevel*/, remoteTitle;
    [SerializeField] private Toggle[] toogles;
    [SerializeField] private Button continueBtn;
    public void Spawn(GameData localData, GameData remoteData)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<AccountAlert>().Init(localData, remoteData);
    }
    public void Init(GameData localData, GameData remoteData)
    {
        localGold.text = localData.gold.ToString();
        localRuby.text = localData.ruby.ToString();
        //localLevel.text = localData.Level.ToString();
        remoteGold.text = remoteData.gold.ToString();
        remoteRuby.text = remoteData.ruby.ToString();
        if (PlayerPrefs.GetInt("LOGIN_TYPE", 0) == 1)
            remoteTitle.text = "FaceBook Progress";
        else if (PlayerPrefs.GetInt("LOGIN_TYPE", 0) == 3)
            remoteTitle.text = "Apple Progress";
        //remoteLevel.text = remoteData.Level.ToString();
    }
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
        continueBtn.GetComponent<Animator>().enabled = false;
        continueBtn.interactable = false;
        var text = continueBtn.GetComponentInChildren<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);
    }
    public void OnClickToggle(bool state)
    {
        if (continueBtn.interactable) return;
        continueBtn.interactable = true;
        continueBtn.GetComponent<Animator>().enabled = true;
        var text = continueBtn.GetComponentInChildren<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    }
    public void OnClickContinue()
    {
        bool useRemoteData = toogles[1].isOn;
        DatabaseController.Instance.ProgressAfterLogin(useRemoteData);
        //PlayerPrefs.SetInt(DatabaseController.LOGIN_TYPE, 1);
        StartCoroutine(DelayClose());
    }
    public void OnClickClose()
    {
        PlayerPrefs.SetInt(DatabaseController.LOGIN_TYPE, 2);
        StartCoroutine(DelayClose());
    }
    private IEnumerator DelayClose()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
