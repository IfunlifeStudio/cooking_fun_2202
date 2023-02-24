using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ToolPanelTutorial : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject[] children;
    private bool canClick = false;
    public void Spawn()
    {
        Instantiate(gameObject);
    }
    private void Start()
    {
        AudioController.Instance.PlaySfx(popUpClip);
        StartCoroutine(DelayActive());
    }
    private IEnumerator DelayActive()
    {
        FindObjectOfType<MainMenuController>().setIndexTab(5);
        foreach (GameObject go in children)
            go.SetActive(false);
        canClick = false;
        yield return new WaitForSeconds(0.75f);
        canClick = true;
        foreach (GameObject go in children)
            go.SetActive(true);
    }
    public void OnClick()
    {
        if (!canClick) return;
        UIController.Instance.CanBack = true;
        FindObjectOfType<DetailPanelController>().OnClickMachineUpgrade();
        Destroy(gameObject);
    }
}
