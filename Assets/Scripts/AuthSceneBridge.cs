using UnityEngine;
using TMPro;

public class AuthSceneBridge : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text feedbackText;

    void Start()
    {
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.InitializeUI(emailInput, passwordInput, feedbackText);
        }
    }
}