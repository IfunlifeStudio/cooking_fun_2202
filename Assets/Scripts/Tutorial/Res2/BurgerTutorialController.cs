using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject orderTut, bunTut, steakTut, steakmachineTut, burgerTut;
    [SerializeField] private BunMachine bunMachine;
    [SerializeField] private IngredientHolder steakHolder;
    [SerializeField] private Machine steakMachine;
    [SerializeField] private ServePlateController servePlate;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        orderTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(orderTut);
        StartCoroutine(DelayBunTut());
    }
    private IEnumerator DelayBunTut()
    {
        yield return new WaitForSeconds(0.25f);
        bunTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickBunTut()
    {
        Time.timeScale = 1;
        bunMachine.ServeFood();
        Destroy(bunTut);
        StartCoroutine(DelaySteakTut());
    }
    private IEnumerator DelaySteakTut()
    {
        yield return new WaitForSeconds(0.25f);
        steakTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickSteakTut()
    {
        steakHolder.AddIngredient();
        Time.timeScale = 1;
        Destroy(steakTut);
        StartCoroutine(DelaySteakMachineTut());
    }
    private IEnumerator DelaySteakMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(steakMachine.id);
        yield return new WaitForSeconds(workTime + 0.1f);
        steakmachineTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickSteakMachineTut()
    {
        Time.timeScale = 1;
        steakMachine.ServeFood();
        Destroy(steakmachineTut);
        StartCoroutine(DelayBurgerTut());
    }
    private IEnumerator DelayBurgerTut()
    {
        yield return new WaitForSeconds(0.25f);
        burgerTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickBurgerTut()
    {
        servePlate.ServeFood();
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(burgerTut);
        Destroy(gameObject);
    }
}
