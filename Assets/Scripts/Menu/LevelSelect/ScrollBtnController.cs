using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DigitalRuby.Tween;

public class ScrollBtnController : MonoBehaviour
{
    public float value =1f;
    public ScrollRect scrollRect;
    public void OnLeftScroll()
    {
        System.Action<ITween<float>> scrollUpdate = (t) =>
        {
            scrollRect.horizontalNormalizedPosition = t.CurrentValue;
        };
        TweenFactory.Tween("LeftScoll" + Time.time, scrollRect.horizontalNormalizedPosition, scrollRect.horizontalNormalizedPosition - value, 0.25f, TweenScaleFunctions.QuinticEaseOut, scrollUpdate);
    }
    public void OnRightScroll()
    {
        System.Action<ITween<float>> scrollUpdate = (t) =>
        {
            scrollRect.horizontalNormalizedPosition = t.CurrentValue;
        };
        TweenFactory.Tween("LeftScoll" + Time.time, scrollRect.horizontalNormalizedPosition, scrollRect.horizontalNormalizedPosition + value, 0.25f, TweenScaleFunctions.QuinticEaseOut, scrollUpdate);
    }
}
