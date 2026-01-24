using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;

public class RhythmGameManager : MonoBehaviour
{
    [Header("Audio & BeatScroller")]
    public AudioSource theMusic;
    public BeatScroller theBS;
    public bool startPlaying;

    [Header("Score")]
    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 150;
    public int scorePerPerfectNote = 200;
    public TextMeshProUGUI scoreText; // main score display

    [Header("End Game UI")]
    public Image fadeImage;          // full-screen black image
    public float fadeDuration = 1.5f;
    public GameObject endPopup;      // popup panel at the end
    public TextMeshProUGUI finalScoreText; // text on the popup showing final score

    [Header("Instructions Panel")]
    public StartInfoPanel startInfoPanel; // your start info panel

    private bool gameEnding = false;

    public static RhythmGameManager instance;

    void Start()
    {
        instance = this;

        if (scoreText != null)
            scoreText.text = "Score: 0";

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (endPopup != null)
            endPopup.SetActive(false);
    }

    void Update()
    {
        // Start the game only when the start panel is closed
        if (!startPlaying)
        {
            if (startInfoPanel == null || !startInfoPanel.isActiveAndEnabled)
            {
                StartGame();
            }
        }

        // End game check
        if (startPlaying && !theMusic.isPlaying && !gameEnding)
        {
            gameEnding = true;
            StartCoroutine(EndGameSequence());
        }
    }

    private void StartGame()
    {
        startPlaying = true;
        theMusic.Play();
        if (theBS != null) theBS.Begin();
    }

    #region Score Methods
    void NoteHit()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    public void NormalHit()
    {
        currentScore += scorePerNote;
        NoteHit();
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodNote;
        NoteHit();
    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote;
        NoteHit();
    }

    public void NoteMissed()
    {
        // Optional: handle misses
    }
    #endregion

    #region End Game
    IEnumerator EndGameSequence()
    {
        // Fade to black
        if (fadeImage != null)
        {
            float t = 0f;
            Color c = fadeImage.color;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeImage.color = c;
        }

        // Set final score in the popup
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + currentScore;

        // Show popup
        if (endPopup != null)
            endPopup.SetActive(true);
    }

    // Call this from your popup button to close the game
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}
