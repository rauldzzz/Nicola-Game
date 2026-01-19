using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupPanel; 
    public Button backButton;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false); 

        if (backButton != null)
            backButton.onClick.AddListener(ClosePopup);
    }

    // Show the popup and pause the game
    public void ShowPopup()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Close popup and resume the game
    public void ClosePopup()
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(false);
        Time.timeScale = 1f; 
    }
}
