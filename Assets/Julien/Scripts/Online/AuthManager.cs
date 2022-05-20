using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public string environment = "production";
    
    // Start is called before the first frame update
    async void Start()
    {
        InitializationOptions initializationOptions = new InitializationOptions().SetEnvironmentName(environment);
        await UnityServices.InitializeAsync(initializationOptions);

        SetupEvents();

        await SignInAnonymouslyAsync();
    }

    void SetupEvents()
    {
        
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        AuthenticationService.Instance.SignInFailed += OnSignInFailed;
        AuthenticationService.Instance.SignedOut += OnSignedOut;
        
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously completed !");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnSignedOut()
    {
        Debug.Log("Player signed out !");
    }

    private void OnSignInFailed(RequestFailedException obj)
    {
        Debug.LogError(obj);
    }

    private void OnSignedIn()
    {
        Debug.Log($"Player ID : {AuthenticationService.Instance.PlayerId}");
        
        Debug.Log($"Access Token : {AuthenticationService.Instance.AccessToken}");
    }
}
