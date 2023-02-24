using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerClassifyData
{
    public float moneyThreshold = 5.99f;
    public int[] keyThresholds = { 15, 35 };
    public AdsSettings[] adsSettingsCollection;
    public AdsFullCondition[] adsFullConditions;
}
