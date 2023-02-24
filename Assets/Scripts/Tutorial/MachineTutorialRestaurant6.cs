using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTutorialRestaurant6 : MachineTutorialController
{
    [SerializeField] private GameObject[] tutorialPanels_2;
    [SerializeField] private Machine mousseMachine;
    [SerializeField] private ServePlateController mouseServePlate;
    [SerializeField] private CustomerController customerController;
    public override void OnClickFoodTut()
    {
        servePlate.ServeFood();
        Time.timeScale = 1;
        Destroy(tutorialPanels[4]);
        customerController.gameObject.SetActive(true);
        StartCoroutine(DelayStartTut_2());
    }
    public override void OnClickHolderTut()
    {
        ingredientHolder.GetComponent<BoxCollider2D>().enabled = false;
        base.OnClickHolderTut();
    }

    private IEnumerator DelayStartTut_2()
    {
        yield return new WaitForSeconds(4f);
        tutorialPanels_2[0].SetActive(true);
        Time.timeScale = 0;

    }
    public void OnClickOrderTut_2()
    {
        Time.timeScale = 1;
        Destroy(tutorialPanels_2[0]);
        StartCoroutine(DelayMachineTut_2());
    }
    private IEnumerator DelayMachineTut_2()
    {
        yield return new WaitForSeconds(0.2f);
        tutorialPanels_2[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMachineTut_2()
    {
        Time.timeScale = 1;
        mousseMachine.FillIngredent();
        Destroy(tutorialPanels_2[1]);
        StartCoroutine(DelayMachineTut());
    }
    private IEnumerator DelayMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(mousseMachine.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.3f);
        tutorialPanels_2[2].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickFoodTut_2()
    {
        mouseServePlate.ServeFood();
        Time.timeScale = 1;
        mousseMachine.GetComponent<BoxCollider2D>().enabled = true;
        ingredientHolder.GetComponent<BoxCollider2D>().enabled = true;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(tutorialPanels_2[2]);
        Destroy(gameObject);
    }
}
