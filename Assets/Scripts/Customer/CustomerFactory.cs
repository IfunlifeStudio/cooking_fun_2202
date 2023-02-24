using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CustomerFactory : MonoBehaviour
{
    private static CustomerFactory instance = null;
    public static CustomerFactory Instance
    {
        get { return instance; }
    }
    [SerializeField] private Transform[] seats, spawnPos;
    [SerializeField] private CustomerController[] customerPrefabs = default;
    public List<Transform> freeSeats = default;
    public List<CustomerController> customersPool = default, activeCustomers = default;
    private int[] customerIds;
    [SerializeField] bool isTut = false;
    [HideInInspector] public bool isWorking = false, isViolate = false;
    [SerializeField] private bool isEvent = false;
    private bool isPuddingActive = false, hasShowOffSkin = false, hasSantaSkin;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            freeSeats = new List<Transform>();
            freeSeats.AddRange(seats);
            customersPool = new List<CustomerController>();
            activeCustomers = new List<CustomerController>();
            customerIds = new int[customerPrefabs.Length];
            List<int> tmpIds = new List<int>();
            for (int i = 0; i < customerPrefabs.Length; i++)
            {
                if (customerPrefabs[i].HasUnlock)
                {
                    if (customerPrefabs[i].customerId == 320000)
                        hasSantaSkin = true;
                    tmpIds.Add(customerPrefabs[i].customerId);
                }

            }
            customerIds = tmpIds.ToArray();
        }
        else
            Destroy(gameObject);
    }
    public void SpawnAdditionWave(int numberOfCustomer)//add parameter to spawn only fix customer
    {
        StopAllCoroutines();
        StartCoroutine(SpawnLoop(true, numberOfCustomer));
    }
    public void SpawnWave()
    {
        StartCoroutine(SpawnLoop(false, 0));
    }
    public void StopSpawn()
    {
        isWorking = false;
        for (int i = activeCustomers.Count - 1; i > -1; i--)
            activeCustomers[i].StopCustomer();
        activeCustomers.Clear();
    }
    private IEnumerator SpawnLoop(bool isLimitCustomer, int limitCustomers)
    {
        isWorking = true;
        LevelData currentLevel = LevelDataController.Instance.currentLevel;
        int waveCount = currentLevel.waves.Length;//
        OrderFactory ordersCollection;
        if (waveCount < 6)//work around the issue of food spawn function when new food tier unlocked
            ordersCollection = new OrderFactory(currentLevel.GetCurrentWave(), currentLevel.GetFoodSpawnPercent());
        else
        {
            int index = Mathf.Min(2, DataController.Instance.GetLevelState(currentLevel.chapter, currentLevel.id));
            int levelBreakPoint = currentLevel.chapter > 1 ? 12 : 5;
            if (DataController.Instance.GetHighestLevel(currentLevel.chapter) >= levelBreakPoint)
                index += 3;
            ordersCollection = new OrderFactory(currentLevel.waves[index], currentLevel.GetFoodSpawnPercent(), isTut);
        }
        int currentCustomer = 0;
        int totalCustomer = ordersCollection.orders.Length;
        if (isLimitCustomer) totalCustomer = limitCustomers;
        while (currentCustomer < totalCustomer && isWorking)
        {
            if (freeSeats.Count > 0)
            {
                float spawnInterval = currentLevel.GetCustomerAppearInterval();
                Transform freeSeat = freeSeats[Random.Range(0, freeSeats.Count)];
                freeSeats.Remove(freeSeat);
                int index = Random.Range(0, 100) > 50 ? 0 : 1;
                float moveTime = CalculateCustomerMoveTime(spawnPos[index].position, freeSeat.position, currentLevel.GetCustomerSpeed());
                if (moveTime < spawnInterval)
                    yield return new WaitForSeconds(spawnInterval - moveTime);//delay customer move correspond to the move time
                SpawnCustomer(spawnPos[index], spawnPos[1 - index], freeSeat, ordersCollection.orders[currentCustomer]);
                currentCustomer++;
                if (moveTime < spawnInterval)
                    yield return new WaitForSeconds(moveTime - 1);
                else
                    yield return new WaitForSeconds(spawnInterval - 1);
            }
            yield return new WaitForSeconds(1);
        }
        isWorking = false;
    }
    public void SpawnCustomer(Transform spawnPos, Transform exitPos, Transform seat, int[] productIds)
    {
        if (!isWorking) return;
        int id = -1;
        List<int> activeIds = new List<int>();
        foreach (var _customer in activeCustomers)
            activeIds.Add(_customer.customerId);
        List<int> availableIds = new List<int>();
        for (int i = 0; i < customerIds.Length; i++)
            if (!activeIds.Contains(customerIds[i]))
                availableIds.Add(customerIds[i]);
        int num_showoff = PlayerPrefs.GetInt("showoff_skin_2022", 0);
        if (num_showoff >= 1 && num_showoff < 5 && !hasShowOffSkin && hasSantaSkin)
        {
            num_showoff += 1;
            id = CustomerController.SANTA_SKIN_ID;
            hasShowOffSkin = true;
            PlayerPrefs.SetInt("showoff_skin_2022", num_showoff);
        }
        else
        {
            if (availableIds.Count > 0)
                id = availableIds[Random.Range(0, availableIds.Count)];
            else
                id = customerIds[Random.Range(0, customerIds.Length)];
        }
        CustomerController customer = null;
        foreach (CustomerController tmp in customersPool)//find customer in pool
            if (tmp.customerId == id)
            {
                customer = tmp;
                break;
            }
        if (customer != null)
        {
            activeCustomers.Add(customer);
            customersPool.Remove(customer);
            customer.Init(spawnPos, exitPos, seat, productIds, isPuddingActive, isEvent);
            return;
        }
        foreach (CustomerController tmp in customerPrefabs)//spawn a new customer
            if (tmp.customerId == id)
                customer = tmp;
        CustomerController controller = customer.Spawn(spawnPos, exitPos, seat, productIds, isPuddingActive, isEvent);
        activeCustomers.Add(controller);
    }
    private float CalculateCustomerMoveTime(Vector3 spawnPos, Vector3 targetPos, float speed)
    {
        float result = Vector3.Distance(spawnPos, targetPos) / speed;
        return result;
    }
    public void DisposeCustomer(CustomerController customer)
    {
        customersPool.Add(customer);
        activeCustomers.Remove(customer);
    }
    public void ReleaseSeat(Transform seat, CustomerController customer)//mark the seat as free
    {
        freeSeats.Add(seat);
    }
    public bool CheckFood(int foodId)//check if this food can serve
    {
        bool result = false;
        foreach (var customer in activeCustomers)
            if (customer.CheckFood(foodId))
            {
                result = true;
                break;
            }
        return result;
    }
    public void ServeFood(Food food)
    {
        int n = activeCustomers.Count;//bubble sort the customer base on start wait time so we always serve customer who have the longest waiting time.
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (activeCustomers[j].remainTime > activeCustomers[j + 1].remainTime)
                {
                    var temp = activeCustomers[j];
                    activeCustomers[j] = activeCustomers[j + 1];
                    activeCustomers[j + 1] = temp;
                }
        for (int i = 0; i < n; i++)
            if (activeCustomers[i].CheckFood(food.FoodId))
            {
                activeCustomers[i].ServeFood(food);
                break;
            }
    }
    public CustomerController GetCustomerHasShortestWaitime()
    {
        if (activeCustomers.Count == 0) return null;
        int n = activeCustomers.Count;//bubble sort the customer base on start wait time so we always serve customer who have the longest waiting time.
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (activeCustomers[j].remainTime > activeCustomers[j + 1].remainTime)
                {
                    var temp = activeCustomers[j];
                    activeCustomers[j] = activeCustomers[j + 1];
                    activeCustomers[j + 1] = temp;
                }
        for (int i = 0; i < n; i++)
            if (activeCustomers[i].isWaiting)
                return activeCustomers[i];
        return null;
    }
    public void OnPuddingActive()
    {
        isViolate = false;
        isPuddingActive = true;
        foreach (var customer in activeCustomers)
            customer.OnPuddingActive();
    }
    public void OnPuddingDeactive()
    {
        isPuddingActive = false;
        foreach (var customer in activeCustomers)
            customer.OnPuddingDeactive();
    }
    public void SendMessageOnCondition3Violate(Vector3 customerPosstion)
    {
        if (!isViolate)
        {
            AudioController.Instance.Vibrate();
            isViolate = true;
            MessageManager.Instance.SendMessage(
            new Message(CookingMessageType.OnConditionViolate,
            new object[] { LevelDataController.Instance.currentLevel.ConditionType(), customerPosstion }));
        }
    }
}
class OrderFactory//control the food spawn percent here. pregenerate foodid in to a list then the player can get food from here
{
    private List<int> dishs;
    private bool[] dishsState;
    public int[][] orders;
    public OrderFactory(string wave, string spawnPercent, bool isTut = default)
    {
        string[] _orders = wave.Split('_');
        int totalCustomer = _orders.Length;
        var currentLevel = LevelDataController.Instance.currentLevel;
        if ((currentLevel.chapter == 11 && currentLevel.id == 1 && isTut)
             || (currentLevel.chapter == 1 && currentLevel.id == 13 && isTut)
            || (currentLevel.chapter == 13 && currentLevel.id == 1 && isTut)
            || (currentLevel.chapter == 8 && currentLevel.id == 1 && isTut)
            || (currentLevel.chapter == 4 && currentLevel.id == 1 && isTut))
        {
            totalCustomer -= 2;
        }
        else if (isTut)
            totalCustomer -= 1;
        orders = new int[totalCustomer][];
        dishs = new List<int>();
        int[] foodPerTiers = new int[4] { 0, 0, 0, 0 };//count total food tier 1, count total food tier 2, count total food type 3
        for (int i = 0; i < totalCustomer; i++)
        {
            for (int j = 0; j < _orders[i].Length; j++)
            {
                int foodType = int.Parse(_orders[i][j].ToString());
                foodPerTiers[foodType - 1]++;
            }
        }
        string[] spawnPercents = spawnPercent.Split('-');//process the spawn percent
        Dictionary<int, int> spawnPercentsTable = new Dictionary<int, int>();
        for (int i = 0; i < spawnPercents.Length; i++)
        {
            if (spawnPercents[i].Contains("_"))
            {
                string[] datas = spawnPercents[i].Split('_');
                spawnPercentsTable.Add(int.Parse(datas[0]), int.Parse(datas[1]));
            }
        }
        for (int tier = 1; tier < foodPerTiers.Length + 1; tier++)
        {
            int[] foodIds = ProductFactory.Instance.GetProductByType(tier);
            float[] foodSpawnPercents = new float[foodIds.Length];
            int remainSpawnPercent = 100;
            int unconfigCount = 0;
            for (int i = 0; i < foodSpawnPercents.Length; i++)
                if (spawnPercentsTable.ContainsKey(foodIds[i]))
                {
                    foodSpawnPercents[i] = spawnPercentsTable[foodIds[i]] / 100f;
                    remainSpawnPercent -= spawnPercentsTable[foodIds[i]];
                }
                else
                {
                    foodSpawnPercents[i] = -1f;
                    unconfigCount++;
                }
            if (unconfigCount > 0)
            {
                float averageSpawnPercent = remainSpawnPercent / (100f * unconfigCount);
                for (int i = 0; i < foodSpawnPercents.Length; i++)
                    if (foodSpawnPercents[i] == -1f)
                        foodSpawnPercents[i] = averageSpawnPercent;
            }
            int totalProcessedFood = 0;//calculate food id base on percent
            for (int i = 0; i < foodIds.Length; i++)
            {
                int totalDish = (int)(foodPerTiers[tier - 1] * foodSpawnPercents[i]);
                if (i == foodIds.Length - 1)
                    totalDish = foodPerTiers[tier - 1] - totalProcessedFood;
                for (int j = 0; j < totalDish; j++)
                {
                    dishs.Add(foodIds[i]);
                    totalProcessedFood++;
                }
            }
        }
        dishsState = new bool[dishs.Count];
        for (int i = 0; i < dishsState.Length; i++)
            dishsState[i] = false;
        for (int i = 0; i < totalCustomer; i++)//create all order here
        {
            int[] order = new int[_orders[i].Length];
            for (int j = 0; j < _orders[i].Length; j++)
            {
                int foodType = int.Parse(_orders[i][j].ToString());
                int foodId = GetFoodIdByType(foodType);
                order[j] = foodId;
            }
            orders[i] = order;
        }
    }
    private int GetFoodIdByType(int type)
    {
        int[] foodIds = ProductFactory.Instance.GetProductByType(type);
        int foodId;
        do
        {
            foodId = foodIds[Random.Range(0, foodIds.Length)];
        }
        while (!IsThisIdAvailable(foodId));
        for (int i = 0; i < dishs.Count; i++)
        {
            if (dishs[i] == foodId && !dishsState[i])
            {
                dishsState[i] = true;
                break;
            }
        }
        return foodId;
    }
    private bool IsThisIdAvailable(int foodId)
    {
        bool result = false;
        for (int i = 0; i < dishs.Count; i++)
        {
            if (dishs[i] == foodId && !dishsState[i])
            {
                result = true;
                break;
            }
        }
        return result;
    }
}