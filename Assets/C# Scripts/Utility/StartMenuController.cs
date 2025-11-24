using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when pressing Start (optional)")]
    public string sceneToLoad;

    [Header("UI Elements")]
    public CanvasGroup uiGroup;
    public GameObject buttonHighlight;
    public TMP_Text transitionText;

    [Header("Animation Settings")]
    public float fadeDuration = 1f;
    public float textMoveDuration = 1.5f;
    public Vector3 textTargetPosition = Vector3.zero;
    public Vector3 textTargetScale = new Vector3(1.5f, 1.5f, 1f);
    public float holdDuration = 1f;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private bool isTransitioning = false;

    void Awake()
    {   
        if (uiGroup == null)
            Debug.LogWarning("UIGroup not assigned! Please assign it in the Inspector.");
    }

    public void OnStartClick()
    {
        if (!isTransitioning)
            StartCoroutine(StartGameSequence());
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private IEnumerator StartGameSequence()
    {
        isTransitioning = true;

        // Disable all buttons
        foreach (Button button in GetComponentsInChildren<Button>())
            button.interactable = false;

        if (buttonHighlight != null)
            buttonHighlight.SetActive(false);

        // Fade out UI
        if (uiGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(uiGroup, 1f, 0f, fadeDuration));

        // Fade audio
        if (backgroundMusic != null)
            yield return StartCoroutine(FadeAudio(backgroundMusic, fadeDuration));

        // Animate transition text
        if (transitionText != null)
        {
            transitionText.alpha = 0f;
            RectTransform textRect = transitionText.GetComponent<RectTransform>();
            Vector3 startPos = textRect.localPosition;
            Vector3 startScale = textRect.localScale;

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

            yield return new WaitForSeconds(holdDuration);

            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                transitionText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        // Load scene by name or next build index
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextSceneIndex);
            else
                Debug.LogWarning("No next scene in build settings!");
        }
    }

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
        source.volume = startVolume;
    }
}
