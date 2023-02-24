using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FirstPlayTutorial : MonoBehaviour
{
    [SerializeField] private GameObject characterTut, orderTut, breadTut, breadMachineTut, jamTut, sandwichJamTut;
    [SerializeField] private IngredientHolder breadHolder;
    [SerializeField] private Machine breadMachine;
    [SerializeField] private ToppingIngredient jamTopping;
    [SerializeField] private ServePlateController servePlate;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        APIController.Instance.LogEventBeginTut();
        if (PlayerPrefs.GetInt("user_property_level", 0) == 0)
        {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("Level", "1_1_0");
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("Chapter", "0");
            PlayerPrefs.SetInt("user_property_level", 1);
        }
        characterTut.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickCharacterPanel()
    {
        Destroy(characterTut);
        Time.timeScale = 1;
        StartCoroutine(DelayOrderTut());
    }
    private IEnumerator DelayOrderTut()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
        orderTut.SetActive(true);
    }
    public void OnClickOrderTut()
    {
        Time.timeScale = 1;
        Destroy(orderTut);
        StartCoroutine(DelayBreadTut());
    }
    private IEnumerator DelayBreadTut()
    {
        yield return new WaitForSeconds(0.25f);
        Time.timeScale = 0;
        breadTut.SetActive(true);
    }
    public void OnClickBreadTut()
    {
        Time.timeScale = 1;
        Destroy(breadTut);
        breadHolder.AddIngredient();
        StartCoroutine(DelayBreadMachineTut());
    }
    private IEnumerator DelayBreadMachineTut()
    {
        float workTime = DataController.Instance.GetMachineWorkTime(breadMachine.id);
        yield return new WaitForSeconds(workTime);
        Time.timeScale = 0;
        breadMachineTut.SetActive(true);
    }
    public void OnClickBreadMachineTut()
    {
        Time.timeScale = 1;
        Destroy(breadMachineTut);
        breadMachine.ServeFood();
        StartCoroutine(DelayJamTut());
    }
    private IEnumerator DelayJamTut()
    {
        yield return new WaitForSeconds(0.25f);
        Time.timeScale = 0;
        jamTut.SetActive(true);
    }
    public void OnClickJamTut()
    {
        Time.timeScale = 1;
        Destroy(jamTut);
        jamTopping.AddIngredient();
        StartCoroutine(DelaySandwichJamTut());
    }
    private IEnumerator DelaySandwichJamTut()
    {
        yield return new WaitForSeconds(0.25f);
        Time.timeScale = 0;
        sandwichJamTut.SetActive(true);
    }
    public void OnClickSandwichJameTut()
    {
        Time.timeScale = 1;
        servePlate.ServeFood();
        FindObjectOfType<GameController>().ManualStartGameLoop();
        Destroy(sandwichJamTut);
        Destroy(gameObject);
    }
}
