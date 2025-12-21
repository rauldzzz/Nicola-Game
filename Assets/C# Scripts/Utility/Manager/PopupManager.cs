using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel; // Assign your Panel here
    public Button backButton;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false); // Hide popup initially

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToStartScene);
    }

    // Show the popup and pause the game
    public void ShowPopup()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    // Button action
    public void ReturnToStartScene()
    {
        Time.timeScale = 1f; // Resume time before scene switch
        SceneManager.LoadScene("StartScene"); // Replace with your start scene name
    }
}
