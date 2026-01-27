using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
 * SaveManager
 * -------------------
 * Handles saving and loading player progress across scenes.
 * - Tracks completed levels, collected keys, and overworld position.
 * - Ensures persistence via DontDestroyOnLoad.
 * - Provides utility methods for managing level completion, keys, and player positioning.
 * - Used in multiple scenes to maintain consistent player state.
 */
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // Singleton instance

    [Header("Overworld Settings")]
    public Vector3 overworldPlayerPosition; // Last saved position of player in the overworld

    [Header("Level Progress")]
    public string lastVisitedLevel = "";               // Last level the player completed
    public HashSet<string> completedLevels = new();   // Tracks completed levels

    [Header("Coins")]
    public int totalCoins = 0; // Total coins collected (currently unused in this snippet)

    [Header("Keys")]
    public HashSet<string> collectedKeys = new();     // Tracks collected keys

    [Header("Overworld Interactions")]
    public HashSet<string> interactions = new();     // Tracks interactions/events in overworld

    private void Awake()
    {
        // Singleton setup: destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to sceneLoaded to position player in overworld
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only move player if overworld scene is loaded
        if (scene.name == "OverworldScene") 
        {
            MovePlayerToSavedPosition();
        }
    }

    private void MovePlayerToSavedPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = overworldPlayerPosition;
        }
        else
        {
            Debug.LogWarning("Player not found in the overworld scene!");
        }
    }

    #region Level Methods

    // Marks a level as completed and updates last visited level.
    public void CompleteLevel(string levelName)
    {
        if (!completedLevels.Contains(levelName))
            completedLevels.Add(levelName);

        lastVisitedLevel = levelName;
    }

    #endregion

    #region Overworld Methods

    // Saves the player's overworld position, snapping to center of tiles.
    public void SaveOverworldPosition(Vector3 pos)
    {
        overworldPlayerPosition = new Vector3(
            Mathf.Floor(pos.x) + 0.5f, 
            Mathf.Floor(pos.y) + 0.5f, 
            pos.z
        );
    }

    #endregion

    #region Keys

    // Marks a key as collected.
    public void CollectKey(string keyID)
    {
        collectedKeys.Add(keyID);
    }

    // Checks if the player has collected a specific key.
    public bool HasKey(string keyID)
    {
        return collectedKeys.Contains(keyID);
    }

    #endregion
}
