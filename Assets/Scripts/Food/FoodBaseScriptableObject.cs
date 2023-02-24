using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FoodBase", menuName = "ScriptableObjects/FoodBaseScriptableObject", order = 1)]
public class FoodBaseScriptableObject : ScriptableObject
{
    public int foodId;
    public int[] ingredientIds;
}
