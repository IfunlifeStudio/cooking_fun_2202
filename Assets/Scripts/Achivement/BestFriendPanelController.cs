using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestFriendPanelController : UIView
{
    [SerializeField] private Toggle[] toggles;
    private bool isScrolled;
    private int targetScrollValue;
    [SerializeField] RectTransform ContentRect;
    private int index = -1;
    Action onCloseCallback;
    [SerializeField] GameObject bfHelpPanel;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UIController.Instance.PushUitoStack(this);
        GetComponent<Animator>().Play("Appear");
        yield return new WaitForSeconds(0.2f);
        string certi_name = DataController.Instance.GetGameData().profileDatas.CertiTile;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (certi_name == toggles[i].name)
            {
                toggles[i].isOn = true;
                index = i;
                break;
            }
        }
        isScrolled = true;
        yield return new WaitForSeconds(0.2f);
        PlayerPrefs.SetInt("has_certificate", 1);
    }
    public void Init(Action onCloseCallback)
    {
        this.onCloseCallback = onCloseCallback;
    }
    // Update is called once per frame
    void Update()
    {
        if (isScrolled)
        {
            targetScrollValue = (index > 6) ? 168 : 0;
            float deltaPos = targetScrollValue - ContentRect.anchoredPosition.y;
            float sign = deltaPos / Mathf.Abs(deltaPos);
            float baseSpeed = Mathf.Max(600f, 2 * deltaPos * sign);
            if (Mathf.Abs(deltaPos) > 20)
                ContentRect.anchoredPosition += new Vector2(0, sign * Time.deltaTime * baseSpeed);
            else
                isScrolled = false;
        }
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        if (onCloseCallback != null)
            onCloseCallback.Invoke();
        GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.3f);
    }
    public void OnclickHelpPanel()
    {
        Instantiate(bfHelpPanel, transform);
    }
    public void OnClickToggle(bool state)
    {
    }
}
