using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YakitoriTutorialController : MonoBehaviour
{
    [SerializeField] private int machineId;
    [SerializeField] private GameObject orderTutPanel, IngredientHolderTutPanel, FoodTutPanel;
    [SerializeField] private IngredientHolder ingredientHolder;
    [SerializeField] private ServePlateController servePlate;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        orderTutPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(orderTutPanel);
        StartCoroutine(DelayIngredientHodlerTut());
    }
    private IEnumerator DelayIngredientHodlerTut()
    {
        yield return new WaitForSeconds(0.1f);
        IngredientHolderTutPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickHolderTut()
    {
        Time.timeScale = 1;
        ingredientHolder.AddIngredient();
        Destroy(IngredientHolderTutPanel);
        StartCoroutine(DelayFoodTut());
    }
    private IEnumerator DelayFoodTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(machineId);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.25f);
        FoodTutPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickFoodTut()
    {
        servePlate.ServeFood();
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(FoodTutPanel);
        Destroy(gameObject);
    }
}
