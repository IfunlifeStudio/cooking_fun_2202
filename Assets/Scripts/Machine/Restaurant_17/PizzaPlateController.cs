using DigitalRuby.Tween;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaPlateController : Machine
{

    [SerializeField] protected SpriteRenderer dish;
    [SerializeField] protected Sprite[] dishSprites;
    [SerializeField] protected AudioClip addIngredientSfx, foodServeSfx;
    [SerializeField] protected PizzaMachineController[] machine;
    [SerializeField] private GameObject upgradeEffect;
    private int idFood = 0;
    public Food activeFood = null;
    protected BoxCollider2D boxCollider2D;
    int plateLevel = 1;
    protected Vector3 originScale;
    private void Start()
    {
        plateLevel = DataController.Instance.GetMachineLevel(id);
        originScale = transform.localScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }

    public void OnSelect(LeanFinger finger)
    {
        ScalePlate();
        bool result = false;
        if (state == MachineStates.Standby)
        {
            for (int i = 0; i < machine.Length; i++)
            {
                if (!machine[i].IsMachineReady()) continue;
                if (machine[i].StoreCookedFood(idFood))
                {
                    activeFood.DisposeFood();
                    activeFood = null;
                    idFood = 0;
                    result = true;
                    break;
                }
            }

        }
        else if (state == MachineStates.Burned && Time.time - clickTimeStamp < 0.5f)
        {
            result = true;
            TossFood();
        }
        if (result)
        {
            boxCollider2D.enabled = false;
        }
        else
        {
            machine[0].CookingWarning();
        }
        clickTimeStamp = Time.time;
    }
    public void PizzaTut()
    {
        if (state == MachineStates.Standby)
        {
            for (int i = 0; i < machine.Length; i++)
            {
                if (!machine[i].IsMachineReady()) continue;
                if (machine[i].StoreCookedFood(idFood))
                {
                    activeFood.DisposeFood();
                    activeFood = null;
                    idFood = 0;
                    break;
                }
            }
        }
    }
    public virtual bool StoreCookedFood(int id)
    {
        if (activeFood == null)
        {
            boxCollider2D.enabled = true;
            activeFood = FoodFactory.Instance.SpawnFoodById(id, transform);
            //activeFood.GetComponent<FoodAnimatorController>().Play("FoodAppear");
            AudioController.Instance.PlaySfx(addIngredientSfx);
            boxCollider2D.enabled = true;
            idFood = 17000;
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
                idFood += id;
                result = true;
            }
        }
        return result;
    }

    public override bool ServeFood()
    {
        return false;
    }

    public override bool FillIngredent()
    {
        return false;
    }

    public override void TossFood()
    {
        state = MachineStates.Standby;
        transform.GetChild(0).gameObject.SetActive(false);
        FindObjectOfType<TrashBinController>().TossTrash(foodId);
    }

    public override void BackToWorkingStatus()
    {
    }

    public override void SetupSkin()
    {
        dish.sprite = dishSprites[plateLevel - 1];
    }

    public override void ShowUpgradeProcessAgain()
    {
        plateLevel = DataController.Instance.GetMachineLevel(id);
        dish.sprite = dishSprites[plateLevel - 2];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(plateLevel));
    }

    public override void ShowUpgradeProcess()
    {
        plateLevel = DataController.Instance.GetMachineLevel(id);
        dish.sprite = dishSprites[plateLevel - 1];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(plateLevel + 1));
    }
    IEnumerator DelayUpgrade(int level)
    {
        yield return new WaitForSeconds(5f);
        if (level > 3) level = 3;
        dish.sprite = dishSprites[level - 1];
        ScalePlate();
        GameObject go = Instantiate(upgradeEffect, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(go, 1);
    }
    public void ScalePlate()
    {
        System.Action<ITween<Vector3>> updateMachineScale = (t) =>
        {
            transform.localScale = t.CurrentValue;
        };
        Vector3 targetScale = new Vector3(originScale.x * 1.05f, originScale.y * 1.05f, originScale.z);
        TweenFactory.Tween("Machine" + Random.Range(0, 100f), originScale, targetScale, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateMachineScale)
            .ContinueWith(new Vector3Tween().Setup(targetScale, originScale, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateMachineScale));


    }
}
