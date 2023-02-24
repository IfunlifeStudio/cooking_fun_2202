using UnityEngine;
using Lean.Touch;
using DigitalRuby.Tween;
using System.Collections;

public class ServePlateController : LeanSelectableBehaviour
{
    public int id = 100;
    public Food activeFood = null;
    [SerializeField] protected SpriteRenderer dish;
    [SerializeField] protected Sprite[] dishSprites;
    [SerializeField] protected AudioClip addIngredientSfx, foodServeSfx;
    [SerializeField] protected GameObject autoServeEffect;
    [SerializeField] GameObject upgradeEffect;
    protected BoxCollider2D boxCollider2D;
    protected Vector3 originScale;
    private Vector3 originPosition;
    int plateLevel = 1;
    private void Start()
    {
        plateLevel = DataController.Instance.GetMachineLevel(id);
        originScale = transform.localScale;
        originPosition = transform.localPosition;
        boxCollider2D = GetComponent<BoxCollider2D>(); 
        boxCollider2D.enabled = false;
        //dish.sprite = dishSprites[plateLevel - 1];
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
                boxCollider2D.enabled = false;
                activeFood = null;
            }
        }
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
    public void Pulse()
    {
        System.Action<ITween<Vector3>> updateIngredientPosition = (t) =>
        {
            transform.localPosition = t.CurrentValue;
        };
        Vector3 targetPos = new Vector3(originPosition.x, originPosition.y + 0.32f, -2);
        TweenFactory.Tween("pulsePlate" + Random.Range(0, 100f), originPosition, targetPos, 0.1f, TweenScaleFunctions.QuinticEaseOut, updateIngredientPosition)
            .ContinueWith(new Vector3Tween().Setup(targetPos, originPosition, 0.1f, TweenScaleFunctions.QuinticEaseIn, updateIngredientPosition));
        AudioController.Instance.Vibrate();
    }
    public void ShowUpgradeProcessAgain()
    {
        dish.sprite = dishSprites[plateLevel - 2];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(plateLevel));
    }

    public void ShowUpgradeProcess()
    {
        dish.sprite = dishSprites[plateLevel - 1];
        if (gameObject.activeInHierarchy)
            StartCoroutine(DelayUpgrade(plateLevel + 1));
    }
    public void SetupSkin()
    {
        dish.sprite = dishSprites[plateLevel - 1];
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
}
