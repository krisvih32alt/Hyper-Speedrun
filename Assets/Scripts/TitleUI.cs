using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button loginButton;
    public Button logoutButton;

    void Start()
    {
        bool loggedIn = AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn();
        loginButton.gameObject.SetActive(!loggedIn);
        logoutButton.gameObject.SetActive(loggedIn);
    }
}