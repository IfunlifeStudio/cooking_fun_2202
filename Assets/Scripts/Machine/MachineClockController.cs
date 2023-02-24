using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
public class MachineClockController : MonoBehaviour
{
    private int currentActiveClock = -1;
    [SerializeField] private GameObject[] clocks;
    [SerializeField] private Image[] clockFillLayers;
    [SerializeField] private float fillThreshold = 0.75f;
    public MachineClockController Spawn(Vector3 spawnPos)
    {       
        Transform canvasWorld = GameObject.Find("CanvasWorld").transform;
        GameObject go = Instantiate(gameObject, spawnPos, Quaternion.identity,canvasWorld);
        MachineClockController machineClockController = go.GetComponent<MachineClockController>();
        return machineClockController;
    }
    public void SetFillAmount(float fillAmount, bool isCooking)
    {
        if (isCooking)
        {
            if (currentActiveClock != 0)
            {
                if (currentActiveClock > -1)
                    clocks[currentActiveClock].SetActive(false);
                currentActiveClock = 0;
                ShowClock(0);
            }
        }
        else
        {
            if (fillAmount > 0.75f)
            {
                if (currentActiveClock != 2)
                {
                    if (currentActiveClock > -1)
                        clocks[currentActiveClock].SetActive(false);
                    currentActiveClock = 2;
                    clocks[2].SetActive(true);
                }
            }
            else
            {
                if (currentActiveClock != 1)
                {
                    if (currentActiveClock > -1)
                        clocks[currentActiveClock].SetActive(false);
                    currentActiveClock = 1;
                    ShowClock(1);
                }
            }
        }
        clockFillLayers[currentActiveClock].fillAmount = fillAmount;
    }
    private void ShowClock(int index)
    {
        Transform clock = clocks[index].transform;
        clocks[index].SetActive(true);
        System.Action<ITween<Vector3>> updateClockScale = (t) =>
                            {
                                if (clock != null)
                                    clock.localScale = t.CurrentValue;
                            };
        TweenFactory.Tween("clock" + Random.Range(0f, 1000f) + Time.time, Vector3.zero, Vector3.one, 0.5f, TweenScaleFunctions.QuinticEaseOut, updateClockScale);
    }
    public void HideClock()
    {
        foreach (var clock in clocks)
            clock.SetActive(false);
        currentActiveClock = -1;
    }
    public void Dispose()
    {
        Destroy(gameObject);
    }
}

