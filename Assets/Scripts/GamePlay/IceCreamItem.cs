using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DigitalRuby.Tween;
public class IceCreamItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private GameObject iceCreamProp;
    public bool isTut;
    void Start()
    {
        bool isIceCreamActive = DataController.Instance.IsItemUnlocked((int)ItemType.IceCream);
        int[] itemUnlocks = DataController.Instance.GetItemDataById((int)ItemType.IceCream).GetUnlockCondition();
        gameObject.SetActive(isIceCreamActive);
        int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.IceCream);
        itemQuantity.text = _itemQuantity.ToString();
    }
    public bool CanUseIceCreamToCustomer(CustomerController customer)
    {
        if (isTut) return false;
        if (DataController.Instance.IsItemUnlocked((int)ItemType.IceCream) && DataController.Instance.GetItemQuantity((int)ItemType.IceCream) > 0)
        {

            DataController.Instance.UseItem((int)ItemType.IceCream, 1);
            int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.IceCream);
            itemQuantity.text = _itemQuantity.ToString();
            StartCoroutine(DoIceCreamEffect(customer));
            return true;
        }
        return false;
    }
    private IEnumerator DoIceCreamEffect(CustomerController customer)
    {
        itemQuantity.transform.parent.gameObject.SetActive(false);
        GameObject iceCream = Instantiate(iceCreamProp, transform.position - new Vector3(0, 0, 0.2f), Quaternion.identity);
        System.Action<ITween<Vector3>> updateIceCreamPos = (t) =>
                {
                    iceCream.transform.position = t.CurrentValue;
                };
        System.Action<ITween<Vector3>> completedIceCreamMovement = (t) =>
        {
            Destroy(iceCream);
        };
        TweenFactory.Tween("iceCream" + Random.Range(0, 100) + Time.time, iceCream.transform.position, customer.transform.position + new Vector3(0, 1.5f, -0.1f), 0.5f, TweenScaleFunctions.Linear, updateIceCreamPos, completedIceCreamMovement);
        yield return new WaitForSecondsRealtime(0.25f);
        customer.OnIceCreamActive();
        int _itemQuantity = DataController.Instance.GetItemQuantity((int)ItemType.IceCream);
        itemQuantity.transform.parent.gameObject.SetActive(true);
        itemQuantity.text = _itemQuantity.ToString();
    }
}
