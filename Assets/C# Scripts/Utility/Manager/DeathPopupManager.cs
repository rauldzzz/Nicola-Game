using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPopupManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathPopup; // Assign the DeathPopup panel

    [Header("Respawn")]
    public Transform respawnPoint;      // Set last checkpoint or level start (Optional)
    [Tooltip("If empty: use player position (Tag = 'Player').")]
    public bool autoUsePlayerPosition = true;

    [Header("Scene Settings")]
    [Tooltip("For ResetScene (current scene)")]
    public string resetSceneName = "";

    [Tooltip("LoadOverworld")]
    public string overworldSceneName = "OWTest";

    [Tooltip("LoadTitleScreen")]
    public string titleScreenSceneName = "StartScene";
    private bool isActive = false;

    void Awake()
    {   // Hide the popup at the start
        if (deathPopup != null)
            deathPopup.SetActive(false);

        // RespawnPoint set to player if left empty
        if (respawnPoint == null && autoUsePlayerPosition)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                respawnPoint = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("No object with Tag 'Player'! RespawnPoint stays empty.");
            }
        }
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

    /// <summary>
    /// Load the overworld scene
    /// </summary>
    public void LoadOverworld()
    {
        ResumeTime();
        if (!string.IsNullOrEmpty(overworldSceneName))
            SceneManager.LoadScene(overworldSceneName);
        else
            Debug.LogWarning("Overworld Scene Name is empty!");
    }

    /// <summary>
    /// Load the title screen
    /// </summary>
    public void LoadTitleScreen()
    {
        ResumeTime();
        if (!string.IsNullOrEmpty(titleScreenSceneName))
            SceneManager.LoadScene(titleScreenSceneName);
        else
            Debug.LogWarning("Title Screen Scene Name is empty!");
    }
}
