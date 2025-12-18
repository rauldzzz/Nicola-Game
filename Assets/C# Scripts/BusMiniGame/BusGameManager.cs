using TMPro; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BusGameManager : MonoBehaviour
{
    public static BusGameManager Instance;

    [Header("Next Level Setup")]
    // 1. This is the box you drag the scene into
#if UNITY_EDITOR
    public UnityEditor.SceneAsset nextSceneFile;
#endif
    // 2. This is the hidden text variable the game actually uses
    [HideInInspector] public string nextSceneName;

    [Header("Game Settings")]
    public float gameSpeed = 10f;

    [Header("Game Settings")]
    public GameObject rockPrefab;
    public float gameDuration = 60f; 
    public float totalDistance = 100f; 

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public float laneDistance = 1.5f;

    [Header("References")]
    public BusController busController; 
    public GameObject winPanel;         
    public Slider progressSlider;
    public TextMeshProUGUI distanceText;

    private float timer;
    private float currentDistance;
    private bool gameFinished = false;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (nextSceneFile != null)
        {
            nextSceneName = nextSceneFile.name;
        }
#endif
    }

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        currentDistance = totalDistance;

        // Initialize Slider
        if (progressSlider != null)
        {
            progressSlider.maxValue = totalDistance;
            progressSlider.value = totalDistance;
        }

        // Start Spawning Rocks
        InvokeRepeating(nameof(SpawnRock), 1f, spawnInterval);
    }

    void Update()
    {
        if (gameFinished) return;

        timer += Time.deltaTime;
        float depreciationRate = totalDistance / gameDuration;
        currentDistance -= depreciationRate * Time.deltaTime;

        if (distanceText != null) distanceText.text = currentDistance.ToString("F1") + " km";
        if (progressSlider != null) progressSlider.value = currentDistance;

        if (timer >= gameDuration)
        {
            FinishGame();
        }
    }

    void SpawnRock()
    {
        if (gameFinished) return;

        // Randomly choose left (-1) or right (1)
        float spawnX = (Random.Range(0, 2) == 0) ? -laneDistance : laneDistance;

        // Spawn just above the screen (e.g., Y = 6)
        Vector3 spawnPos = new Vector3(spawnX, 6f, 0);
        Instantiate(rockPrefab, spawnPos, Quaternion.identity);
    }

    void FinishGame()
    {
        gameFinished = true;
        currentDistance = 0;

        // 1. Stop spawning NEW rocks immediately
        CancelInvoke(nameof(SpawnRock));

        // 2. Start the smooth braking coroutine instead of setting speed to 0 instantly
        StartCoroutine(BrakeToStop());

        // 3. Tell bus to park
        if (busController != null) busController.StartArrivalSequence();

        // 4. Show UI after a delay (Wait for the bus to fully stop first)
        Invoke(nameof(ShowWinUI), 3.5f); // Increased delay to 3.5s to account for braking time
    }

    // This is the new smooth braking logic
    System.Collections.IEnumerator BrakeToStop()
    {
        float startSpeed = gameSpeed;
        float duration = 3.0f; // How long it takes to stop (3 seconds)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Reduce speed over time
            gameSpeed = Mathf.Lerp(startSpeed, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        gameSpeed = 0f; // Ensure it hits exactly 0 at the end
    }
    void ShowWinUI()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    // Call this from the Button
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No next scene linked in GameManager!");
        }
    }
}
