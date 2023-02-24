using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickAudio : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip btnClickAudio;
    public bool isNotScale;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isNotScale)
            return;
        else
            transform.localScale = Vector3.one * 0.9f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isNotScale)
            return;

        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btnClickAudio != null)
            AudioController.Instance.PlaySfx(btnClickAudio);
    }
    public void OnClick()
    {

    }
}
