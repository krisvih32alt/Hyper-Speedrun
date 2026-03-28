using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    void OnGUI()
    {
        // Runs at Unity's GUI rate (much lighter than Update)
        GUI.Label(new Rect(Screen.width - 200, 10, 190, 30), 
                  "FPS: " + Mathf.RoundToInt(1f / Time.unscaledDeltaTime));
    }
}
