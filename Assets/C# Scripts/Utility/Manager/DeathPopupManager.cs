using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * DeathPopupManager
 * -----------------
 * Handles the death popup UI and related scene/respawn logic.
 * - Shows the death popup and pauses the game when the player dies.
 * - Allows resetting the current scene, returning to the overworld, or going to the title screen.
 * - Supports optional respawn point setup.
 */

public class DeathPopupManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPopup; // Assign the DeathPopup panel

    [Header("Respawn")]
    public Transform respawnPoint;      // Set last checkpoint or level start (Optional)
    public bool autoUsePlayerPosition = true; // If true, will default to player object if respawnPoint is null

    [Header("Scene Settings")]
    public string resetSceneName = "";        // Optional: name for current scene reset
    public string overworldSceneName = "OWTest";  // Optional: overworld scene
    public string titleScreenSceneName = "StartScene"; // Optional: title screen scene

    private bool isActive = false; // Tracks whether the death popup is currently active

    void Awake()
    {
        // Hide the popup at the start
        if (deathPopup != null)
            deathPopup.SetActive(false);

        // If respawn point not assigned, use the player's position
        if (respawnPoint == null && autoUsePlayerPosition)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                respawnPoint = playerObj.transform;
            else
                Debug.LogWarning("No object with Tag 'Player'! RespawnPoint stays empty.");
        }
    }

    // Call this when the player dies to show the popup and pause the game
    public void ShowDeathPopup()
    {
        if (isActive) return;

        isActive = true;

        if (deathPopup != null)
            deathPopup.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    // Resume normal game time
    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    // Reload the current scene (or optional resetSceneName)
    public void ResetScene()
    {
        ResumeTime();

        if (string.IsNullOrEmpty(resetSceneName))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        else
        {
            SceneManager.LoadScene(resetSceneName);
        }
    }

    // Load the overworld scene
    public void LoadOverworld()
    {
        ResumeTime();

        if (!string.IsNullOrEmpty(overworldSceneName))
            SceneManager.LoadScene(overworldSceneName);
        else
            Debug.LogWarning("Overworld Scene Name is empty!");
    }

    // Load the title screen
    public void LoadTitleScreen()
    {
        ResumeTime();

        if (!string.IsNullOrEmpty(titleScreenSceneName))
            SceneManager.LoadScene(titleScreenSceneName);
        else
            Debug.LogWarning("Title Screen Scene Name is empty!");
    }
}