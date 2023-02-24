using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeHelperController : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().Play("Appear");
    }
    private IEnumerator DelayClosePanel()
    {
        GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(0.2f);
        //var levelSelectHelper = FindObjectOfType<LevelSelectHelper>();
        //if (levelSelectHelper.CanUpgradeMachine())
        //{
        //    if (FindObjectOfType<IngredientUpgradeController>() != null)
        //        FindObjectOfType<MachineUpgradeController>().ShowUpgradeHelper();
        //}
        //else
        //{
        //    if (FindObjectOfType<IngredientUpgradeController>() != null)
        //        FindObjectOfType<IngredientUpgradeController>().ShowUpgradeHelper();
        //}
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        Destroy(gameObject);
    }
    public void OnClickUpgrade()
    {
        var levelSelectHelper = FindObjectOfType<LevelSelectHelper>();
        if (levelSelectHelper.CanUpgradeMachine())
            FindObjectOfType<DetailPanelController>().OnClickMachineUpgrade();
        else
            FindObjectOfType<DetailPanelController>().OnClickIngredientUpgrade();
        StartCoroutine(DelayClosePanel());
    }
    public void OnClickContinue()
    {
        StartCoroutine(DelayClosePanel());
        FindObjectOfType<LevelSelectorController>().OnClickPlay();
    }
}
