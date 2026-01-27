using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * StartInfoPanel
 * --------------
 * Displays an info panel at the start of a scene.
 * - Pauses the game while the panel is open.
 * - Can be configured to only show once per scene using PlayerPrefs.
 * - Can be closed with a key press.
 */

public class StartInfoPanel : MonoBehaviour
{
    public KeyCode closeKey = KeyCode.E; // Key to close the panel
    public bool showOnlyOnce = true;     // Show only the first time the scene is played

    private string prefKey; // PlayerPrefs key for tracking if panel was shown
    private bool isOpen;    // Whether the panel is currently open

    void Awake()
    {
        // Generate a unique key for this scene
        prefKey = "StartInfoPanel_" + SceneManager.GetActiveScene().name;

        // Ensure the panel is active at start
        gameObject.SetActive(true);
    }

    void Start()
    {
        StartCoroutine(InitPanel());
    }

    private System.Collections.IEnumerator InitPanel()
    {
        yield return null; // Wait a frame to ensure UI is fully initialized

        // Hide panel if it should only show once and has already been seen
        if (showOnlyOnce && PlayerPrefs.GetInt(prefKey, 0) == 1)
        {
            gameObject.SetActive(false);
            yield break;
        }

        // Ensure panel renders above everything
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        isOpen = true;
        Time.timeScale = 0f; // Pause game while panel is open
    }

    void Update()
    {
        if (!isOpen) return;

        // Close panel on key press
        if (Input.GetKeyDown(closeKey))
            ClosePanel();
    }

    // Close the panel and optionally save that it has been seen
    public void ClosePanel()
    {
        if (!isOpen) return;

        isOpen = false;
        Time.timeScale = 1f; // Resume game

        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(prefKey, 1);
            PlayerPrefs.Save();
        }

        gameObject.SetActive(false);
    }
}
