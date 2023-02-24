using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
public class ButtonScaler : MonoBehaviour
{
    Vector3 originScale;
    private void Start()
    {
        originScale = transform.localScale;
    }
    public void OnClick()
    {
        Vector3 targetScale = transform.localScale * 0.85f;
        System.Action<ITween<Vector3>> updateBtnScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        TweenFactory.Tween("BtnScale" + Random.Range(0, 100f) + Time.time, transform.localScale, targetScale, 0.03f, TweenScaleFunctions.QuinticEaseOut, updateBtnScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.06f, TweenScaleFunctions.QuinticEaseIn, updateBtnScale));
    }
}
