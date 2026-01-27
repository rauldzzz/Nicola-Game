using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;

/*
 * RhythmGameManager
 * -----------------
 * Manages core gameplay for the rhythm game.
 * - Starts and stops the music and beat scroller.
 * - Tracks score and updates UI.
 * - Handles end-of-song sequence with fade and popup.
 * - Interfaces with a start info panel to delay gameplay until ready.
 */

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
    public TextMeshProUGUI scoreText;

    [Header("End Game UI")]
    public Image fadeImage;           // Full-screen black for fade effect
    public float fadeDuration = 1.5f;
    public GameObject endPopup;       // Panel to show final score
    public TextMeshProUGUI finalScoreText;

    [Header("Instructions Panel")]
    public StartInfoPanel startInfoPanel;

    private bool gameEnding = false;

    public static RhythmGameManager instance;

    void Start()
    {
        instance = this;

        if (scoreText != null)
            scoreText.text = "Score: 0";

        // Ensure fade starts fully transparent
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
        // Wait until start panel is closed before starting gameplay
        if (!startPlaying)
        {
            if (startInfoPanel == null || !startInfoPanel.isActiveAndEnabled)
                StartGame();
        }

        // Check if music has finished to trigger end sequence
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
        // Could implement penalty or feedback here
    }
    #endregion

    #region End Game
    IEnumerator EndGameSequence()
    {
        // Fade screen to black over time
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

        // Update final score on popup
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + currentScore;

        if (endPopup != null)
            endPopup.SetActive(true);
    }

    // Called by end popup button to quit
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion
}