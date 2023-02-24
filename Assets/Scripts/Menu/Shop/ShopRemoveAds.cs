using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopRemoveAds : MonoBehaviour
{
/*    private void Start()
    {
        if (DataController.Instance.RemoveAds == 0)
        {
            if (CodelessIAPStoreListener.initializationComplete)
            {
                if (CodelessIAPStoreListener.Instance.GetProduct("com.cscmobi.cooking.love.free.removead").hasReceipt)
                    DataController.Instance.RemoveAds = 1;
            }
        }
        gameObject.SetActive(DataController.Instance.RemoveAds == 0);
    }*/
    public void OnBtnBuyClick()
    {
        DatabaseController.Instance.SpawnRemoveAdsSuccessOverlay();
        DataController.Instance.RemoveAds = 1;
        //StartCoroutine(DelayDeactive());
    }
    public void RestoreNonConsumed()
    {
        if (CodelessIAPStoreListener.initializationComplete)
        {
            if (CodelessIAPStoreListener.Instance.GetProduct("com.cscmobi.cooking.love.free.removead").hasReceipt)
            {
                DataController.Instance.RemoveAds = 1;
                DatabaseController.Instance.SpawnRestoreOverlay();
            }
            else
                DatabaseController.Instance.SpawnRestoreFailOverlay();
        }
    }
    private IEnumerator DelayDeactive()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(DataController.Instance.RemoveAds == 0);
    }
}
