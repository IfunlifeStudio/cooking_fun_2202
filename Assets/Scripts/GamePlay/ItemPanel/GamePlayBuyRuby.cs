using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GamePlayBuyRuby : UIView
{
    [SerializeField] Animator animator;
    //[SerializeField] GameObject[] rubyPanelPrefabs;
    //[SerializeField] GameObject  rubyFullPanelPrefab;
    [SerializeField] GameObject[] firstPacks, secondPacks;
    GameObject activeRubyPanel;
    [SerializeField] Transform rubyPanelContainer;
    void Start()
    {
        PlayerPrefs.SetString("shop_loaction", "shop_mini_ingame_lose_level");
        int playerClassLevel = PlayerClassifyController.Instance.GetPlayerClassifyLevel();
        for (int i = 0; i < firstPacks.Length; i++)
        {
            firstPacks[i].SetActive(false);
            secondPacks[i].SetActive(false);
        }
        firstPacks[playerClassLevel].SetActive(true);
        secondPacks[playerClassLevel].SetActive(true);
        UIController.Instance.PushUitoStack(this);

    }
    public override void OnHide()
    {
        UIController.Instance.PopUiOutStack();
        StartCoroutine(DelayOnHide());
    }
    IEnumerator DelayOnHide()
    {
        animator.Play("Disappear");
        yield return new WaitForSecondsRealtime(0.25f);
        Destroy(gameObject);
    }
}
