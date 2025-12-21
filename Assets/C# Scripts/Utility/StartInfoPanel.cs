using UnityEngine;
using UnityEngine.SceneManagement;

public class StartInfoPanel : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode closeKey = KeyCode.E;

    [Tooltip("If enabled, the panel will only show once PER SCENE")]
    public bool showOnlyOnce = true;

    private string prefKey;
    private bool isOpen;

    void Awake()
    {
        prefKey = "StartInfoPanelShown_" + SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        if (showOnlyOnce && PlayerPrefs.GetInt(prefKey, 0) == 1)
        {
            gameObject.SetActive(false);
            isOpen = false;
            return;
        }

        gameObject.SetActive(true);
        isOpen = true;

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

        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(prefKey, 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
