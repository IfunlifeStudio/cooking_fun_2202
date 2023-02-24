using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ReadyCookingTutorial : MonoBehaviour
{
    [SerializeField] private GameObject readyCookingTutorial;
    float instructionTime = 0;
    bool isShow = false;
    GameController gameController;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        instructionTime = Time.time;
    }
    private void Update()
    {
        if(Time.time - instructionTime >= 8)
        {
            CanShowTut();
        }
        if (Input.GetMouseButtonDown(0))
            instructionTime = Time.time;
    }
    private bool CanShowTut()
    {
        if (!isShow && !gameController.isEndGame)
        {
            isShow = true;
            gameController.IsShowAds = true;
            readyCookingTutorial.SetActive(true);
            return true;
        }
        else
            return false;
    }
    public void OnClickReadyTut()
    {
        gameController.IsShowAds = false;
        Destroy(gameObject);
    }
}
