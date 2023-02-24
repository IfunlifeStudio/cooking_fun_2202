using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FoodTutorialPanel : MonoBehaviour
{
    [SerializeField] protected AudioClip popUpClip;
    [SerializeField] protected Transform mask;
    public abstract void Spawn(Vector3 maskPos);
}
