using UnityEngine;
using UnityEngine.SceneManagement;

public class StartInfoPanel : MonoBehaviour
{
    public KeyCode closeKey = KeyCode.E;
    public bool showOnlyOnce = true;

    private string prefKey;
    private bool isOpen;

    void Awake()
    {
        prefKey = "StartInfoPanel_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameObject.SetActive(true); // Ensure active
    }

    void Start()
    {
        StartCoroutine(InitPanel());
    }

    private System.Collections.IEnumerator InitPanel()
    {
        yield return null; // Wait one frame so UI fully initializes

        if (showOnlyOnce && PlayerPrefs.GetInt(prefKey, 0) == 1)
        {
            gameObject.SetActive(false);
            yield break;
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

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
        Time.timeScale = 1f;

        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(prefKey, 1);
            PlayerPrefs.Save();
        }

        gameObject.SetActive(false);
    }
}