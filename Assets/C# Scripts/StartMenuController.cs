using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when pressing Start")]
    public string sceneToLoad;

    [Header("UI Elements")]
    [Tooltip("UIGroup that holds the UI elements that are supposed to dissapear (like Buttons ect.)")]
    public CanvasGroup uiGroup;

    [Tooltip("Optional custom highlight GameObject for buttons")]
    public GameObject buttonHighlight;

    [Tooltip("Transition text that appears when starting (e.g., 'Loading...' or game name)")]
    public TMP_Text transitionText;

    [Tooltip("Duration of fade for buttons and text")]
    public float fadeDuration = 1f;

    [Tooltip("How long the text takes to move and scale into position")]
    public float textMoveDuration = 1.5f;

    [Tooltip("Target position for the text to move to (usually center of screen)")]
    public Vector3 textTargetPosition = Vector3.zero;

    [Tooltip("How much the text scales up during the animation")]
    public Vector3 textTargetScale = new Vector3(1.5f, 1.5f, 1f);

    [Tooltip("How long the text stays on screen before fading out")]
    public float holdDuration = 1f;

    [Header("Audio")]
    [Tooltip("Optional background music to fade out when starting")]
    public AudioSource backgroundMusic;

    private CanvasGroup canvasGroup;    // CanvasGroup of the entire menu (for fading out)
    private bool isTransitioning = false; // Prevent multiple clicks

    void Awake()
    {
        if (uiGroup == null)
        {
            Debug.LogWarning("UIGroup not assigned! Please assign it in the Inspector.");
        }
    }

    /// <summary>
    /// Called when the Start button is clicked
    /// </summary>
    public void OnStartClick()
    {
        if (!isTransitioning)
            StartCoroutine(StartGameSequence());
    }

    /// <summary>
    /// Called when the Exit button is clicked
    /// </summary>
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    /// <summary>
    /// Handles the full start transition (fade out, text animation, scene load)
    /// </summary>
    private IEnumerator StartGameSequence()
    {
        isTransitioning = true;

        // Disable all buttons immediately so player can't click anything
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }

        // Disable custom highlight if assigned
        if (buttonHighlight != null)
        {
            buttonHighlight.SetActive(false);
        }

        // Fade out only the UI group (buttons + title, background stays visible)
        yield return StartCoroutine(FadeCanvasGroup(uiGroup, 1f, 0f, fadeDuration));


        // Fade out background music if assigned
        if (backgroundMusic != null)
        {
            yield return StartCoroutine(FadeAudio(backgroundMusic, fadeDuration));
        }

        // Animate transition text (if assigned)
        if (transitionText != null)
        {
            // Prepare text at start position and invisible
            transitionText.alpha = 0f;
            RectTransform textRect = transitionText.GetComponent<RectTransform>();
            Vector3 startPos = textRect.localPosition;
            Vector3 startScale = textRect.localScale;

            // Animate text: fade in, move, and scale up
            float elapsed = 0f;
            while (elapsed < textMoveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / textMoveDuration;

                transitionText.alpha = Mathf.Lerp(0f, 1f, t);
                textRect.localPosition = Vector3.Lerp(startPos, textTargetPosition, t);
                textRect.localScale = Vector3.Lerp(startScale, textTargetScale, t);

                yield return null;
            }

            // Hold the text for a bit before fading out
            yield return new WaitForSeconds(holdDuration);

            // Fade text out
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                transitionText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        // Finally, load the next scene after everything fades away
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Smoothly fades the CanvasGroup between two alpha values
    /// </summary>
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
    }

    /// <summary>
    /// Smoothly fades out an AudioSource over time
    /// </summary>
    private IEnumerator FadeAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset volume for reuse
    }
}
