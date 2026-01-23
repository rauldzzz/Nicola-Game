using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OverlayCutscene : MonoBehaviour
{
    public GameObject cutsceneCanvas; 
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.timeReference = VideoTimeReference.Freerun;

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void StartCutscene()
    {
        cutsceneCanvas.SetActive(true);

        Time.timeScale = 0f;

        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Time.timeScale = 1f;

            cutsceneCanvas.SetActive(false);
    }
}