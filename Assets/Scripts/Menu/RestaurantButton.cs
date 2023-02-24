using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RestaurantButton : MonoBehaviour
{
    public int resIndex = 1;
    //[SerializeField] ActiveUserPanel activeUserPanel;
    [SerializeField] private Image[] resElements;
    private MainMenuController menuController;
    private void Start()
    {
        menuController = FindObjectOfType<MainMenuController>();

    }

    public void OnClick()
    {
        menuController.OnSelectRestaurant(resIndex);
    }
    public void SetUpMaterial(Material mat)
    {
        foreach (Image element in resElements)
            element.material = mat;
    }
    //public void SetActiveUserPanelPosition(Vector3 position)
    //{
    //    if (Application.internetReachability != NetworkReachability.NotReachable)
    //        activeUserPanel.GetComponent<RectTransform>().anchoredPosition = position;
    //    else
    //        activeUserPanel.gameObject.SetActive(false);
    //}

}
