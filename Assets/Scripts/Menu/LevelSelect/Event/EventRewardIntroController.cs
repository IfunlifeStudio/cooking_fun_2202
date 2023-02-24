using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.Tween;
public class EventRewardIntroController : MonoBehaviour
{
    public int eventId = 51;
    [SerializeField] private Transform[] rewardElements;
    [SerializeField] private Sprite coinIcon, rubyIcon, energyIcon, itemIcon;
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private TextMeshProUGUI countDownText;
    private int goldAmount = 0, rubyAmount = 0, unlimitedEnergyTime = 0, itemAmount = 0;
    public void Spawn(int _goldAmount, int _rubyAmount, int _unlimitEnergyTime, int _itemAmount, Transform canvasOverlay)
    {
        GameObject go = Instantiate(gameObject, canvasOverlay);
        go.GetComponent<EventRewardIntroController>().Init(_goldAmount, _rubyAmount, _unlimitEnergyTime, _itemAmount);
    }
    public void Init(int _goldAmount, int _rubyAmount, int _unlimitEnergyTime, int _itemAmount)//maximun display 3 rewards
    {
        AudioController.Instance.PlaySfx(popUpClip);
        goldAmount = 0;
        rubyAmount = 0;
        unlimitedEnergyTime = 0;
        itemAmount = 0;
        foreach (var go in rewardElements)
            go.gameObject.SetActive(false);
        int rewardCount = 0;
        if (_goldAmount > 0)
        {
            goldAmount = _goldAmount;
            rewardCount++;
        }
        if (_rubyAmount > 0)
        {
            rubyAmount = _rubyAmount;
            rewardCount++;
        }
        if (_unlimitEnergyTime > 0)
        {
            unlimitedEnergyTime = _unlimitEnergyTime;
            rewardCount++;
        }
        if (_itemAmount > 0)
        {
            itemAmount = _itemAmount;
            rewardCount++;
        }
        var _rewardElements = GetRewardElements(rewardCount);
        int index = 0;
        if (_goldAmount > 0)
        {
            _rewardElements[index].gameObject.SetActive(true);
            _rewardElements[index].GetChild(0).GetComponent<Image>().sprite = coinIcon;
            _rewardElements[index].GetComponentInChildren<TextMeshProUGUI>().text = goldAmount.ToString();
            index++;
        }
        if (_rubyAmount > 0)
        {
            _rewardElements[index].gameObject.SetActive(true);
            _rewardElements[index].GetChild(0).GetComponent<Image>().sprite = rubyIcon;
            _rewardElements[index].GetComponentInChildren<TextMeshProUGUI>().text = rubyAmount.ToString();
            index++;
        }
        if (_unlimitEnergyTime > 0)
        {
            _rewardElements[index].gameObject.SetActive(true);
            _rewardElements[index].GetChild(0).GetComponent<Image>().sprite = energyIcon;
            _rewardElements[index].GetComponentInChildren<TextMeshProUGUI>().text = unlimitedEnergyTime + " mins";
            index++;
        }
        if (itemAmount > 0)
        {
            _rewardElements[index].gameObject.SetActive(true);
            _rewardElements[index].GetChild(0).GetComponent<Image>().sprite = itemIcon;
            _rewardElements[index].GetComponentInChildren<TextMeshProUGUI>().text = itemAmount.ToString();
            index++;
        }
        float eventTimeStamp = 0;
        if (eventId == 51)
            eventTimeStamp = PlayerPrefs.GetFloat(EventDataController.EVENT_TIME_STAMP, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        else if (eventId == 52)
            eventTimeStamp = PlayerPrefs.GetFloat(EventDataController.EVENT_TIME_STAMP_2, (float)DataController.ConvertToUnixTime(System.DateTime.UtcNow));
        int deltaTime = (int)(3f * 24 * 60 * 60 - DataController.ConvertToUnixTime(System.DateTime.UtcNow) + eventTimeStamp);
        countDownText.text = deltaTime / (60 * 60) + "h " + (deltaTime / 60) % 60 + "m";
        gameObject.GetComponent<Animator>().Play("Appear");
    }
    public Transform[] GetRewardElements(int totalCard)
    {
        List<Transform> _rewardElements = new List<Transform>();
        if (totalCard == 1)
            _rewardElements.Add(rewardElements[0]);
        else
        if (totalCard == 2)
        {
            _rewardElements.Add(rewardElements[1]);
            _rewardElements.Add(rewardElements[2]);
        }
        else
        {
            _rewardElements.Add(rewardElements[3]);
            _rewardElements.Add(rewardElements[4]);
            _rewardElements.Add(rewardElements[5]);
        }
        return _rewardElements.ToArray();
    }
    public void OnClickPlay()
    {
        StartCoroutine(DelayShowEventLevelSelectPanel());
    }
    private IEnumerator DelayShowEventLevelSelectPanel()
    {
        gameObject.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        FindObjectOfType<EventLevelSelectorController>().Init(eventId);
        Destroy(gameObject);

    }
    public void OnClickClose()
    {
        gameObject.GetComponent<Animator>().Play("Disappear");
        Destroy(gameObject, 0.2f);
    }
}
