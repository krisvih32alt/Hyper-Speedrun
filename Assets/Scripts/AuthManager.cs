using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections.Generic;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI feedbackText; // To show errors/success to the player

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterButton()
    {
        Register(emailInput.text, passwordInput.text, usernameInput.text);
    }

    public void LoginButton()
    {
        Login(emailInput.text, passwordInput.text);
    }

    public void Register(string email, string password, string username)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void Login(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        feedbackText.text = "Registered and Logged In!";
        SyncLocalProgressToCloud();
    }

    void OnLoginSuccess(LoginResult result)
    {
        feedbackText.text = "Logged in!";
        LoadCloudProgress();
    }

    void OnError(PlayFabError error)
    {
        feedbackText.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }

    // --- DATA SYNCING ---

    private void SyncLocalProgressToCloud()
    {
        // Example: Pulling a local highscore from PlayerPrefs to save to PlayFab
        int localHighScore = PlayerPrefs.GetInt("HighScore", 0);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> {
                { "HighScore", localHighScore.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, 
            result => Debug.Log("Local progress synced to cloud."), 
            OnError);
    }

    private void LoadCloudProgress()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => {
            if (result.Data != null && result.Data.ContainsKey("HighScore"))
            {
                int cloudScore = int.Parse(result.Data["HighScore"].Value);
                PlayerPrefs.SetInt("HighScore", cloudScore);
                Debug.Log("Cloud progress loaded: " + cloudScore);
            }
        }, OnError);
    }
}