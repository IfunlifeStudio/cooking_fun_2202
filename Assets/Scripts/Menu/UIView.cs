using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIView : MonoBehaviour,IBaseView
{
    public virtual void OnHide()
    {
    }

    public virtual void OnShow()
    {

    }

    public virtual void OnShow(Transform parent)
    {

    }
}
public interface IBaseView
{
    void OnHide();
    void OnShow();
    void OnShow(Transform parent);
}
