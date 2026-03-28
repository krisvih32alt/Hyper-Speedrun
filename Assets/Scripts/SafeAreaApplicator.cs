using UnityEngine;

public class SafeAreaManager : MonoBehaviour
{
    RectTransform Panel;

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        // The origin of safe area is at the bottom-left of the screen

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // Convert pixel coordinates to a 0-1 range for anchors
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
    }
}
