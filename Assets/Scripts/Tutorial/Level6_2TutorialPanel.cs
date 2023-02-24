using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6_2TutorialPanel : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private GameObject[] children;
    IEnumerator Start()
    {
        FindObjectOfType<MainMenuController>().setIndexTab(5);
        yield return new WaitForSeconds(0.5f);//activate all children
        AudioController.Instance.PlaySfx(popUpClip);
        children[0].SetActive(true);
    }
    public void OnClickLevelBtnTut()
    {
        children[0].SetActive(false);
        children[1].SetActive(true);
        children[2].SetActive(true);
    }
    public void OnClickPlay()
    {
        FindObjectOfType<LevelSelectorController>().OnClickPlay();
        APIController.Instance.LogEventCompleteTut();
        Destroy(gameObject);
    }
}
