using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeMachineTutorial : MonoBehaviour
{
    [SerializeField] private GameObject orderTutPanel, MachineTutPanel, FoodTutPanel;
    [SerializeField] private Machine Machine;
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
        StartCoroutine(DelayMachineTut());
    }
    private IEnumerator DelayMachineTut()
    {
        yield return new WaitForSeconds(0.1f);
        MachineTutPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMachineTut()
    {
        Time.timeScale = 1;
        Machine.FillIngredent();
        Destroy(MachineTutPanel);
        StartCoroutine(DelayFoodTut());
    }
    private IEnumerator DelayFoodTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(Machine.id);
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
