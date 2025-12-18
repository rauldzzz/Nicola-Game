using UnityEngine;

public class StartInfoPanel : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode closeKey = KeyCode.E;

    [Tooltip("If enabled, the panel will only show once")]
    public bool showOnlyOnce = true;

    // Unique key for this panel
    private const string PREF_KEY = "StartInfoPanelShown";

    private bool isOpen;

    void Start()
    {
        // Check if panel was already shown
        if (showOnlyOnce && PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
        {
            gameObject.SetActive(false);
            isOpen = false;
            return;
        }

        // Show panel
        gameObject.SetActive(true);
        isOpen = true;

        // Pause game
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(closeKey))
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        if (!isOpen) return;

        isOpen = false;

        // Save that we've shown it
        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(PREF_KEY, 1);
            PlayerPrefs.Save();
        }

        // Resume game
        Time.timeScale = 1f;

        // Hide panel
        gameObject.SetActive(false);
    }
}