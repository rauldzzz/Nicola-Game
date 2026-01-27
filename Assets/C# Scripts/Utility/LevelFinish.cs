using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/*
 * LevelFinish
 * -----------
 * Handles level completion when the player reaches the end zone.
 * - Checks all assigned completion conditions.
 * - Updates the UI to show progress on conditions.
 * - Saves progress and coins using SaveManager.
 * - Loads the overworld scene upon successful completion.
 * - Was used in the 2D platformer minigame.
 */

public class LevelFinish : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelName;                    // Optional identifier for this level
    public List<CompletionCondition> conditions; // List of conditions to complete the level

    [Header("UI")]
    public TMP_Text conditionText;              // UI element showing condition progress

    [Header("Scene Settings")]
    public string overworldSceneName;           // Name of the scene to load after completion

    private void Start()
    {
        // Use the scene name as default levelName if none is set
        if (string.IsNullOrEmpty(levelName))
            levelName = SceneManager.GetActiveScene().name;

        UpdateConditionUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Only proceed if all conditions are fulfilled
        if (CheckAllConditions())
        {
            Debug.Log($"Level {levelName} completed!");

            // Mark level as completed in the SaveManager
            SaveManager.Instance.CompleteLevel(levelName);

            // Save coin progress
            SaveManager.Instance.totalCoins = CoinManager.Instance.TotalCoins;

            // Load the overworld scene
            SceneManager.LoadScene(overworldSceneName);
        }
        else
        {
            Debug.Log("Level not completed yet! Complete all conditions.");
        }
    }

    // Returns true if all assigned completion conditions are fulfilled
    public bool CheckAllConditions()
    {
        if (conditions == null || conditions.Count == 0)
            return true;

        foreach (var cond in conditions)
        {
            if (cond == null) continue;
            if (!cond.IsFulfilled())
                return false;
        }

        return true;
    }

    // Updates the UI text with the current status of each condition
    private void UpdateConditionUI()
    {
        if (conditionText == null) return;

        string text = "";
        foreach (var condition in conditions)
        {
            if (condition != null)
                text += $"{condition.GetStatusText()}\n";
        }
        conditionText.text = text;
    }
}