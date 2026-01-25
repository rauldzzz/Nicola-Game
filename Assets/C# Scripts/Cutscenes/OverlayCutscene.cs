using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OverlayCutscene : MonoBehaviour
{
    public GameObject cutsceneCanvas;
    public VideoPlayer videoPlayer;
    public string nextSceneName;

    // NEW: Add a slot for the background music
    public AudioSource backgroundMusic;

    void Start()
    {
        videoPlayer.timeReference = VideoTimeReference.Freerun;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void StartCutscene()
    {
        cutsceneCanvas.SetActive(true);
        Time.timeScale = 0f;

        // NEW: Pause the background music if it exists
        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }

        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Time.timeScale = 1f;

        // NEW: Resume the music
        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }

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