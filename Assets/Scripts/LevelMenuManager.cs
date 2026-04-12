using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelCardPrefab; 
    public RectTransform contentParent; 

    [Header("Settings")]
    public int numberOfLevels = 20;    
    public string levelPrefix = "LEVEL ";
    public float spacing = 15f;

    void Start()
    {
        SpawnMenu();
    }

    public void SpawnMenu()
    {
        // 1. CLEAR
        foreach (Transform child in contentParent) {
            Destroy(child.gameObject);
        }

        // 2. SPAWN
        for (int i = 1; i <= numberOfLevels; i++)
        {
            GameObject newCard = Instantiate(levelCardPrefab, contentParent);
            
            // This ensures the card scale doesn't go wonky when spawning
            newCard.transform.localScale = Vector3.one; 

            TextMeshProUGUI label = newCard.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = levelPrefix + i;

            LevelButton buttonLogic = newCard.GetComponent<LevelButton>();
            if (buttonLogic != null) buttonLogic.Setup(i);
        }

        // 3. THE FORCE FIX
        UpdateContentHeight();
    }

    void UpdateContentHeight()
    {
        RectTransform prefabRect = levelCardPrefab.GetComponent<RectTransform>();
        float cardHeight = prefabRect.rect.height;

        float totalHeight = (200 * numberOfLevels) + (25 * (numberOfLevels - 1)) + 75;

        // This is the most "aggressive" way to set height in Unity
        contentParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        
        // Reset position to top so it doesn't start scrolled halfway
        contentParent.anchoredPosition = new Vector2(contentParent.anchoredPosition.x, 0);
    }
}