using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauChickenTutController : BaseMachineTutController
{
    [SerializeField] private GameObject[] tutorialPanels_2;
    [SerializeField] private HauChickenMachineController machineHau;
    public IngredientHauChicken ingredientHolderHau;
    
    [SerializeField] private CustomerController customerController;
    public override void OnFinishTut()
    {
        Time.timeScale = 1;
        customerController.gameObject.SetActive(true);
        StartCoroutine(DelayStartTut_2());
    }
    public void OnClickHolderTut_2()
    {
        
        Time.timeScale = 1;
        ingredientHolderHau.AddIngredient();               
        Destroy(tutorialPanels_2[1]);
        StartCoroutine(DelayMachineTut());       
    }

    private IEnumerator DelayStartTut_2()
    {
        yield return new WaitForSeconds(5.5f);
        tutorialPanels_2[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut_2()
    {
        UIController.Instance.CanBack = false;
        Time.timeScale = 1;
        Destroy(tutorialPanels_2[0]);
        StartCoroutine(DelayChickenHolderTut_2());
    }
    private IEnumerator DelayChickenHolderTut_2()
    {
        yield return new WaitForSeconds(0.2f);
        tutorialPanels_2[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMachineTut_2()
    {
        Time.timeScale = 1;
        machineHau.ServeFood();
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(tutorialPanels_2[2]);
        Destroy(gameObject);
    }
    private IEnumerator DelayMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(machineHau.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.3f);
        tutorialPanels_2[2].SetActive(true);
        Time.timeScale = 0;
    }

}
