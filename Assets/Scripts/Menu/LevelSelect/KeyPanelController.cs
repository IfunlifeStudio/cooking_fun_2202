using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class KeyPanelController : UIView
{
     [SerializeField] private AudioClip popUpClip;
    [SerializeField] private TextMeshProUGUI keyCount;
    LevelSelectorController levelSelector;
    // Start is called before the first frame update
    public void Init(LevelSelectorController _levelSelector, int totalKeys, int totalLevels)
    {
        levelSelector = _levelSelector;
        AudioController.Instance.PlaySfx(popUpClip);
        keyCount.text = totalKeys + "/" + totalLevels;
    }
    public void OnClickContinue()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayClosePanel());
    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayOnHide());
    }
    IEnumerator DelayOnHide()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");    
        int chapter = DataController.Instance.currentChapter;
        int focus = DataController.Instance.GetFocusKeyLevel(DataController.Instance.currentChapter);
        levelSelector.GetLevelBtn(chapter, focus).OnSelectLevel();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
    public void Spawn(LevelSelectorController _levelSelector,Transform parent, int totalKeys, int totalLevels)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<KeyPanelController>().Init(_levelSelector,totalKeys, totalLevels);
        FindObjectOfType<MainMenuController>()?.setIndexTab(5);
        UIController.Instance.PushUitoStack(go.GetComponent<KeyPanelController>());
    }
}
