using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance { get { return instance; } }
    private Stack<UIView> uiStack;
    public bool CanBack { get; set; }
    public bool IsShowSandwichBtn { get; set; }
    public NotifyPanelController notifyPanel;
    private NotifyPanelController activeNotify;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            uiStack = new Stack<UIView>();
            CanBack = true;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public void PushUitoStack(UIView uiView)
    {
        if (CanBack)
        {
            uiStack.Push(uiView);
        }
    }
    public void PopUiOutStack()
    {
        if (CanBack && uiStack.Count > 0)
        {
            uiStack.Pop();
        }
    }
    public void ClearStack()
    {
        uiStack.Clear();
        IsShowSandwichBtn = false;
    }
    public int StackLenght
    {
        get { return uiStack.Count; }
    }
    public Stack<UIView> UiStack
    {
        set { uiStack = value; }
        get { return uiStack; }
    }
    public UIView GetCurrentUi()
    {
        if (uiStack.Count > 0)
        {
            return uiStack.Peek();
        }
        return null;
    }
    public void ShowNotiFy(NotifyType notifyType, Action callback = null)
    {
        if (activeNotify == null)
            activeNotify = notifyPanel.Spawn(notifyType, callback);
    }
}
