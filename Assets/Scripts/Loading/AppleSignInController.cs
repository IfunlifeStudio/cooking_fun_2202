using System;
using System.Text;
using System.Threading.Tasks;
using AppleAuth.Interfaces;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using System.Security.Cryptography;
using System.Collections.Generic;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Native;
using AppleAuth.Extensions;

public class AppleSignInController : MonoBehaviour
{
    private const string AppleUserIdKey = "AppleUserId";
    private IAppleAuthManager _appleAuthManager;
    private void Start()
    {        
        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);
        }
        DontDestroyOnLoad(gameObject);
    }
    public FirebaseAuth GetAthInstanceApple()
    {
        return  FirebaseAuth.DefaultInstance;
    }
    private void Update()
    {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this._appleAuthManager != null)
        {
            this._appleAuthManager.Update();
        }
    }
    //generate rawNonce to send to firebase
    private static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }
    //Generate the SHA256 hash of the rawNonce
    private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }
        return result;
    }
    public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
    {
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce);
        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PlayerPrefs.SetString(AppleUserIdKey, credential.User);
                    this.PerformFirebaseAuthentication(appleIdCredential, rawNonce,firebaseAuthCallback);
                }
            },
            error =>
            {
                DatabaseController.Instance.DespawnLoadingOverlay();
                DataController.Instance.LoadData();
            });
    }
    private void PerformFirebaseAuthentication(IAppleIDCredential appleIdCredential, string rawNonce, Action<FirebaseUser> firebaseAuthCallback)
    {
        DatabaseController.Instance.SpawnLoading();
        var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
        var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
        var firebaseCredential = OAuthProvider.GetCredential("apple.com", identityToken, rawNonce, authorizationCode);
        GetAthInstanceApple().SignInWithCredentialAsync(firebaseCredential)
            .ContinueWithOnMainThread(task => HandleSignInWithUser(task, firebaseAuthCallback));
    }
    public void AttemptQuickLogin(Action<FirebaseUser> firebaseAuthCallback)
    {
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);
        var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce);

        // Quick login should succeed if the credential was authorized before and not revoked
        this._appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                // If it's an Apple credential, save the user ID, for later logins
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
                }
                else
                {
                    DatabaseController.Instance.DespawnLoadingOverlay();
                    DataController.Instance.LoadData();
                }
            },
            error =>
            {
                // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                DatabaseController.Instance.DespawnLoadingOverlay();
                DataController.Instance.LoadData();
            });
    }
    private static void HandleSignInWithUser(Task<FirebaseUser> task,Action<FirebaseUser> firebaseUserCallback)
    {
        if (task.IsCanceled)
        {
            Debug.Log("Firebase auth was canceled");
            DatabaseController.Instance.DespawnLoadingOverlay();
            DataController.Instance.LoadData();
        }
        else if (task.IsFaulted)
        {
            Debug.Log("Firebase auth was failed");
            DatabaseController.Instance.DespawnLoadingOverlay();
            DataController.Instance.LoadData();
        }
        else
        {
            var firebaseUser = task.Result;
            firebaseUserCallback(firebaseUser);
        }
    }
}
