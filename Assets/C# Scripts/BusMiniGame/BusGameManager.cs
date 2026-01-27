using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/*
 * BusGameManager
 * --------------
 * Controls the overall flow of the bus minigame:
 * - Spawning obstacles
 * - Tracking progress over time (distance)
 * - Handling win condition, braking, and transition back to the overworld
 */
public class BusGameManager : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static BusGameManager Instance;

    [Header("Next Level Setup")]
#if UNITY_EDITOR
    // Scene reference used only in the editor for convenience
    public UnityEditor.SceneAsset nextSceneFile;
#endif
    // Scene name used at runtime
    [HideInInspector] public string nextSceneName;

    [Header("Game Settings")]
    [Tooltip("Current gameSpeed")]
    public float gameSpeed = 10f;

    [Header("Game Settings")]
    [Tooltip("Prefab for the rock obstacles and game duration/distance")]
    public GameObject rockPrefab;
    public float gameDuration = 60f;
    public float totalDistance = 100f;

    [Header("Spawning")]
    [Tooltip("Controls how often obstacles appear and lane spacing")]
    public float spawnInterval = 1.5f;
    public float laneDistance = 1.5f;

    [Header("References")]
    [Tooltip("References to player, UI, and progress display")]
    public BusController busController;
    public GameObject winPanel;
    public Slider progressSlider;
    public TextMeshProUGUI distanceText;

    [Header("Overworld Settings")]
    [Tooltip("Optional: set this to override the overworld spawn position")]
    public Vector3 overworldSpawnPosition;

    // Internal state tracking
    private float timer;
    private float currentDistance;
    private bool gameFinished = false;

    // Keeps track of spawned obstacles for cleanup
    private List<GameObject> spawnedRocks = new List<GameObject>();

    void OnValidate()
    {
#if UNITY_EDITOR
        // Automatically sync scene name from SceneAsset
        if (nextSceneFile != null)
        {
            nextSceneName = nextSceneFile.name;
        }
#endif
    }

    void Awake()
    {
        // Set singleton instance
        Instance = this;
    }

    void Start()
    {
        // Hide win UI at the start
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        currentDistance = totalDistance;

        // Initialize progress slider to full distance
        if (progressSlider != null)
        {
            progressSlider.maxValue = totalDistance;
            progressSlider.value = totalDistance;
        }

        // Start spawning obstacles repeatedly
        InvokeRepeating(nameof(SpawnRock), 1f, spawnInterval);
    }

    void Update()
    {
        if (gameFinished) return;

        // Advance game timer
        timer += Time.deltaTime;

        // Calculate how fast distance decreases over time
        float depreciationRate = totalDistance / gameDuration;
        currentDistance -= depreciationRate * Time.deltaTime;

        // Update UI elements
        if (distanceText != null)
            distanceText.text = currentDistance.ToString("F1") + " km";

        if (progressSlider != null)
            progressSlider.value = currentDistance;

        // End the game when the timer runs out
        if (timer >= gameDuration)
        {
            FinishGame();
        }
    }

    void SpawnRock()
    {
        if (gameFinished) return;

        // Randomly choose left or right lane
        float spawnX = (Random.Range(0, 2) == 0) ? -laneDistance : laneDistance;

        // Spawn obstacle slightly above the visible screen
        Vector3 spawnPos = new Vector3(spawnX, 6f, 0);
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        spawnedRocks.Add(rock);
    }

    void FinishGame()
    {
        gameFinished = true;
        currentDistance = 0;

        // Stop spawning new obstacles
        CancelInvoke(nameof(SpawnRock));

        // Remove all existing obstacles
        DespawnAllRocks();

        // Gradually slow the game instead of stopping instantly
        StartCoroutine(BrakeToStop());

        // Trigger bus arrival cutscene
        if (busController != null)
            busController.StartArrivalSequence();

        // Show win UI after braking finishes
        Invoke(nameof(ShowWinUI), 3.5f);
    }

    void DespawnAllRocks()
    {
        foreach (GameObject rock in spawnedRocks)
        {
            if (rock != null)
                Destroy(rock);
        }

        spawnedRocks.Clear();
    }

    // Smoothly reduces game speed to zero over a fixed duration
    IEnumerator BrakeToStop()
    {
        float startSpeed = gameSpeed;
        float duration = 3.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            gameSpeed = Mathf.Lerp(startSpeed, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameSpeed = 0f;
    }

    void ShowWinUI()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    // Called by UI button to return to the overworld
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (SaveManager.Instance != null)
            {
                // Store overworld spawn position before switching scenes
                SaveManager.Instance.SaveOverworldPosition(overworldSpawnPosition);
            }

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No next scene linked in GameManager!");
        }
    }
}