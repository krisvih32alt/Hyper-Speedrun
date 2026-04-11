using UnityEngine;
using UnityEngine.UI;
using TMPro; // Crucial for TextMeshPro support

public class LevelMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelCardPrefab; // The "Template" from your Project folder
    public Transform contentParent;    // The "Content" object inside your ScrollView

    [Header("Settings")]
    public int numberOfLevels = 20;    // Change this to spawn more/less
    public string levelPrefix = "LEVEL ";

    void Start()
    {
        // This clears old cards and spawns new ones when the game starts
        SpawnMenu();
    }

    public void SpawnMenu()
    {
        // 1. CLEAR: Deletes any placeholder cards you left in the editor
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // 2. SPAWN: The loop that creates your levels
        for (int i = 1; i <= numberOfLevels; i++)
        {
            // Create the clone
            GameObject newCard = Instantiate(levelCardPrefab, contentParent);
            
            // Name it in the hierarchy (e.g., Level_01)
            newCard.name = "Level_" + i.ToString("D2");

            // Update the TextMeshPro text inside the prefab
            TextMeshProUGUI label = newCard.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) {
                label.text = levelPrefix + i;
            }

            // Tell the button which level it is (Links to Script B)
            LevelButton buttonLogic = newCard.GetComponent<LevelButton>();
            if (buttonLogic != null) {
                buttonLogic.Setup(i);
            }
        }
    }
}