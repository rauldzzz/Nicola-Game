using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPopupManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPopup; // Assign the DeathPopup panel

    [Header("Respawn")]
    public Transform respawnPoint;      // Set last checkpoint or level start

    private bool isActive = false;

    void Awake()
    {
        // Hide the popup at the start
        if (deathPopup != null)
            deathPopup.SetActive(false);
    }

    /// <summary>
    /// Call this when player dies
    /// </summary>
    public void ShowDeathPopup()
    {
        if (isActive) return;

        isActive = true;

        if (deathPopup != null)
            deathPopup.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Resume normal time
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public void ResetScene()
    {
        ResumeTime();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Load the overworld scene
    /// </summary>
    public void LoadOverworld()
    {
        ResumeTime();
        SceneManager.LoadScene("OverworldScene"); // Replace with your overworld scene name
    }

    /// <summary>
    /// Load the title screen
    /// </summary>
    public void LoadTitleScreen()
    {
        ResumeTime();
        SceneManager.LoadScene("StartScene"); // Replace with your title scene name
    }
}
