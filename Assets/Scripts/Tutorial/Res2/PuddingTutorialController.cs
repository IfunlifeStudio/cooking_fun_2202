using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject puddingTutorialPanel;
    [SerializeField] private PuddingItemController puddingItem;
    [SerializeField] private IceCreamItem iceCreamItem;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
        puddingTutorialPanel.SetActive(true);
    }
    public void OnClickUsePudding()
    {
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(puddingTutorialPanel);
        puddingItem.OnClickPudding();
        iceCreamItem.isTut = false;
        Destroy(gameObject);
    }
}
