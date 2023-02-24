﻿using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class JuicePlateController : LeanSelectableBehaviour
{
        public int id = 100;
    public Food activeFood = null;
    [SerializeField] protected SpriteRenderer dish;
    [SerializeField] protected Sprite[] dishSprites;
    [SerializeField] protected AudioClip addIngredientSfx, foodServeSfx;
    [SerializeField] protected GameObject autoServeEffect;
    protected BoxCollider2D boxCollider2D;
    private void Start()
    {
        if(boxCollider2D!=null)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = false;
        }
        
        int plateLevel = DataController.Instance.GetMachineLevel(id);
        dish.sprite = dishSprites[plateLevel - 1];
    }
    protected virtual void Update()
    {
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe) && activeFood != null)
        {
            if (CustomerFactory.Instance.CheckFood(activeFood.FoodId))
            {
                CustomerFactory.Instance.ServeFood(activeFood);
                GameObject instantEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                Destroy(instantEffect, 2);
                activeFood.DisposeFood();
                AudioController.Instance.PlaySfx(foodServeSfx);
                activeFood = null;
            }
        }
    }
    public virtual bool StoreCookedFood(int id)
    {
        if (activeFood == null)
        {
            if(boxCollider2D!=null)
                boxCollider2D.enabled = true;
            activeFood = FoodFactory.Instance.SpawnFoodById(id, transform);
            activeFood.GetComponent<FoodAnimatorController>().Play("FoodAppear");
            AudioController.Instance.PlaySfx(addIngredientSfx);
            return true;
        }
        return false;
    }
    public bool AddIngredient(int id)
    {
        bool result = false;
        if (activeFood != null)
        {
            Food tmp = activeFood.MergeFood(id);
            if (tmp != null)
            {
                activeFood = tmp;
                tmp.GetComponent<FoodAnimatorController>().Play("FoodAppear");
                AudioController.Instance.PlaySfx(addIngredientSfx);
                result = true;
            }
        }
        return result;
    }
    protected override void OnSelect(LeanFinger finger)
    {
        if (activeFood != null)
        {
            bool foodHasDispose = activeFood.OnClick();
            if (foodHasDispose)
            {
                if(boxCollider2D!=null)       
                    boxCollider2D.enabled = false;
                AudioController.Instance.PlaySfx(foodServeSfx);
                activeFood = null;
            }
        }
    }
    public virtual void ServeFood()
    {
        if (activeFood != null)
        {
            bool foodHasDispose = activeFood.OnClick();
            if (foodHasDispose)
            {
                AudioController.Instance.PlaySfx(foodServeSfx);
                 if(boxCollider2D!=null)       
                    boxCollider2D.enabled = false;
                activeFood = null;
            }
        }
    }
}
