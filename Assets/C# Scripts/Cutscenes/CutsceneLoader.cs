using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/*
 * CutsceneLoader
 * ---------------
 * Plays a VideoPlayer cutscene and automatically transitions to a specified scene when the video ends.
 * - Assign a VideoPlayer component in the inspector
 * - Specify the scene to load after the cutscene finishes
 */
public class CutsceneLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer; // VideoPlayer component that plays the cutscene
    public string sceneToLoad;      // Scene name to load after the video finishes

    void Start()
    {
        // Subscribe to the VideoPlayer's loopPointReached event to detect when the video ends
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // Called automatically when the video finishes playing
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneToLoad); // Load the next scene
    }
}