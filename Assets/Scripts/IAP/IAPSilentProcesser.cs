using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPSilentProcesser : MonoBehaviour
{
    private List<string> iapPacks = new List<string>();
    public bool canProcessIAP = false;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        while (!canProcessIAP)
            yield return null;
        foreach (var packId in iapPacks)
            ProcessPurchase(packId);
        DataController.Instance.SaveData();
        Destroy(gameObject);
    }
    private void ProcessPurchase(string packId)
    {
        TextAsset data = null;
        data = Resources.Load<TextAsset>("iap_packs");
        string packsData = data.text;
        Packs packs = JsonUtility.FromJson<Packs>(packsData);
        Pack pack = packs.GetPackById(packId);
        if (pack != null)
        {
            if (pack.rubyAmount > 0)
                DataController.Instance.Ruby += pack.rubyAmount;
            if (pack.unlimitedEnergyTime > 0)
                DataController.Instance.AddUnlimitedEnergy(pack.unlimitedEnergyTime);
            if (pack.itemIds.Length > 0)
            {
                for (int i = 0; i < pack.itemIds.Length; i++)
                    DataController.Instance.AddItem(pack.itemIds[i], pack.itemAmounts[i]);
            }
        }
    }
    public void OnCompletePurchase(UnityEngine.Purchasing.Product product)
    {
        iapPacks.Add(product.definition.id);
    }
    public void OnFailPurchase(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason reason)
    {

    }
}
