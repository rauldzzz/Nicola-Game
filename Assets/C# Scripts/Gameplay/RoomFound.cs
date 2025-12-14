using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomFound : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject winPopupPrefab;

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

        // Freeze the game
        Time.timeScale = 0f;

        // Show popup UI
        if (winPopupPrefab != null)
        {
            GameObject popup = Instantiate(winPopupPrefab);
            // Find the button and assign the callback
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
        SceneManager.LoadScene("OWTest"); // replace with your scene name
    }
}
