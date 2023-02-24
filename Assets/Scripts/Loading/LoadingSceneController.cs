using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] Slider slider;
    public void UpdateLoadingBar(float fillPercent)
    {
        slider.value = fillPercent;
    }

}
