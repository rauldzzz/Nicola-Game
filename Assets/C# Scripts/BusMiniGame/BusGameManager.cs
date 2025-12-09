using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
public class BusGameManager : MonoBehaviour
{
    public static BusGameManager Instance;

    [Header("Game Settings")]
    public float gameSpeed = 10f;

    [Header("Game Settings")]
    public GameObject rockPrefab;
    public float gameDuration = 60f; 
    public float totalDistance = 100f; 

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public float laneDistance = 1.5f; 

    [Header("UI References")]
    public Slider progressSlider;
    public TextMeshProUGUI distanceText; 

    private float timer;
    private float currentDistance;
    private bool gameFinished = false;

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
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

        // 1. Update Timer
        timer += Time.deltaTime;

        // 2. Calculate Distance
        // We calculate how much distance to remove per frame to ensure it hits 0 at exactly 60 seconds
        float depreciationRate = totalDistance / gameDuration;
        currentDistance -= depreciationRate * Time.deltaTime;

        // 3. Update UI
        if (distanceText != null)
            distanceText.text = currentDistance.ToString("F1") + " km";

        if (progressSlider != null)
            progressSlider.value = currentDistance;

        // 4. Check Win Condition
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
        CancelInvoke(nameof(SpawnRock)); 
        Debug.Log("Level Complete!");

    }
}