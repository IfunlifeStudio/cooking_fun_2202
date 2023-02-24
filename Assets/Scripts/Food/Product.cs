using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
public class Product : MonoBehaviour
{
    public FoodBaseScriptableObject foodBase;
    public int FoodId
    {
        get { return foodBase.foodId; }
    }
    public int Type
    {
        get { return foodBase.ingredientIds.Length; }
    }
    [HideInInspector] public Vector3 originScale;
    public Product Spawn(Vector3 initPos, Transform parent)
    {
        GameObject go = Instantiate(gameObject, initPos, Quaternion.identity, parent);
        go.GetComponent<Product>().originScale = transform.localScale;
        return go.GetComponent<Product>();
    }
    public void Init(Vector3 initPos, Transform parent)
    {
        transform.SetParent(parent);
        transform.position = initPos;
        transform.localScale = originScale;
        gameObject.SetActive(true);
    }
    public void Dispose()
    {
        transform.localScale = originScale;
        transform.SetParent(null);
        gameObject.SetActive(false);
        ProductFactory.Instance.DisposeProduct(this);
    }
    public bool IsProductUnlock(bool isEvent = false)
    {
        bool result = true;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        if (isEvent)
        {
            foreach (int ingredientId in foodBase.ingredientIds)
                if (!EventDataController.Instance.IsIngredientUnlocked(ingredientId, currentLevel))
                {
                    result = false;
                    break;
                }
            return result;
        }
        else
        {
            foreach (int ingredientId in foodBase.ingredientIds)
                if (!DataController.Instance.IsIngredientUnlocked(ingredientId, currentLevel))
                {
                    result = false;
                    break;
                }
            return result;
        }
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        System.Action<ITween<Vector3>> updateProductPos = (t) =>
                    {
                        transform.position = t.CurrentValue;
                    };
        TweenFactory.Tween("Product" + parent.name + Time.time, transform.position, parent.position, (parent.position - transform.position).magnitude / 5, TweenScaleFunctions.SineEaseIn, updateProductPos);
    }
}
