using UnityEngine;
using System.Collections.Generic;
public class FoodFactory : MonoBehaviour
{
    private static FoodFactory instance = null;
    public static FoodFactory Instance
    {
        get { return instance; }
    }
    private int[] foodIds;
    public Food[] foodPrefabs;
    private List<Food> foodsPool;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            foodsPool = new List<Food>();
            List<int> tmp = new List<int>();
            foreach (var food in foodPrefabs)
                if (!tmp.Contains(food.FoodId))
                    tmp.Add(food.FoodId);
            foodIds = tmp.ToArray();
        }
        else
            Destroy(gameObject);
    }
    public Food SpawnFoodById(int id, Transform parent)
    {
        Food result = null;
        foreach (var food in foodsPool)//find food in pool
            if (food.FoodId == id)
            {
                result = food;
                break;
            }
        if (result != null)
        {
            foodsPool.Remove(result);
            result.transform.SetParent(parent);
            result.transform.localPosition = Vector3.zero;
            result.transform.localScale = Vector3.one;
            result.gameObject.SetActive(true);
            return result;
        }
        foreach (var food in foodPrefabs)//spawn new food
            if (food.FoodId == id)
                result = food;
        if (result != null) return result.Spawn(parent);
        return null;
    }
    public void DisposeFood(Food food)
    {
        foodsPool.Add(food);
    }
    public int GetMergeFoodId(int[] ids)//-1 mean cant find a valid food
    {
        int result = 0;
        foreach (int foodId in ids)
            result += foodId;
        foreach (var food in foodPrefabs)
            if (food.FoodId == result)
            {
                List<int> ingredients = new List<int>(food.IngredientIds);
                foreach (int id in ids)
                    if (!ingredients.Contains(id))
                        return -1;
                return food.FoodId;
            }
        return -1;
    }
}
