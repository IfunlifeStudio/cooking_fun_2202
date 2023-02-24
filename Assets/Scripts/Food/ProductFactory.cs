using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductFactory : MonoBehaviour
{
    private static ProductFactory instance = null;
    public static ProductFactory Instance
    {
        get { return instance; }
    }
    [SerializeField] private Product[] productPrefabs;
    [SerializeField] private bool isEvent = false;
    private List<Product> productsPool;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            productsPool = new List<Product>();
        }
        else
            Destroy(gameObject);
    }
    public Product SpawnProductById(int id, Vector3 originPos, Transform parent)
    {
        Product product = null;
        foreach (var tmp in productsPool)
            if (tmp.FoodId == id)
            {
                product = tmp;
                break;
            }
        if (product != null)
        {
            productsPool.Remove(product);
            product.Init(originPos, parent);
            return product;
        }
        foreach (var tmp in productPrefabs)
            if (tmp.FoodId == id)
            {
                product = tmp;
                break;
            }
        Product spawnProduct = product.Spawn(originPos, parent);
        return spawnProduct;
    }
    public void DisposeProduct(Product product)
    {
        productsPool.Add(product);
    }
    public int[] GetProductByType(int type)
    {
        List<int> result = new List<int>();
        foreach (var product in productPrefabs)
            if (product.Type == type && product.IsProductUnlock(isEvent))
                result.Add(product.FoodId);
        return result.ToArray();
    }
}
