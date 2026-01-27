using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/*
 * OverlayCutscene
 * ----------------
 * Handles playing a cutscene as an overlay on top of the current scene.
 * - Pauses the game while the cutscene is playing
 * - Optionally pauses/resumes background music
 * - Can automatically load a next scene after the cutscene finishes
 */
public class OverlayCutscene : MonoBehaviour
{
    public GameObject cutsceneCanvas;   // Canvas containing the cutscene UI
    public VideoPlayer videoPlayer;     // VideoPlayer component for the cutscene
    public string nextSceneName;        // Optional scene to load after cutscene ends
    public AudioSource backgroundMusic; // Optional background music to pause during cutscene

    void Start()
    {
        // Use freerun mode so the video plays independent of Time.timeScale (paused game)
        videoPlayer.timeReference = VideoTimeReference.Freerun;

        // Subscribe to event for when the video finishes
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void StartCutscene()
    {
        cutsceneCanvas.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        // Pause the music if assigned
        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }

        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Time.timeScale = 1f; // Resume the game

        // Resume the music if it was paused
        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }

        // Either load the next scene or hide the cutscene canvas
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            cutsceneCanvas.SetActive(false);
        }
    }
}