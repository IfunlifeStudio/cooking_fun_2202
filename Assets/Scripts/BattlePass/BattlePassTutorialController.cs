using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject treeTut, decorTut, sockTut, playTut;
    public void OnClickTree()
    {
        treeTut.SetActive(false);
        decorTut.SetActive(true);
    }
    public void OnClickDecor()
    {
        decorTut.SetActive(false);
        sockTut.SetActive(true);
    }
    public void OnClickSock()
    {
        sockTut.SetActive(false);
        playTut.SetActive(true);
    }
    public void OnClickPlay()
    {
        Destroy(gameObject);
    }
}
