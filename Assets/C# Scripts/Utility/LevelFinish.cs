using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelName; // Optional: unique level identifier
    public List<CompletionCondition> conditions; // Completion conditions

    [Header("UI")]
    public TMP_Text conditionText;

    [Header("Scene Settings")]
    public string overworldSceneName; // Name of your overworld scene

    private void Start()
    {
        // Optionally auto-set levelName to the scene name
        if (string.IsNullOrEmpty(levelName))
            levelName = SceneManager.GetActiveScene().name;

        UpdateConditionUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (CheckAllConditions())
        {
            Debug.Log($"Level {levelName} completed!");
            SaveManager.Instance.CompleteLevel(levelName);

            // Optional: store coins, etc. in SaveManager if needed
            SaveManager.Instance.totalCoins = CoinManager.Instance.TotalCoins;

            // Load the overworld
            SceneManager.LoadScene(overworldSceneName);
        }
        else
        {
            Debug.Log("Level not completed yet! Complete all conditions.");
        }
    }

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
