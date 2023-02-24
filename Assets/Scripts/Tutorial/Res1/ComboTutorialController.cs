using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ComboTutorialController : MonoBehaviour
{
    [SerializeField] private GameObject characterTutorial, coffeTutorial, breadTutorial, jamTutorial, comboTutorial;
    [SerializeField] private ServePlateController breadPlate, jamPlate, TeaPlate;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        characterTutorial.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickCharacterTut()
    {
        Destroy(characterTutorial);
        Time.timeScale = 1;
        StartCoroutine(DelayActiveCoffee());
    }
    private IEnumerator DelayActiveCoffee()
    {
        yield return new WaitForSeconds(5.5f);
        coffeTutorial.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickCoffeCup()
    {
        Destroy(coffeTutorial);
        Time.timeScale = 1;
        TeaPlate.ServeFood();
        StartCoroutine(DelayActiveBreadTut());
    }
    private IEnumerator DelayActiveBreadTut()
    {
        yield return new WaitForSeconds(0.25f);
        breadTutorial.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnClickBread()
    {
        Destroy(breadTutorial);
        Time.timeScale = 1;
        breadPlate.ServeFood();
        StartCoroutine(DelayActiveBreadJam());
    }
    private IEnumerator DelayActiveBreadJam()
    {
        yield return new WaitForSeconds(0.25f);
        Time.timeScale = 0;
        jamTutorial.SetActive(true);
    }
    public void OnClickBreadJam()
    {
        Time.timeScale = 1;
        jamPlate.ServeFood();
        Destroy(jamTutorial);
        StartCoroutine(DelayActiveComboTut());
    }
    private IEnumerator DelayActiveComboTut()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        comboTutorial.SetActive(true);
    }
    public void OnClickComboTut()
    {
        Time.timeScale = 1;
        Destroy(comboTutorial);
        Destroy(gameObject);
        FindObjectOfType<GameController>().ManualStartGameLoop();
    }
}
