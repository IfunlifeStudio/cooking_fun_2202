using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActiveUserPanel : MonoBehaviour
{
    [SerializeField] int minActiveUser = 35000;
    [SerializeField] int maxActiveUser = 45000;
    [SerializeField] int maxRange = 500;
    [SerializeField] TextMeshProUGUI activeUserQuantity;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            StartCoroutine(DelayShowOnlineUser());
        else
            gameObject.SetActive(false);
    }
    IEnumerator DelayShowOnlineUser()
    {
        var delay = new WaitForSeconds(60);
        while (true)
        {
            activeUserQuantity.text = RadomActiveUser.ToString();
            yield return delay;
        }
    }
    public int RadomActiveUser
    {
        get
        {
            int rd_user = Random.Range(minActiveUser, maxActiveUser);
            int range = Random.Range(-maxRange, maxRange);
            if (rd_user + range < maxActiveUser) return rd_user;
            else return rd_user + range;
        }
    }
}
