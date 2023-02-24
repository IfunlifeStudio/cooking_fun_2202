using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YakitoriServePlateController : ServePlateController
{
    public int foodId = 10;
    [SerializeField] private int idMachine;
    [SerializeField] private Food[] YakiFoods;
    [SerializeField] private Transform[] spawnPosition;
    [SerializeField] private bool isEvent = false;
    private bool isIngredientUnlocked;
    // Start is called before the first frame update
    void Start()
    {
        originScale = transform.localScale;
        int currentLevel = LevelDataController.Instance.currentLevel.id;
        isIngredientUnlocked = isEvent ? EventDataController.Instance.IsIngredientUnlocked(foodId, currentLevel) : DataController.Instance.IsIngredientUnlocked(foodId, currentLevel);
        transform.gameObject.SetActive(isIngredientUnlocked);
    }
    protected override void Update()
    {
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.AutoServe))
        {
            AutoServe();
        }
    }
    private bool AutoServe()
    {
        for (int j = 0; j < YakiFoods.Length; j++)
        {
            if (YakiFoods[j] != null && CustomerFactory.Instance.CheckFood(YakiFoods[j].FoodId))
            {
                ServeFood();
                GameObject instantEffect = Instantiate(autoServeEffect, transform.position, autoServeEffect.transform.rotation);
                Destroy(instantEffect, 2);
                return true;
            }
        }
        return false;
    }
    public void OnSelect(LeanFinger finger)
    {
        ServeFood();
    }
    public override void ServeFood()
    {
        for (int j = 0; j < YakiFoods.Length; j++)
        {
            bool foodHasDispose = false;
            if (YakiFoods[j] != null)
            {
                foodHasDispose = YakiFoods[j].OnClick();
                if (foodHasDispose)
                {
                    AudioController.Instance.PlaySfx(foodServeSfx);
                    YakiFoods[j] = null;
                    return;
                }
            }
        }
    }
    public override bool StoreCookedFood(int id)
    {
        int machineLevel = DataController.Instance.GetMachineLevel(idMachine);
        bool result = false;
        for (int i = 0; i < machineLevel; i++)
        {
            if (YakiFoods[i] == null)
            {
                if (machineLevel == 1)
                    YakiFoods[i] = FoodFactory.Instance.SpawnFoodById(id, spawnPosition[2]);
                else if (machineLevel == 2)
                    YakiFoods[i] = FoodFactory.Instance.SpawnFoodById(id, spawnPosition[i*2+1]);
                else if (machineLevel == 3)
                    YakiFoods[i] = FoodFactory.Instance.SpawnFoodById(id, spawnPosition[i*2]);
                YakiFoods[i].GetComponent<FoodAnimatorController>().Play("FoodAppear");
                result = true;
            }
        }
        return result;
    }
}
