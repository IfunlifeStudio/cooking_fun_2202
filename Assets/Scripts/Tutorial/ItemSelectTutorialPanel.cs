using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ItemSelectTutorialPanel : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject[] children;
    private UnityAction onClickCallback;
    public void Spawn(UnityAction _onClickCallback)
    {
        Transform camCanvas = FindObjectOfType<MainMenuController>().cameraCanvas;
        GameObject go = Instantiate(gameObject, camCanvas);
        go.GetComponent<ItemSelectTutorialPanel>().Init(_onClickCallback);
        UIController.Instance.CanBack = false;
        FindObjectOfType<MainMenuController>().setIndexTab(5);
    }
    private IEnumerator DelayActive()
    {
        foreach (var go in children)
            go.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        foreach (var go in children)
            go.SetActive(true);
    }
    public void Init(UnityAction _onClickCallback)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        this.onClickCallback = _onClickCallback;
        StartCoroutine(DelayActive());
    }
    public void OnClick()
    {
        onClickCallback.Invoke();
        UIController.Instance.CanBack = true;
        Destroy(gameObject);
    }
}
