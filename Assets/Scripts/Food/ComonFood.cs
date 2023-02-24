using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ComonFood : Food//common food is alway on plate suck as "bread with topping"
{
    [SerializeField] private bool isToss;
    public override Food MergeFood(int id)
    {
        List<int> ingredients = new List<int>(IngredientIds);
        ingredients.Add(id);
        int mergedFoodId = FoodFactory.Instance.GetMergeFoodId(ingredients.ToArray());
        if (mergedFoodId != -1)
        {
            Food resultFood = FoodFactory.Instance.SpawnFoodById(mergedFoodId, transform.parent);
            DisposeFood();
            return resultFood;
        }
        return null;
    }
    public override bool OnClick()
    {
        bool result = false;
        if (CustomerFactory.Instance.CheckFood(FoodId))
        {
            CustomerFactory.Instance.ServeFood(this);
            DisposeFood();
            result = true;
        }
        else
        {
            if ((Time.time - clickTimeStamp < 0.5f) && !isToss)
            {
                FindObjectOfType<TrashBinController>().TossTrash(FoodId);
                DisposeFood();
                result = true;
            }
            else
                GetComponent<FoodAnimatorController>().Play("FoodAppear");
        }
        clickTimeStamp = Time.time;
        return result;
    }
}
