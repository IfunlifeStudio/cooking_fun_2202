using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBtnTutorialPanel : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject[] children;
    IEnumerator Start()
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        if (mainMenu != null)
            mainMenu.setIndexTab(5);
        yield return new WaitForSeconds(3f);//activate all children
        AudioController.Instance.PlaySfx(popUpClip);
        foreach (GameObject child in children)
            child.SetActive(true);
    }
    public void OnClick()
    {
        FindObjectOfType<LevelSelectorController>().OnClickPlay();
        UIController.Instance.CanBack = true;
        Destroy(gameObject);
    }
}
