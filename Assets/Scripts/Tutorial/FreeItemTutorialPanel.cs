using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class FreeItemTutorialPanel : MonoBehaviour
{
    private int itemId;
    [SerializeField] private AudioClip popUpClip;
    private UnityAction onClickCallback;
    public void Spawn(int itemId, Transform parent, UnityAction _clickCallBack)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<FreeItemTutorialPanel>().Init(itemId, _clickCallBack);
    }
    public void Init(int itemId, UnityAction clickCallBack)
    {
        this.itemId = itemId;
        this.onClickCallback = clickCallBack;
        AudioController.Instance.PlaySfx(popUpClip);
    }
    public void OnClick()
    {
        onClickCallback?.Invoke();
        if (itemId != 0)
            DataController.Instance.ClaimFree(itemId);
        Destroy(gameObject);
    }
}
