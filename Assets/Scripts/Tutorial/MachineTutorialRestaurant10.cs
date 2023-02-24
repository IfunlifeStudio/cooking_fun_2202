using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTutorialRestaurant10 : MonoBehaviour
{
    [SerializeField] private GameObject[] grillPorkTutPanel,matchaTutPanel;
    [SerializeField] private Machine grillPorkMachine,matchaMachine,bunMachine;
    [SerializeField] private IngredientHolder GrillporkHolder;
    [SerializeField] private ServePlateController kebabServePlate,matchaServePlate;
    [SerializeField] private CustomerController customerController;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        grillPorkTutPanel[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(grillPorkTutPanel[0]);
        StartCoroutine(DelayBunTut());
    }
    private IEnumerator DelayBunTut()
    {
        yield return new WaitForSeconds(0.25f);
        grillPorkTutPanel[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickBunTut()
    {
        Time.timeScale = 1;
        bunMachine.ServeFood();
        Destroy(grillPorkTutPanel[1]);
        StartCoroutine(DelayGrillPorkHolderTut());
    }
    private IEnumerator DelayGrillPorkHolderTut()
    {
        yield return new WaitForSeconds(0.25f);
        grillPorkTutPanel[2].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickGrillPorkHolderTut()
    {
         GrillporkHolder.AddIngredient();
        Time.timeScale = 1;
        Destroy(grillPorkTutPanel[2]);
        StartCoroutine(DelayGrillPorkMachineTut());
    }
    private IEnumerator DelayGrillPorkMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(grillPorkMachine.id);
        yield return new WaitForSeconds(workTime + 0.1f);
        grillPorkTutPanel[3].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickGrillPorkMachineTut()
    {
        Time.timeScale = 1;
        grillPorkMachine.ServeFood();
        Destroy(grillPorkTutPanel[3]);
        StartCoroutine(DelayGrillPorkTut());
    }
    private IEnumerator DelayGrillPorkTut()
    {
        yield return new WaitForSeconds(0.25f);
        grillPorkTutPanel[4].SetActive(true);
        Time.timeScale = 0;
    }
    public  void OnGrillPorkFoodTut()
    {
        kebabServePlate.ServeFood();
        Time.timeScale = 1;
        Destroy(grillPorkTutPanel[4]);
        customerController.gameObject.SetActive(true);
        StartCoroutine(DelayStartTut_2());
    }
    private IEnumerator DelayStartTut_2()
    {
        yield return new WaitForSeconds(5f);
         matchaTutPanel[0].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickOrderMatchaTut()
    {
        Time.timeScale = 1;
        Destroy(matchaTutPanel[0]);
        StartCoroutine(DelayMatchaMachineTut());
    }
    private IEnumerator DelayMatchaMachineTut()
    {
        yield return new WaitForSeconds(0.2f);
        matchaTutPanel[1].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMachineTut()
    {
        Time.timeScale = 1;
        matchaMachine.FillIngredent();
        Destroy(matchaTutPanel[1]);
        StartCoroutine(DelayMatchaTut());
    }
    private IEnumerator DelayMatchaTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(matchaMachine.id);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Instant_Cook))
            yield return new WaitForSeconds(0.2f);
        else
            yield return new WaitForSeconds(workTime + 0.3f);
        matchaTutPanel[2].SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickMatchaFoodTut()
    {
        Time.timeScale = 1;
        matchaServePlate.ServeFood();
        //matchaMachine.GetComponent<BoxCollider2D>().enabled = true;
        //bunMachine.GetComponent<BoxCollider2D>().enabled = true;
        //GrillporkHolder.GetComponent<BoxCollider2D>().enabled = true;
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(matchaTutPanel[2]);
        Destroy(gameObject);
    }
}
