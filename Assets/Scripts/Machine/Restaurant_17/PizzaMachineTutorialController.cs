using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaMachineTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialPanels;
    public ToppingIngredientPizza ingredientTopping;
    public PizzaMachineController pizzaMachine;
    public PizzaPlateController pizzaPlate;
    public IngredientsPizzaHolder pizzaHolder;
    private IEnumerator Start()
    {
        UIController.Instance.CanBack = false;
        yield return new WaitForSeconds(1f);
        tutorialPanels[0].SetActive(true);
        
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(tutorialPanels[0]);
        StartCoroutine(DelayToppingTut());
    }
    private IEnumerator DelayPizzaHolderTut()
    {
        pizzaHolder.PizzaHolderTut();
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[2].SetActive(true);
        Time.timeScale = 0;
    }
    public virtual void OnClickHolderTut()
    {
        Time.timeScale = 1;
        pizzaPlate.PizzaTut();
      
        Destroy(tutorialPanels[2]);
        StartCoroutine(DelayMachineTut());
    }
     private IEnumerator DelayToppingTut()
    {
        yield return new WaitForSeconds(0.25f);
        tutorialPanels[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickToppingTut()
    {
        ingredientTopping.AddIngredient();
        Destroy(tutorialPanels[1]);
        Time.timeScale = 1;
        StartCoroutine(DelayPizzaHolderTut());
    }
    private IEnumerator DelayMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(1700);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.1f);
        yield return new WaitForSeconds(0.1f);
        tutorialPanels[3].SetActive(true);
        Time.timeScale = 0;
    }

    public virtual void OnClickMachineTut()
    {
        pizzaMachine.ServeFood();       
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(tutorialPanels[3]);
        Destroy(gameObject);
        
    }
}
