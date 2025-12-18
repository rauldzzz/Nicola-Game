using UnityEngine;

public class StartInfoPanel : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode closeKey = KeyCode.E;

    private bool isOpen = true;

    void Start()
    {
        // Show panel
        gameObject.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
        isOpen = true;
    }

    void Update()
    {
        if (!isOpen) return;

        // Close panel when E is pressed
        if (Input.GetKeyDown(closeKey))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        isOpen = false;

        // Resume the game
        Time.timeScale = 1f;

        // Hide panel
        gameObject.SetActive(false);
    }
}
