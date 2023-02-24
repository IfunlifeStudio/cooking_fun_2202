using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsparagusMachineTutorialController : MonoBehaviour
{
    public GameObject[] tutorialPanels;
    public IngredientHolder ingredientHolder;
    public Machine machine;
    public Machine asparagusMachine;
    public ToppingIngredient ingredientTopping;
    public ServePlateController FoodservePlate;
    public ServePlateController asparagusServePlate;
    private float startTime;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        tutorialPanels[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(tutorialPanels[0]);
        StartCoroutine(DelayAsparagusMachine());
    }
    private IEnumerator DelayAsparagusMachine()
    {
        yield return new WaitForSeconds(0.1f);
        tutorialPanels[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnclickAsparagusMachine()
    {
        Time.timeScale = 1;
        startTime = Time.time;
        asparagusMachine.FillIngredent();
        Destroy(tutorialPanels[1]);
        StartCoroutine(DelayHolderTut());
    }
    private IEnumerator DelayHolderTut()
    {
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[2].SetActive(true);
        Time.timeScale = 0;
    }
    public virtual void OnClickHolderTut()
    {
        Time.timeScale = 1;
        ingredientHolder.AddIngredient();
        Destroy(tutorialPanels[2]);
        StartCoroutine(DelayChickenMachineTut());
    }
    private IEnumerator DelayChickenMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(machine.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.5f);
        tutorialPanels[3].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickChickenMachineTut()
    {
        Time.timeScale = 1;
        machine.ServeFood();
        Destroy(tutorialPanels[3]);
        StartCoroutine(DelayIngredientTut());
    }
    private IEnumerator DelayIngredientTut()
    {
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[4].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickIngredientTut()
    {
        ingredientTopping.AddIngredient();
        Destroy(tutorialPanels[4]);
        Time.timeScale = 1;
        StartCoroutine(DelayAsparagusTut());
    }
    private IEnumerator DelayAsparagusTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(asparagusMachine.id);
        float tempTime = Mathf.Abs(Time.time - (startTime + workTime));
        yield return new WaitForSeconds(0.1f);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.1f);
        else
            yield return new WaitForSeconds(tempTime + 0.2f);
        tutorialPanels[5].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickAsparagusFood()
    {
        Time.timeScale = 1;
        asparagusServePlate.ServeFood();
        Destroy(tutorialPanels[5]);
        StartCoroutine(DelayFoodTut());
    }
    private IEnumerator DelayFoodTut()
    {
        yield return new WaitForSeconds(0.1f);
        tutorialPanels[6].SetActive(true);
        Time.timeScale = 0;
    }
    public virtual void OnClickFoodTut()
    {
        FoodservePlate.ServeFood();
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(tutorialPanels[6]);
        Destroy(gameObject);
    }
}
