using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton SaveManager to track player progress across scenes
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

        // Subscribe to scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only move player if overworld scene
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
    public void CompleteLevel(string levelName)
    {
        if (!completedLevels.Contains(levelName))
            completedLevels.Add(levelName);

        lastVisitedLevel = levelName;
    }
    #endregion

    #region Overworld Methods
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
