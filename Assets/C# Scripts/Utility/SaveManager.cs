using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton SaveManager to track player progress across scenes
/// Handles overworld position, level completion, keys, and interactions.
/// Coins and level-specific elements reset each time a level is loaded.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Overworld Settings")]
    public Vector3 overworldPlayerPosition; // Last saved overworld position

    [Header("Level Progress")]
    public string lastVisitedLevel = ""; // Name of the last visited level
    public HashSet<string> completedLevels = new HashSet<string>(); // Tracks completed levels

    [Header("Coins")]
    public int totalCoins = 0; // Total coins across all levels

    [Header("Keys")]
    public HashSet<string> collectedKeys = new HashSet<string>(); // Tracks collected keys across levels

    [Header("Overworld Interactions")]
    public HashSet<string> interactions = new HashSet<string>(); // Example: doors unlocked, NPCs talked to

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Level Methods

    /// <summary>
    /// Mark a level as completed
    /// </summary>
    public void CompleteLevel(string levelName)
    {
        if (!completedLevels.Contains(levelName))
            completedLevels.Add(levelName);

        lastVisitedLevel = levelName;
    }

    #endregion

    #region Overworld Methods

    /// <summary>
    /// Save the player's overworld position, aligned to grid (nearest 0.5)
    /// </summary>
    public void SaveOverworldPosition(Vector3 pos, float lockDuration = 0.5f)
    {
        // Round to nearest 0.5 to align with grid
        overworldPlayerPosition = new Vector3(
            Mathf.Floor(pos.x) + 0.5f,
            Mathf.Floor(pos.y) + 0.5f,
            pos.z
        );


        // Find player and start movement lock coroutine
        GridMovementHold_Commented playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovementHold_Commented>();
        if (playerMovement != null)
        {
            playerMovement.StartCoroutine(LockMovementCoroutine(playerMovement, lockDuration));
        }
        else
        {
            Debug.LogWarning("SaveManager: Could not find Player GameObject to lock movement.");
        }
    }

    /// <summary>
    /// Coroutine to temporarily disable movement
    /// </summary>
    private IEnumerator LockMovementCoroutine(GridMovementHold_Commented movement, float duration)
    {
        movement.enabled = false;
        yield return new WaitForSeconds(duration);
        movement.enabled = true;
    }


    #endregion

    #region Keys

    /// <summary>
    /// Collect a key and make it persist across levels
    /// </summary>
    public void CollectKey(string keyID)
    {
        collectedKeys.Add(keyID);
    }

    /// <summary>
    /// Check if a key has already been collected
    /// </summary>
    public bool HasKey(string keyID)
    {
        return collectedKeys.Contains(keyID);
    }

    #endregion
}
