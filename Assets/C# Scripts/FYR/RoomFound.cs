using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomFound : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject winPopupPrefab;

    [Header("Overworld Settings")]
    [Tooltip("Optional: set this to override the overworld spawn position")]
    public Vector3 overworldSpawnPosition;

    [Header("Level Settings")]
    public string levelName = "FYRMinigame"; // unique identifier for this minigame

    private bool hasWon = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return;
        if (other.CompareTag("Player"))
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        hasWon = true;

        // Mark the level as completed
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelName);

            // Directly set the overworld position
            SaveManager.Instance.SaveOverworldPosition(overworldSpawnPosition);
        }

        // Freeze the game
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

    private void ReturnToOverworld()
    {
        Time.timeScale = 1f; // unfreeze
        SceneManager.LoadScene("OverworldScene");
    }
}
