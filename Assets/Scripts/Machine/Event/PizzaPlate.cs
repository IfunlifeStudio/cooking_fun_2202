using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
public class PizzaPlate : ServePlateController
{
    [SerializeField] private PizzaMachine pizzaMachine;
    private float clickTimeStamp = 0;
    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        int plateLevel = EventDataController.Instance.GetMachineLevel(id);
        dish.sprite = dishSprites[plateLevel - 1];
    }
    protected override void OnSelect(LeanFinger finger)
    {
        if (activeFood != null)
        {
            pizzaMachine.activeFoodId = activeFood.FoodId;
            bool foodHasDispose = pizzaMachine.FillIngredent();
            if (foodHasDispose)
            {
                AudioController.Instance.PlaySfx(foodServeSfx);
                this.activeFood.DisposeFood();
                activeFood = null;
            }
            else
            if (Time.time - clickTimeStamp < 0.25f)
            {
                FindObjectOfType<TrashBinController>().TossTrash(activeFood.FoodId);
                this.activeFood.DisposeFood();
                activeFood = null;
            }

        }
        clickTimeStamp = Time.time;
    }
}
