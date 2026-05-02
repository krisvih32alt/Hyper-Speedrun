using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public TextMeshProUGUI feedbackText;

    [Header("Buttons")]
    public Button registerButton;
    public Button loginButton;

    private string _playFabId;
    private bool _isNewAccount;

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

    // --- BUTTON HANDLERS ---

    public void RegisterButton()
    {
        Register(emailInput.text, passwordInput.text);
    }

    public void LoginButton()
    {
        Login(emailInput.text, passwordInput.text);
    }

    // --- REGISTER ---

    public void Register(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            feedbackText.text = "All fields are required.";
            return;
        }

        if (password.Length < 8 || password.Length > 100)
        {
            feedbackText.text = "Password must be between 8 and 100 characters.";
            return;
        }

        SetUIInteractable(false);

        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        _playFabId = result.PlayFabId;
        _isNewAccount = true;
        SetUIInteractable(true);
        SyncLocalProgressToCloud();
        SceneManager.LoadScene("ChooseDisplayName");
    }

    // --- LOGIN ---

    public void Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            feedbackText.text = "Email and password are required.";
            return;
        }

        SetUIInteractable(false);

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        _playFabId = result.PlayFabId;
        _isNewAccount = false;
        SetUIInteractable(true);
        LoadCloudProgress();
        CheckDisplayName();
    }

    private void CheckDisplayName()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), result =>
        {
            string displayName = result.PlayerProfile.DisplayName;
            if (string.IsNullOrEmpty(displayName))
                SceneManager.LoadScene("ChooseDisplayName");
            else
                SceneManager.LoadScene("Title"); 
        }, OnError);
    }

    // --- ERROR HANDLING ---

    void OnError(PlayFabError error)
    {
        SetUIInteractable(true);
        Debug.LogError(error.GenerateErrorReport());

        switch (error.Error)
        {
            // Registration
            case PlayFabErrorCode.EmailAddressNotAvailable:
                feedbackText.text = "That email is already registered."; break;
            case PlayFabErrorCode.UsernameNotAvailable:
                feedbackText.text = "That username is already taken."; break;
            case PlayFabErrorCode.InvalidEmailAddress:
                feedbackText.text = "Please enter a valid email address."; break;
            case PlayFabErrorCode.InvalidPassword:
                feedbackText.text = "Password must be 8-100 characters."; break;
            case PlayFabErrorCode.InvalidUsername:
                feedbackText.text = "Username can only contain letters and numbers."; break;

            // Login — blended intentionally to prevent account enumeration
            case PlayFabErrorCode.AccountNotFound:
            case PlayFabErrorCode.InvalidEmailOrPassword:
                feedbackText.text = "Incorrect email or password."; break;

            // Network / server
            case PlayFabErrorCode.ConnectionError:
                feedbackText.text = "Can't reach the server. Check your connection."; break;
            case PlayFabErrorCode.ServiceUnavailable:
                feedbackText.text = "Servers are temporarily down. Try again soon."; break;
            case PlayFabErrorCode.RequestViewConstraintParamsNotAllowed:
                feedbackText.text = "Too many attempts. Please wait a moment."; break;

            // Everything else — show the real error
            default:
                feedbackText.text = $"Error {error.Error}: {error.ErrorMessage}"; break;
        }
    }

    // --- UI STATE ---

    void SetUIInteractable(bool state)
    {
        if (registerButton != null) registerButton.interactable = state;
        if (loginButton != null) loginButton.interactable = state;
        if (emailInput != null) emailInput.interactable = state;
        if (passwordInput != null) passwordInput.interactable = state;
    }

    // --- DATA SYNCING ---

    private void SyncLocalProgressToCloud()
    {
        // Only sync on first registration — do not use PlayerPrefs as source
        // of truth for scores. PlayFab is the authority. This is a one-time
        // migration for players who played before signing up.
        int localHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (localHighScore <= 0) return;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "HighScore", localHighScore.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Local progress synced to cloud."),
            OnError);
    }

    private void LoadCloudProgress()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("HighScore"))
            {
                int cloudScore = int.Parse(result.Data["HighScore"].Value);
                // Cache locally for display only — never write this back to cloud
                PlayerPrefs.SetInt("HighScore", cloudScore);
                Debug.Log("Cloud progress loaded: " + cloudScore);
            }
        }, OnError);
    }

    // --- PUBLIC ACCESSORS ---

    public string GetPlayFabId() => _playFabId;
    public bool IsLoggedIn() => !string.IsNullOrEmpty(_playFabId);
}