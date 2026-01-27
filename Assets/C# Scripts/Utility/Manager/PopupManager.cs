using UnityEngine;
using UnityEngine.UI;

/*
 * PopupManager
 * ------------
 * Handles showing and hiding a generic popup panel in the UI.
 * - Pauses the game when the popup is active.
 * - Can assign a button to close the popup automatically.
 */

public class PopupManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel; // The popup panel to show/hide
    public Button backButton;     // Optional button to close the popup

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false); // Hide popup on start

        if (backButton != null)
            backButton.onClick.AddListener(ClosePopup); // Link back button
    }

    // Show the popup and pause the game
    public void ShowPopup()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    // Close popup and resume the game
    public void ClosePopup()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}