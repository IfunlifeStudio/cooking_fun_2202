using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    [SerializeField] private Image handle;
    [SerializeField] private Image backgroundSlider;
    [SerializeField] private Sprite[] handleSprites;
    [SerializeField] private Sprite[] bgSliderSprites;
    [SerializeField] private GameObject[] statusTexts;
    public void SetSlideValue(int value)
    {
        GetComponent<Slider>().value = value;
    }
    public void OnValueChanged(float value)
    {
        if (value < 0.5f)
        {
            handle.sprite = handleSprites[0];
            backgroundSlider.sprite = bgSliderSprites[0];
            statusTexts[0].SetActive(true);
            statusTexts[1].SetActive(false);
        }
        else
        {
            handle.sprite = handleSprites[1];
            backgroundSlider.sprite = bgSliderSprites[1];
            statusTexts[0].SetActive(false);
            statusTexts[1].SetActive(true);
        }
    }
}
