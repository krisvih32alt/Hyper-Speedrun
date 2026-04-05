using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScaleButton : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scene Transition")]
    public string sceneToLoad = "SceneToLoad";
    [Header("Scale")]
    public float hoverScale = 1.08f;
    public float clickScale = 0.95f;
    
    [Header("Colors")] 
    public Color hoverColor = Color.white;
    public Color clickColor = new Color(0.8f, 0.8f, 0.9f);
    
    [Header("Smoothness")]
    public float animSpeed = 12f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float volumeMultiplier = 1f;
    
    [Header("Gradient Glow")]
    public Image glowImage;
    public Color glowColorInner = new Color(0f, 1f, 1f, 0f); // Cyan
    public Color glowColorOuter = new Color(0.5f, 0.8f, 1f, 0f); // Blue

    private Vector3 originalScale;
    private Color originalColor;
    private Graphic image;
    private Vector3 targetScale;
    private Color targetColor;
    private float targetGlowAlpha;
    private bool isClickAnimating;
    private Coroutine clickRoutine;

    void Start()
    {
        image = GetComponent<Graphic>();
        if (image == null)
        {
            enabled = false;
            return;
        }
        
        originalScale = transform.localScale;
        originalColor = image.color;
        
        targetScale = originalScale;
        targetColor = originalColor;
        targetGlowAlpha = 0f;
        
        // Setup gradient glow
        if (glowImage != null)
        {
            glowImage.color = glowColorOuter;
            glowImage.raycastTarget = false;
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animSpeed);
        image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * animSpeed);
        
        if (glowImage != null)
        {
            Color glowCol = glowImage.color;
            glowCol.a = Mathf.Lerp(glowCol.a, targetGlowAlpha, Time.deltaTime * animSpeed);
            glowImage.color = glowCol;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isClickAnimating) return;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
        targetGlowAlpha = 0.4f;
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isClickAnimating) return;
        targetScale = originalScale;
        targetColor = originalColor;
        targetGlowAlpha = 0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickRoutine != null) return;
        clickRoutine = StartCoroutine(ClickBounce());
        PlaySound(clickSound);
        StartCoroutine(LoadSceneAfterBounce());

    }

    IEnumerator LoadSceneAfterBounce()
    {
        yield return new WaitForSeconds(0.2f);  // Wait for bounce to finish
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator ClickBounce()
    {
        isClickAnimating = true;

        targetScale = originalScale * clickScale;
        targetColor = clickColor;
        targetGlowAlpha = 0f;
        yield return new WaitForSeconds(0.08f);
        
        targetScale = originalScale * 1.02f;
        targetColor = hoverColor;
        yield return new WaitForSeconds(0.04f);
        
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
        targetGlowAlpha = 0.4f;

        yield return new WaitForSeconds(0.06f);
        isClickAnimating = false;
        clickRoutine = null;
    }

    void PlaySound(AudioClip clip)
{
    if (clip == null || audioSource == null) return;
    
    // Use global volume from AudioManager
float volume = AudioListener.volume * volumeMultiplier * 0.8f;
GetComponent<AudioSource>()?.PlayOneShot(clip);
}


    void OnDisable()
    {
        if (clickRoutine != null)
        {
            StopCoroutine(clickRoutine);
            clickRoutine = null;
        }
    }
}
