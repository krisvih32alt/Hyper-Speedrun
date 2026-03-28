using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    const string VolumeKey = "MasterVolume";

    void Start()
{
    float testLoad = PlayerPrefs.GetFloat("MasterVolume", 999f);
    Debug.Log($"SAVED VOLUME = {testLoad}");  // Should show your slider value (ex: 0.25), NOT 999
    // Load volume FIRST - applies to ALL scenes instantly
    float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    AudioListener.volume = volume;
    
    // THEN find slider (only if you want one)
    if (volumeSlider == null)
        volumeSlider = GetComponentInChildren<Slider>();
    
    if (volumeSlider == null)
        volumeSlider = GameObject.Find("VolumeSlider")?.GetComponent<Slider>();
    
    Debug.Log($"VolumeSlider found: {volumeSlider != null}");
    
    // Set slider AFTER finding it
    if (volumeSlider != null)
        volumeSlider.value = volume;
}



    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }


// Add to END of VolumeSlider.cs
void OnEnable()
{
    if (volumeSlider != null)
        volumeSlider.onValueChanged.AddListener(SetVolume);
}

void OnDisable()
{
    if (volumeSlider != null)
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
}

}