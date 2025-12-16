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
    public string lastVisitedLevel = "";
    public HashSet<string> completedLevels = new HashSet<string>();

    [Header("Coins")]
    public int totalCoins = 0;

    [Header("Keys")]
    public HashSet<string> collectedKeys = new HashSet<string>();

    [Header("Overworld Interactions")]
    public HashSet<string> interactions = new HashSet<string>();

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

    public void CompleteLevel(string levelName)
    {
        if (!completedLevels.Contains(levelName))
            completedLevels.Add(levelName);

        lastVisitedLevel = levelName;
    }

    #endregion

    #region Overworld Methods

    /// <summary>
    /// Save the player's overworld position (aligned to grid if needed)
    /// </summary>
    public void SaveOverworldPosition(Vector3 pos)
    {
        // If you DON'T want grid snapping anymore, just assign pos directly
        overworldPlayerPosition = new Vector3(
            Mathf.Floor(pos.x) + 0.5f,
            Mathf.Floor(pos.y) + 0.5f,
            pos.z
        );
    }

    #endregion

    #region Keys

    public void CollectKey(string keyID)
    {
        collectedKeys.Add(keyID);
    }

    public bool HasKey(string keyID)
    {
        return collectedKeys.Contains(keyID);
    }

    #endregion
}
