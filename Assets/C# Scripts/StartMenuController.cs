using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Set the target scene in the Inspector
    public void OnStartClick()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
