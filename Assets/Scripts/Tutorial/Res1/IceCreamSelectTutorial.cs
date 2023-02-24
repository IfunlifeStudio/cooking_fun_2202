using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamSelectTutorial : MonoBehaviour
{
    [SerializeField] private GameObject characterTutPanel, iceCreamTutPanel, AutoTutPanel;
    [SerializeField] private IceCreamItem iceCreamItem;
    [SerializeField] private CustomerController customer;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
        characterTutPanel.SetActive(true);
    }
    public void OnClickCharacterTut()
    {
        Time.timeScale = 1;
        Destroy(characterTutPanel);
        StartCoroutine(DelayEnableIceCreamTut());
    }
    IEnumerator DelayEnableIceCreamTut()
    {
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;
        iceCreamTutPanel.SetActive(true);
    }
    public void OnClickUseIceCream()
    {
        Time.timeScale = 1;
        Destroy(iceCreamTutPanel);
        iceCreamItem.isTut = false;
        iceCreamItem.CanUseIceCreamToCustomer(customer);
        StartCoroutine(DelayEnableAutoTut());
    }
    IEnumerator DelayEnableAutoTut()
    {
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;
        AutoTutPanel.SetActive(true);
    }
    public void OnClickAutoTut()
    {
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(AutoTutPanel);
        Destroy(gameObject);
    }
}
