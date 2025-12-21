using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("UI Elements")]
    public CanvasGroup uiGroup;
    public GameObject buttonHighlight;
    public TMP_Text transitionText;

    [Header("Black Screen")]
    public CanvasGroup blackScreenGroup;
    public float blackScreenHoldTime = 3f;

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
        if (blackScreenGroup != null)
        {
            blackScreenGroup.alpha = 1f;
            blackScreenGroup.blocksRaycasts = true;
        }

        if (backgroundMusic != null)
            backgroundMusic.Stop();
    }

    void Start()
    {
        StartCoroutine(StartupSequence());
    }

    private IEnumerator StartupSequence()
    {
        // Hold black screen
        yield return new WaitForSeconds(blackScreenHoldTime);

        // Fade out black screen
        if (blackScreenGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(blackScreenGroup, 1f, 0f, fadeDuration));

        blackScreenGroup.blocksRaycasts = false;

        // Start music AFTER reveal
        if (backgroundMusic != null)
            backgroundMusic.Play();
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

        foreach (Button button in GetComponentsInChildren<Button>())
            button.interactable = false;

        if (buttonHighlight != null)
            buttonHighlight.SetActive(false);

        if (uiGroup != null)
            yield return StartCoroutine(FadeCanvasGroup(uiGroup, 1f, 0f, fadeDuration));

        if (backgroundMusic != null)
            yield return StartCoroutine(FadeAudio(backgroundMusic, fadeDuration));

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

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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