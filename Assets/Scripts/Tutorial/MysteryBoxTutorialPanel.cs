using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxTutorialPanel : MonoBehaviour
{
    [SerializeField] private AudioClip popUpClip;
    [SerializeField] private Transform mask, hand;
    [SerializeField] private GameObject[] children;
    public void Spawn(Vector3 spawnPos)
    {
        UIController.Instance.CanBack = false;
        GameObject go = Instantiate(gameObject);
        go.GetComponent<MysteryBoxTutorialPanel>().Init(spawnPos);
    }
    public void Init(Vector3 spawnPos)
    {
        AudioController.Instance.PlaySfx(popUpClip);
        mask.position = new Vector3(spawnPos.x, spawnPos.y, mask.position.z);
        hand.position = new Vector3(spawnPos.x + 51, spawnPos.y - 67, hand.position.z);
    }
    public void OnClick()
    {
        UIController.Instance.CanBack = true;
        FindObjectOfType<KeyBoxBtnController>().OnClickBox();
        Destroy(gameObject);
    }
}
