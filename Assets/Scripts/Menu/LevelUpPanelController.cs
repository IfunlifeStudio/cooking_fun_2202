using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelUpPanelController : MonoBehaviour
{
    //[SerializeField] private AudioClip popUpClip;
    //[SerializeField] private TextMeshProUGUI levelText, rewardText;
    //[SerializeField] private GameObject gemProp, tapEffect;
    //[SerializeField] private Animator anim;
    //[SerializeField] private RateUsController rateUsPrefab;
    //private int rewards;
    //public void Spawn(int level, int rewards, Transform parent)
    //{
    //    GameObject go = Instantiate(gameObject, parent);
    //    go.GetComponent<LevelUpPanelController>().Init(level, rewards);
    //}
    //public void Init(int level, int _rewards)
    //{
    //    AudioController.Instance.PlaySfx(popUpClip);
    //    levelText.text = level.ToString();
    //    rewards = _rewards;
    //    rewardText.text = rewards.ToString();
    //    anim.Play("Appear");
    //}
    //public void OnClickClaim()
    //{
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelUp);
    //    FindObjectOfType<MainMenuController>().IncreaseGem(rewardText.transform.position, rewards);
    //    DataController.Instance.Ruby += rewards;
    //    DataController.Instance.SaveData();
    //    GameObject go = Instantiate(tapEffect, rewardText.transform.position, Quaternion.identity);
    //    Destroy(go, 1);
    //    //StartCoroutine(DelayDeactive());
    //}
    //private IEnumerator DelayDeactive()
    //{
    //    //anim.Play("Disappear");
    //    //yield return new WaitForSeconds(1f);
    //    //if (DataController.Instance.Level == 3)
    //    //    rateUsPrefab.Spawn(transform.parent);
    //    //else
    //    //    FindObjectOfType<MainMenuController>().DisplayMenuPanel();
    //    //Destroy(gameObject);
    //}
}
