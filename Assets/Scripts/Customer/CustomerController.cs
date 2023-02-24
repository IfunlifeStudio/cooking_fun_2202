using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
public class CustomerController : MonoBehaviour
{
    public const int SANTA_SKIN_ID = 320000;
    public int customerId = 100;
    [HideInInspector] public float remainTime = 0f;
    private float startWaitTimeStamp = 0f, speed = 2f, waitTime = 15f;
    [SerializeField] private List<int> foodOrders;
    [SerializeField] private bool isTut;
    [SerializeField] private float waitTimePercent = 1f;
    private Transform[] productsPos = default;
    private GameObject orderBoard;
    private GameObject heartIcon;
    private GameObject tickIcon;
    [SerializeField] private Transform seatTarget = default, exitTarget = default;
    public List<Product> activeProducts;
    public bool isWaiting = false, isUnderPuddingEffect = false, isLeaving = false;
    [SerializeField] private bool isEvent = false;
    CustomerAnimationController animationController;
    private SeatController currentSeat;
    [SerializeField] private GameObject puddingEffectPrefab;
    private GameObject puddingEffect;
    private GameController gameController;
    ComboBarController comboBar;
    bool canUseIceCream;
    IceCreamItem iceCreamItem;
    private IEnumerator Start()
    {
        if (isTut)
        {
            gameObject.SetActive(true);
            this.waitTime = 30;
            this.speed = 2.75f;
            activeProducts = new List<Product>();
            animationController = GetComponent<CustomerAnimationController>();
            currentSeat = seatTarget.GetComponent<SeatController>();
            this.productsPos = currentSeat.productsPos;
            this.orderBoard = currentSeat.orderBoard;
            this.heartIcon = currentSeat.heartIcon;
            this.tickIcon = currentSeat.tickIcon;
            isUnderPuddingEffect = false;
            StartCoroutine(Act());
            yield return null;
            CustomerFactory.Instance.activeCustomers.Add(this);
            CustomerFactory.Instance.freeSeats.Remove(seatTarget);
        }
        canUseIceCream = true;
        comboBar = FindObjectOfType<ComboBarController>();
        iceCreamItem = FindObjectOfType<IceCreamItem>();
        gameController = FindObjectOfType<GameController>();
    }
    public bool HasUnlock
    {
        get
        {
            if (customerId < SANTA_SKIN_ID)
            {
                if (customerId == 310000)
                {
                    if (BattlePassDataController.Instance.HasUnlockVip) return true;
                    else return false;
                }
                else return true;
            }
            else
            {
                return DataController.Instance.HasUnlockCustomerSkin(customerId);
            }
        }
    }
    public void OnIceCreamActive()
    {
        animationController.PlayAnimation("happy", true);
        remainTime = waitTime;
        canUseIceCream = true;
    }
    public void OnPuddingActive()
    {
        puddingEffect = Instantiate(puddingEffectPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity, transform);
        isUnderPuddingEffect = true;
        remainTime = waitTime;
    }
    public void OnUseSecondChance()
    {
        waitTime = LevelDataController.Instance.currentLevel.GetCustomerWaitTime();
        remainTime = waitTime;
        isLeaving = false;
        isWaiting = true;
    }
    public void OnPuddingDeactive()
    {
        Destroy(puddingEffect);
        isUnderPuddingEffect = false;
    }
    public bool CheckFood(int foodId)
    {
        if (!isWaiting) return false;
        return foodOrders.Contains(foodId);
    }
    public void ServeFood(Food food)
    {
        StartCoroutine(_ServeFood(food));
    }
    private IEnumerator _ServeFood(Food food)
    {

        MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnFoodServed));
        foodOrders.Remove(food.FoodId);
        int foodValue = food.GetFoodValue(isEvent);
        if (LevelDataController.Instance.CheckItemActive((int)ItemType.Double_Coin))
            foodValue *= 2;
        gameController.IncreaseGold(foodValue, false);
        LevelData currentLevel = LevelDataController.Instance.currentLevel;
        Product servedProduct = null;
        foreach (Product product in activeProducts)
            if (product.FoodId == food.FoodId)
            {
                servedProduct = product;
                break;
            }
        if (servedProduct != null)
        {
            int combo = comboBar.Combo + 1;
            activeProducts.Remove(servedProduct);
            gameController.OnFoodServed(food.FoodId);
            Vector3 servedProductParentPos = servedProduct.transform.parent.position;
            Product collectedProduct = ProductFactory.Instance.SpawnProductById(food.FoodId, food.transform.position + new Vector3(0, 0.2f, 0), servedProduct.transform.parent);
            float foodFlyTime = Mathf.Clamp((food.transform.position - servedProduct.transform.position).magnitude / 10, 0.15f, 0.23f);
            System.Action<ITween<Vector3>> updateServedProductScale = (t) =>
                    {
                        servedProduct.transform.localScale = t.CurrentValue;
                    };
            System.Action<ITween<Vector3>> completedServedProductScale = (t) =>
            {
                servedProduct.Dispose();
            };
            TweenFactory.Tween("ServedProduct" + servedProduct.FoodId + Time.time + foodFlyTime + Random.Range(0f, 1000f), servedProduct.transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), foodFlyTime, TweenScaleFunctions.QuinticEaseIn, updateServedProductScale, completedServedProductScale);
            System.Action<ITween<Vector3>> updateCollectedProductPos = (t) =>
                    {
                        collectedProduct.transform.position = t.CurrentValue;
                    };
            System.Action<ITween<Vector3>> completedCollectedProductPos = (t) =>
            {
                collectedProduct.Dispose();
            };
            TweenFactory.Tween("CollectedProduct" + collectedProduct.FoodId + Time.time + foodFlyTime + Random.Range(0f, 1000f), collectedProduct.transform.position, servedProduct.transform.position, foodFlyTime, TweenScaleFunctions.QuadraticEaseIn, updateCollectedProductPos, completedCollectedProductPos);
            System.Action<ITween<Vector3>> updateCollectedProductScale = (t) =>
                    {
                        if (collectedProduct != null)
                            collectedProduct.transform.localScale = t.CurrentValue;
                    };
            TweenFactory.Tween("CollectedProductScale" + collectedProduct.FoodId + Time.time + foodFlyTime + Random.Range(0f, 1000f), Vector3.one, new Vector3(0.25f, 0.25f, 0.25f), foodFlyTime, TweenScaleFunctions.QuinticEaseIn, updateCollectedProductScale);
            //AchivementController.Instance.OnServeFood(food.FoodId);
            yield return new WaitForSeconds(foodFlyTime);
            animationController.PlayAnimation("happy", false);

            comboBar.SpawnComboEffect(currentSeat.productsPos[0].position + new Vector3(0, 0.2f, -0.1f), combo);
            if (currentLevel.ObjectiveType() == 3)
            {
                Transform tmp = gameController.objectiveIcon.transform;
                PropFactory.Instance.SpawnDish(food.FoodId, servedProductParentPos, tmp, () => { gameController.ScaleObjecttiveIcon(); });
            }
            //else

        }
        if (foodOrders.Count > 0)
            remainTime = Mathf.Min(remainTime + waitTime * 0.2f, waitTime);//bonus 20% time per each product except the last one
        for (int i = 0; i < activeProducts.Count; i++)
        {
            Transform parent = GetProductPos(i, activeProducts.Count);
            activeProducts[i].SetParent(parent);
        }
        Transform target = null;//spawn coin prefab
        if (currentLevel.ObjectiveType() == 1) target = gameController.objectiveIcon.transform;
        PropFactory.Instance.SpawnCoin(foodValue, transform.position + new Vector3(0, 1, -1), target, () => { gameController.ScaleObjecttiveIcon(); });
    }
    private IEnumerator Act()
    {
        yield return StartCoroutine(TakeASeat());
        yield return StartCoroutine(Waiting());
        yield return StartCoroutine(Leave());
    }
    private IEnumerator TakeASeat()
    {
        isLeaving = false;
        isWaiting = false;
        yield return null;//wait for other component to init
        animationController.SetSkeletonState(exitTarget.transform.position.x < 0);
        animationController.PlayAnimation("move", true);
        yield return StartCoroutine(MoveToTarget(seatTarget));
        tickIcon.SetActive(false);
        orderBoard.SetActive(true);
        currentSeat.UpdateWaitBar(1);
        heartIcon.SetActive(true);
        if (LevelDataController.Instance.currentLevel.ObjectiveType() == 2)
        {
            heartIcon.SetActive(true);
            MessageManager.Instance.SendMessage(
                new Message(CookingMessageType.HeartGoal,
                new object[] { heartIcon.transform.position }
                ));
        }
        else
            heartIcon.SetActive(false);
        animationController.PlayAnimation("idle", true);
        animationController.PlayAppearAudioClip();
        seatTarget.GetComponent<SeatController>().Play("Appear");
        yield return new WaitForSeconds(0.3f);
        remainTime = waitTime;
        if (isTut) remainTime = remainTime * waitTimePercent;
        startWaitTimeStamp = Time.time;
        isWaiting = true;
        for (int i = 0; i < foodOrders.Count; i++)
        {
            Transform parent = GetProductPos(i, foodOrders.Count);
            Product product = ProductFactory.Instance.SpawnProductById(foodOrders[i], parent.position, parent);
            product.GetComponent<FoodAnimatorController>().Play("FoodAppear");
            MessageManager.Instance.SendMessage(
                new Message(CookingMessageType.OnFoodOrder,
                new object[] { product.FoodId, product.transform.position }
                ));
            activeProducts.Add(product);
        }
    }
    private IEnumerator Waiting()
    {
        LevelData currentLevel = LevelDataController.Instance.currentLevel;
        bool canCustomerLeave = !(currentLevel.chapter == 1
        && (0 < currentLevel.id && currentLevel.id < 6)
        && DataController.Instance.GetLevelState(1, currentLevel.id) == 0);
        while (remainTime > 0 && foodOrders.Count > 0)
        {
            if (!isUnderPuddingEffect)
                remainTime -= Time.deltaTime;
            if (!canCustomerLeave) remainTime = Mathf.Max(0.1f, remainTime);
            currentSeat.UpdateWaitBar(remainTime / waitTime);
            animationController.UpdateWaitingAnimation(remainTime / waitTime);
            if (remainTime / waitTime < 0.5f)
            {
                if (heartIcon.activeInHierarchy)
                    heartIcon.SetActive(false);
                if (canUseIceCream)
                {
                    canUseIceCream = false;
                    iceCreamItem?.CanUseIceCreamToCustomer(this);
                }
            }
            else
            {
                if (!heartIcon.activeInHierarchy && LevelDataController.Instance.currentLevel.ObjectiveType() == 2)
                    heartIcon.SetActive(true);
            }
            yield return null;
        }
        if (remainTime / waitTime > 0.5f)
        {
            if (LevelDataController.Instance.currentLevel.ObjectiveType() == 2)
            {
                heartIcon.SetActive(false);
                gameController.OnCustomerGiveHeart();
                Transform tmp = gameController.objectiveIcon.transform;
                PropFactory.Instance.SpawnHeart(1, heartIcon.transform.position, tmp, () => { gameController.ScaleObjecttiveIcon(); });
            }
            else
                gameController.OnCustomerGiveHeart();
        }

    }
    private IEnumerator Leave()
    {
        isLeaving = true;
        isWaiting = false;
        CustomerFactory.Instance.ReleaseSeat(seatTarget, this);
        bool isCustomerHappy = foodOrders.Count == 0;
        if (!isCustomerHappy)
        {
            if (!gameController.isCheat && gameController.isPlaying)
            {
                if (LevelDataController.Instance.currentLevel.ConditionType() == 3)
                    CustomerFactory.Instance.SendMessageOnCondition3Violate(transform.position + new Vector3(0, 2, 0));
                gameController.OnCustomerUnserved();
            }
        }
        else
        {
            animationController.PlayHappyAudioClip();
            MessageManager.Instance.SendMessage(new Message(CookingMessageType.OnOrderCompleted));
            yield return new WaitForSeconds(0.5f);
            tickIcon.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        foreach (var food in activeProducts)//clear all remain food
            food.Dispose();
        gameController.OnCustomerServed();
        //AchivementController.Instance.OnCustomerServed();
        seatTarget.GetComponent<SeatController>().Play("Disappear");
        yield return new WaitForSeconds(1f);
        orderBoard.SetActive(false);
        if (isCustomerHappy)
        {
            animationController.PlayAnimation("happy_out", true);
        }
        else
            animationController.PlayAnimation("move", true);
        yield return StartCoroutine(MoveToTarget(exitTarget));
        gameController.OnCustomerLeave();
        gameObject.SetActive(false);
        OnPuddingDeactive();
        CustomerFactory.Instance.DisposeCustomer(this);
    }
    private IEnumerator MoveToTarget(Transform target)
    {
        float distance;
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
            yield return null;
            distance = transform.position.x - target.position.x;
        }
        while (distance * distance > 0.01f);
    }
    private Transform GetProductPos(int index, int total)
    {
        Transform parent = null;
        if (total == 1)
            parent = productsPos[2];
        else if (total == 2)
            parent = productsPos[index * 2 + 1];
        else if (total == 3)
            parent = productsPos[index * 2];
        return parent;
    }
    public CustomerController Spawn(Transform spawnPos, Transform exitPos, Transform target, int[] foods, bool isPuddingActive, bool isEvent)
    {
        GameObject go = Instantiate(gameObject);
        CustomerController controller = go.GetComponent<CustomerController>();
        controller.Init(spawnPos, exitPos, target, foods, isPuddingActive, isEvent);
        return controller;
    }
    public void StopCustomer()
    {
        if (isLeaving) return;
        remainTime = 0;
        StopAllCoroutines();
        StartCoroutine(Leave());
    }
    public void Init(Transform spawnPos, Transform exitPos, Transform target, int[] foods, bool isPuddingActive, bool isEvent)
    {
        seatTarget = target;
        exitTarget = exitPos;
        transform.position = spawnPos.position;
        waitTimePercent = 1;
        gameObject.SetActive(true);
        this.waitTime = LevelDataController.Instance.currentLevel.GetCustomerWaitTime();
        this.speed = LevelDataController.Instance.currentLevel.GetCustomerSpeed();
        foodOrders = new List<int>();
        this.foodOrders.AddRange(foods);
        activeProducts = new List<Product>();
        animationController = GetComponent<CustomerAnimationController>();
        currentSeat = seatTarget.GetComponent<SeatController>();
        this.productsPos = currentSeat.productsPos;
        this.orderBoard = currentSeat.orderBoard;
        this.heartIcon = currentSeat.heartIcon;
        this.tickIcon = currentSeat.tickIcon;
        isUnderPuddingEffect = isPuddingActive;
        if (isPuddingActive)
            OnPuddingActive();
        this.isEvent = isEvent;
        StartCoroutine(Act());
    }
    public void ResetStatusCustomer()
    {
        remainTime = waitTime;
    }
    public Product GetFirstProductOrder()
    {
        if (activeProducts.Count > 0)
            return activeProducts[0];
        return null;
    }
}
