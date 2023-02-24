using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlateUpgradeTutorial : MonoBehaviour
{
    [SerializeField] private GameObject clickMachinePanel, upgradePanel;
    [SerializeField] private float refX = -125;
    private Vector3 upgradeBtnPos;
    [SerializeField] private RectTransform mask1, hand1;
    [SerializeField] private Transform mask2, hand2;
    [SerializeField] private AudioClip popUpClip;
    public void Spawn(Vector3 _upgradeBtnPos, Transform parent)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<PlateUpgradeTutorial>().Init(_upgradeBtnPos);
    }
    private void Init(Vector3 _upgradeBtnPos)
    {
        upgradeBtnPos = _upgradeBtnPos;
        mask1.anchoredPosition3D = new Vector3(refX, 160, -5);
        hand1.anchoredPosition3D = new Vector3(refX, 160, -5);
        AudioController.Instance.PlaySfx(popUpClip);
        StartCoroutine(DelayActive());
    }
    private IEnumerator DelayActive()
    {
        clickMachinePanel.SetActive(false);
        yield return new WaitForSecondsRealtime(1);
        clickMachinePanel.SetActive(true);
        FindObjectOfType<MachineUpgradeController>().machinesBtn[1].OnSelectMachine();
    }
    public void OnClickMachine()
    {
        clickMachinePanel.SetActive(false);
        upgradePanel.SetActive(true);
        //mask2.position = upgradeBtnPos;
        //hand2.position = upgradeBtnPos;
    }
    public void OnClickUpgrade()
    {
        FindObjectOfType<MachineUpgradeController>().UpgradeMachine();
        UIController.Instance.CanBack = true;
        Destroy(gameObject);
    }
}
