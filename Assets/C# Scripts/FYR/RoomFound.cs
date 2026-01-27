using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * RoomFound
 * ----------
 * Handles the win condition for the minigame.
 * - When the player enters the trigger, the level is marked as complete
 * - Optional overworld position is saved
 * - Game is paused and a win popup UI is displayed
 * - Player can return to the overworld from the popup
 */
public class RoomFound : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject winPopupPrefab; // Prefab for the win UI popup

    [Header("Overworld Settings")]
    [Tooltip("Optional: set this to override the overworld spawn position")]
    public Vector3 overworldSpawnPosition; // Position where player should appear in overworld

    [Header("Level Settings")]
    public string levelName = "FYRMinigame"; // Unique identifier for this minigame

    private bool hasWon = false; // Ensures win is triggered only once
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger once
        if (hasWon) return;

        // Check if player entered
        if (other.CompareTag("Player"))
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        hasWon = true;

        // Save level completion and overworld position
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelName);
            SaveManager.Instance.SaveOverworldPosition(overworldSpawnPosition);
        }

        // Pause the game
        Time.timeScale = 0f;

        // Show popup UI
        if (winPopupPrefab != null)
        {
            GameObject popup = Instantiate(winPopupPrefab);
            Button btn = popup.GetComponentInChildren<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => ReturnToOverworld());
        }
        else
        {
            Debug.LogWarning("Win popup prefab not assigned!");
        }
    }

    // Called from the popup button to return to the overworld
    private void ReturnToOverworld()
    {
        Time.timeScale = 1f; // unpause
        SceneManager.LoadScene("OverworldScene");
    }
}