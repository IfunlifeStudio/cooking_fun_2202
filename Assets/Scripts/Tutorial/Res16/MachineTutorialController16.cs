using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTutorialController16 : MonoBehaviour
{
    public GameObject[] tutorialPanels;
    public IngredientHolder ingredientHolder;
    public Machine machine;
    public ToppingIngredient ingredientTopping;
    public ServePlateController servePlate;
    private IEnumerator Start()
    {
        UIController.Instance.CanBack = false;
        yield return new WaitForSeconds(1.5f);
        tutorialPanels[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        UIController.Instance.CanBack = false;
        Time.timeScale = 1;
        Destroy(tutorialPanels[0]);
        StartCoroutine(DelayHolderTut());
    }
    private IEnumerator DelayHolderTut()
    {
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[1].SetActive(true);
        Time.timeScale = 0;
    }
    public virtual void OnClickHolderTut()
    {
        Time.timeScale = 1;
        ingredientHolder.AddIngredient();
        Destroy(tutorialPanels[1]);
        StartCoroutine(DelayMachineTut());
    }
    private IEnumerator DelayMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(machine.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.1f);
        tutorialPanels[2].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMachineTut()
    {
        Time.timeScale = 1;
        machine.ServeFood();
        Destroy(tutorialPanels[2]);
        if (ingredientTopping != null)
            StartCoroutine(DelayIngredientTut());
        else
            StartCoroutine(DelayFoodTut());
    }
    private IEnumerator DelayIngredientTut()
    {
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[3].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickIngredientTut()
    {
        ingredientTopping.AddIngredient();
        Destroy(tutorialPanels[3]);
        Time.timeScale = 1;
        StartCoroutine(DelayFoodTut());
    }
    private IEnumerator DelayFoodTut()
    {
        yield return new WaitForSeconds(0.1f);
        tutorialPanels[3].SetActive(true);
        Time.timeScale = 0;
    }
    public virtual void OnClickFoodTut()
    {
        servePlate.ServeFood();
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(tutorialPanels[3]);
        Destroy(gameObject);
    }
}
