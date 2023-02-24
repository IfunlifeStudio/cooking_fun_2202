using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class Food : MonoBehaviour
{
    public FoodBaseScriptableObject foodBase;
    public int FoodId
    {
        get { return foodBase.foodId; }
    }
    public int[] IngredientIds
    {
        get { return foodBase.ingredientIds; }
    }
    protected float clickTimeStamp;
    public abstract Food MergeFood(int id);
    public abstract bool OnClick();
    public Food Spawn(Transform parent)
    {
        GameObject go = Instantiate(gameObject, parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        return go.GetComponent<Food>();
    }
    public void DisposeFood()
    {
        gameObject.SetActive(false);
        FoodFactory.Instance.DisposeFood(this);
    }
    public int GetFoodValue(bool isEvent)
    {
        if (isEvent) return EventDataController.Instance.GetFoodValue(this);
        return DataController.Instance.GetFoodValue(this);
    }
}
