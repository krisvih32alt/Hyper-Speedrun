using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    [Header("UI")]
    public Slider volumeSlider;
    public TMPro.TextMeshProUGUI volumeText;
    
    private float masterVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved volume
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            SetMasterVolume(masterVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        // Convert 0-1 slider to -80 to 0 dB (logarithmic)
        float dB = Mathf.Log10(sliderValue) * 20;
        if (sliderValue == 0) dB = -80f; // Mute
        
        audioMixer.SetFloat("MasterVolume", dB);
        masterVolume = sliderValue;
        
        // Update text
        // if (volumeText != null)
        //     volumeText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
        
        // Save
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
    
    // Call this from ScaleButton
    public static float GetMasterVolume()
    {
        return instance ? instance.masterVolume : 1f;
    }
}
