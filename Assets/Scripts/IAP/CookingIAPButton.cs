using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Globalization;
using com.adjust.sdk;
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using UnityEngine.Networking;

namespace UnityEngine.Purchasing
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Unity IAP/IAP Button")]
    [HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
    public class CookingIAPButton : IAPButton
    {
        [Tooltip("[Optional] Displays the localized title from the app store")]
        public TextMeshProUGUI titleTextTMP;
        [Tooltip("[Optional] Displays the localized description from the app store")]
        public TextMeshProUGUI descriptionTextTMP;

        [Tooltip("[Optional] Displays the localized price from the app store")]
        public TextMeshProUGUI priceTextTMP;
        void Start()
        {
            Button button = GetComponent<Button>();

            if (buttonType == ButtonType.Purchase)
            {
                if (button)
                {
                    button.onClick.AddListener(PurchaseProduct);
                    onPurchaseComplete.AddListener(OnCompletePurchase);
                    onPurchaseFailed.AddListener(OnFailPurchase);
                }

                if (string.IsNullOrEmpty(productId))
                {
                    Debug.LogError("IAPButton productId is empty");
                }

                if (!CodelessIAPStoreListener.Instance.HasProductInCatalog(productId))
                {
                    Debug.LogWarning("The product catalog has no product with the ID \"" + productId + "\"");
                }
            }
            else if (buttonType == ButtonType.Restore)
            {
                if (button)
                {
                    button.onClick.AddListener(Restore);
                }
            }
        }

        void OnEnable()
        {
            if (buttonType == ButtonType.Purchase)
            {
                CodelessIAPStoreListener.Instance.AddButton(this);
                if (CodelessIAPStoreListener.initializationComplete)
                {
                    UpdateText();
                }
            }
        }

        void OnDisable()
        {
            if (buttonType == ButtonType.Purchase)
            {
                CodelessIAPStoreListener.Instance.RemoveButton(this);
            }
        }
        public void OnCompletePurchase(Product product)
        {
            RewardPurchase(product);
            if (DataController.Instance.IsTestUser) return;

            var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(product.receipt);

            var purchaseToken = (string)wrapper["TransactionID"];
#if UNITY_IOS
            var purchaseToken = (string)wrapper["Payload"];
#endif
            ValidateIAP(product.definition.id, purchaseToken,
                () =>
                {
                    ValidateIAPWithAdjust(product);
                    float packPriceUSD = IAPPackHelper.GetTrackingPackPrice(product.definition.id);
                    DataController.Instance.GetGameData().userIapValue += packPriceUSD;
                    if (DataController.Instance.GetGameData().userIapValue == 0)
                    {
                        DataController.Instance.GetGameData().userIapValue = PlayerPrefs.GetFloat("user_iap_value", 0);
                        if (DataController.Instance.GetGameData().userIapValue == 0)
                            APIController.Instance.LogEventFirstInapp(product.definition.id);
                    }

                    //string shop_Type = PlayerPrefs.GetString("shop_loaction", "shop_full_inhome");
                    //APIController.Instance.SetProperty("inapp_type", shop_Type);
                    //APIController.Instance.LogEventBuySuccessIap(shop_Type, product.definition.id);

                    if (!(Application.internetReachability == NetworkReachability.NotReachable) && !DataController.Instance.IsTestUser)
                        APIController.Instance.PostData(JsonUtility.ToJson(new IapTracking(product.definition.id, "iapsuccess", 0, packPriceUSD.ToString())), "https://gsmlog.cscmobicorp.com/api/user/event");
                }, null);

        }
        private void RewardPurchase(Product product)
        {
            DataController.Instance.RemoveAds = 1;
            IAPTransactionStatus.Instance.ShowStatus(true, product);
            PlayerClassifyController.Instance.UpdatePlayerClassifyLevel();
        }
        public void OnFailPurchase(Product product, PurchaseFailureReason reason)
        {
            IAPTransactionStatus.Instance.ShowStatus(false, product, reason);
            if (!(Application.internetReachability == NetworkReachability.NotReachable) && !DataController.Instance.IsTestUser)
                APIController.Instance.PostData(JsonUtility.ToJson(new IapTracking(product.definition.id, "iapfailed", (int)reason, "0")), "https://gsmlog.cscmobicorp.com/api/user/event");
        }
        void PurchaseProduct()
        {
            if (buttonType == ButtonType.Purchase)
            {
                IAPTransactionStatus.Instance.OpenTransactionOverlay();
                CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
                var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
                float packPriceUSD = IAPPackHelper.GetTrackingPackPrice(productId);
                if (!(Application.internetReachability == NetworkReachability.NotReachable) && !DataController.Instance.IsTestUser)
                    APIController.Instance.PostData(JsonUtility.ToJson(new IapTracking(product.definition.storeSpecificId, "iapstart", 0, packPriceUSD.ToString())), "https://gsmlog.cscmobicorp.com/api/user/event");
            }
        }

        void ValidateIAPWithAdjust(Product product)
        {
            if (DataController.Instance.IsTestUser) return;
            string receipt = product.receipt;
            var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (wrapper == null)
                return;
            try
            {
#if !UNITY_EDITOR
            AdjustEvent adjustEvent = new AdjustEvent("event_token");
            adjustEvent.setRevenue((double)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
            adjustEvent.setTransactionId(product.transactionID);
            // dùng cho các game muốn custom thêm param (optional)
            //adjustEvent.addCallbackParameter(""demand"", ""value"");
            Adjust.trackEvent(adjustEvent);
#endif

            }
            catch { Debug.Log("NullReceipt"); }

        }
        void Restore()
        {
            //if (buttonType == ButtonType.Restore)
            //{
            //    if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
            //        Application.platform == RuntimePlatform.WSAPlayerX64 ||
            //        Application.platform == RuntimePlatform.WSAPlayerARM)
            //    {
            //        CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>()
            //            .RestoreTransactions();
            //    }
            //    else if (Application.platform == RuntimePlatform.IPhonePlayer ||
            //             Application.platform == RuntimePlatform.OSXPlayer ||
            //             Application.platform == RuntimePlatform.tvOS)
            //    {
            //        CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>()
            //            .RestoreTransactions(OnTransactionsRestored);
            //    }
            //    else if (Application.platform == RuntimePlatform.Android &&
            //             StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
            //    {
            //        CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<ISamsungAppsExtensions>()
            //            .RestoreTransactions(OnTransactionsRestored);
            //    }
            //    //else if (Application.platform == RuntimePlatform.Android &&
            //    //         StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
            //    //{
            //    //    CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMoolahExtension>()
            //    //        .RestoreTransactionID((restoreTransactionIDState) =>
            //    //        {
            //    //            OnTransactionsRestored(
            //    //                restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed &&
            //    //                restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
            //    //        });
            //    //}
            //    else
            //    {
            //        Debug.LogWarning(Application.platform.ToString() +
            //                         " is not a supported platform for the Codeless IAP restore button");
            //    }
            //}
        }

        void OnTransactionsRestored(bool success)
        {
            Debug.Log("Transactions restored: " + success);
            if (success)
            {
                DatabaseController.Instance.SpawnRestoreOverlay();
                DataController.Instance.RemoveAds = 1;
            }
        }
        public void ValidateIAP(string productId, string purchaseToken, Action onSuccess, Action onFail)
        {
            ValidateIAPData validateData = new ValidateIAPData(productId, purchaseToken);
            StartCoroutine(ValidateIAPCoroutine(validateData, onSuccess, onFail));
        }

        private IEnumerator ValidateIAPCoroutine(ValidateIAPData data, Action onSuccess, Action onFail)
        {
            var request = new UnityWebRequest("https://gsm.cscmobicorp.com/api/iap/verify", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 5;
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                onSuccess?.Invoke();
            }
            else
            {
                if (request.downloadHandler == null || request.downloadHandler.text == null)
                {
                    onFail?.Invoke();
                    request.Dispose();
                    yield break;
                }
                ValidateIAPResponse responseData = JsonUtility.FromJson<ValidateIAPResponse>(request.downloadHandler.text);
                if (responseData == null || !responseData.success)
                {
                    onFail?.Invoke();
                }
                else
                {
                    onSuccess?.Invoke();
                }
                request.Dispose();
            }
        }

        internal void UpdateText()
        {
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product != null)
            {
                if (titleTextTMP != null)
                {
                    titleTextTMP.text = product.metadata.localizedTitle;
                }

                if (descriptionTextTMP != null)
                {
                    descriptionTextTMP.text = product.metadata.localizedDescription;
                }
                if (priceTextTMP != null)
                {
                    priceTextTMP.text = product.metadata.localizedPriceString;
                }
            }
        }
    }
}
[System.Serializable]
public class ValidateIAPData
{
    public int os;
    public string packageName;
    public string productId;
    public string purchaseToken;
    public string accountId;
    public string appId;
    public string deviceId;
    public string version;
    public string keyhash;


    public ValidateIAPData(string productId, string purchaseToken)
    {
        this.os = (Application.platform.ToString().ToLower() == "android") ? 1 : 2;
        this.packageName = Application.identifier;
        this.productId = productId;
        this.purchaseToken = purchaseToken;
        this.accountId = "sa01";
        this.appId = "appId";
        this.deviceId = SystemInfo.deviceUniqueIdentifier;
        this.version = Application.version;
        this.keyhash = GetSignatureMD5Hash();

    }

    public static string GetSignatureMD5Hash()
    {
#if UNITY_EDITOR || UNITY_IOS
        return "";
#endif
        var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        var packageName = activity.Call<string>("getPackageName");
        var GET_SIGNATURES = new AndroidJavaClass("android.content.pm.PackageManager").GetStatic<int>("GET_SIGNATURES");
        var packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
        var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, GET_SIGNATURES);
        var signatures = packageInfo.Get<AndroidJavaObject[]>("signatures");
        if (signatures != null && signatures.Length > 0)
        {
            SByte[] bytes = signatures[0].Call<SByte[]>("toByteArray");
            string str = GetSignValidString(bytes);
            return str;
        }
        return "";
    }
    private static string GetSignValidString(SByte[] paramArrayOfByte)
    {
        var MessageDigest = new AndroidJavaClass("java.security.MessageDigest");
        var localMessageDigest = MessageDigest.CallStatic<AndroidJavaObject>("getInstance", "MD5");
        localMessageDigest.Call("update", paramArrayOfByte);
        return ToHexString(localMessageDigest.Call<SByte[]>("digest"));
    }
    public static String ToHexString(SByte[] paramArrayOfByte)
    {
        if (paramArrayOfByte == null)
        {
            return null;
        }
        StringBuilder localStringBuilder = new StringBuilder(2 * paramArrayOfByte.Length);
        for (int i = 0; ; i++)
        {
            if (i >= paramArrayOfByte.Length)
            {
                return localStringBuilder.ToString();
            }
            String str = new AndroidJavaClass("java.lang.Integer").CallStatic<String>("toString", 0xFF & paramArrayOfByte[i], 16);
            if (str.Length == 1)
            {
                str = "0" + str;
            }
            localStringBuilder.Append(str);
        }
    }

}
[System.Serializable]
public class ValidateIAPResponse
{
    public bool success = false;
}

