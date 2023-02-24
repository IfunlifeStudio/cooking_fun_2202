using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadMachineUpgradeTutorial : MonoBehaviour
{
    [SerializeField] private GameObject clickMachinePanel, upgradePanel;
    [SerializeField] private RectTransform mask1, hand1;
    [SerializeField] private Transform mask2, hand2;
    [SerializeField] private float refX = -280;
    [SerializeField] private PlateUpgradeTutorial plateUpgradeTut;
    [SerializeField] private AudioClip popUpClip;
    private Vector3 upgradeBtnPos;
    private bool canSpawnPlateTut = false;
    public void Spawn(Vector3 _upgradeBtnPos, Transform parent, bool _canSpawnPlateTut)
    {
        GameObject go = Instantiate(gameObject, parent);
        go.GetComponent<BreadMachineUpgradeTutorial>().Init(_upgradeBtnPos, _canSpawnPlateTut);
    }
    private void Init(Vector3 _upgradeBtnPos, bool _canSpawnPlateTut)
    {
        canSpawnPlateTut = _canSpawnPlateTut;
        upgradeBtnPos = _upgradeBtnPos;
        mask1.anchoredPosition3D = new Vector3(refX, 160, -5);
        hand1.anchoredPosition3D = new Vector3(refX, 160, -5);
        AudioController.Instance.PlaySfx(popUpClip);
        clickMachinePanel.SetActive(true);
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
        Destroy(gameObject);
        if (canSpawnPlateTut)
            plateUpgradeTut.Spawn(upgradeBtnPos, transform.parent);
    }
}
