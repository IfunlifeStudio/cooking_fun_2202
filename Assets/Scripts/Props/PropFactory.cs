using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropFactory : MonoBehaviour
{
    private static PropFactory instance = null;
    public static PropFactory Instance { get { return instance; } }
    [SerializeField] private CoinPropController coinPrefab;
    [SerializeField] private HeartPropController heartPrefab;
    [SerializeField] private DishPropController dishPrefab;
    private List<CoinPropController> coinsPool = new List<CoinPropController>();
    private List<HeartPropController> heartsPool = new List<HeartPropController>();
    private List<DishPropController> dishesPool = new List<DishPropController>();
    private void Start()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
    public void SpawnCoin(int value, Vector3 position, Transform target, UnityAction action = null)
    {
        if (coinsPool.Count > 0)
        {
            var coin = coinsPool[coinsPool.Count - 1];
            coin.Init(coin.gameObject,value, position, target,action);
            coinsPool.Remove(coin);
        }
        else
            coinPrefab.Spawn(value, position, target,action);
    }
    public void DisposeCoin(CoinPropController coin)
    {
        coinsPool.Add(coin);
    }
    public void SpawnHeart(int value, Vector3 position, Transform target, UnityAction action = null)
    {
        if (heartsPool.Count > 0)
        {
            var heart = heartsPool[heartsPool.Count - 1];
            heart.Init(heart.gameObject,value, position, target,action);
            heartsPool.Remove(heart);
        }
        else
            heartPrefab.Spawn(value, position, target,action);
    }
    public void DisposeHeart(HeartPropController heart)
    {
        heartsPool.Add(heart);
    }
    public void SpawnDish(int value, Vector3 position, Transform target,UnityAction action=null)
    {
        if (dishesPool.Count > 0)
        {
            var dish = dishesPool[dishesPool.Count - 1];
            dish.Init(dish.gameObject,value, position, target,action);
            dishesPool.Remove(dish);
        }
        else
            dishPrefab.Spawn(value, position, target,action);
    }
    public void DisposeDish(DishPropController dish)
    {
        dishesPool.Add(dish);
    }
}
