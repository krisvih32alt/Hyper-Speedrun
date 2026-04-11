using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    private int myLevelIndex;

    public void Setup(int levelNum)
    {
        myLevelIndex = levelNum;
    }

    public void OnClickLoad()
    {
        // This string must match your scene names (e.g., "Level 1", "Level 2")
        string sceneToLoad = "Level " + myLevelIndex;
        
        Debug.Log("Loading: " + sceneToLoad);
        
        // Verify scene is in Build Settings before loading
        if (Application.CanStreamedLevelBeLoaded(sceneToLoad)) {
            SceneManager.LoadScene(sceneToLoad);
        } else {
            Debug.LogError("Error: Scene '" + sceneToLoad + "' is not in Build Settings!");
        }
    }
}