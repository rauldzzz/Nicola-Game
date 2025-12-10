using UnityEngine;

public class BusController : MonoBehaviour
{
    public float laneDistance = 1.5f; 
    public float switchSpeed = 10f;

    private float targetX;
    private bool isRightLane = false;

    private bool isCutscene = false;

    private DeathPopupManager deathPopupManager;

    void Start()
    {
        if (transform.position.x > 0)
        {
            targetX = laneDistance; // Start in Right Lane
        }
        else
        {
            targetX = -laneDistance; // Start in Left Lane
        }

        // Find the DeathPopupManager in the scene
        deathPopupManager = Object.FindFirstObjectByType<DeathPopupManager>();
        if (deathPopupManager == null)
        {
            Debug.LogWarning("DeathPopupManager not found in the scene!");
        }
    }

    void Update()
    {
        if (!isCutscene)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                targetX = laneDistance;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                targetX = -laneDistance;
            }

            // Normal movement (X axis only, Y stays at -3)
            Vector3 targetPos = new Vector3(targetX, -5f, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, switchSpeed * Time.deltaTime);
        }
        // 2. Cutscene Movement (Auto-drive to station)
        else
        {
            // Target: Right Lane (laneDistance) and Middle of Screen (Y = 0)
            Vector3 stationPos = new Vector3(laneDistance, 0f, transform.position.z);

            // Move smoothly towards station (slower speed looks nicer for parking)
            transform.position = Vector3.Lerp(transform.position, stationPos, 2f * Time.deltaTime);
        }
    }

    // Call this to trigger the ending
    public void StartArrivalSequence()
    {
        isCutscene = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check for collision with "Activator" (rock, obstacle, etc.)
        if (other.CompareTag("Activator"))
        {
            Debug.Log("Hit a rock! Game Over.");

            // Show the death popup if the manager exists
            if (deathPopupManager != null)
            {
                deathPopupManager.ShowDeathPopup();
            }
            else
            {
                // Fallback: freeze game if popup manager is missing
                Time.timeScale = 0;
            }
        }
    }
}
