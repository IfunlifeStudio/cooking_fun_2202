using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCreamTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject orderTut, iceCreamMachineTut, iceCreamTut;
    [SerializeField] private IceCreamMachine iceCreamMachine;
    [SerializeField] private ServePlateController servePlate;
    // Start is called before the first frame update
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
        StartCoroutine(DelayIceCreamMachineTut());
    }
    private IEnumerator DelayIceCreamMachineTut()
    {
        yield return new WaitForSeconds(0.1f);
        iceCreamMachineTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickIceCreamMachineTut()
    {
        Time.timeScale = 1;
        iceCreamMachine.FillIngredent();
        Destroy(iceCreamMachineTut);
        StartCoroutine(DelayIceCreamTut());
    }
    private IEnumerator DelayIceCreamTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(iceCreamMachine.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.25f);
        iceCreamTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickIceCreamTut()
    {
        servePlate.ServeFood();
        Time.timeScale = 1;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(iceCreamTut);
        Destroy(gameObject);
    }
}
