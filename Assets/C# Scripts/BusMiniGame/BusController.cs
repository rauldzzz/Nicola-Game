using UnityEngine;

public class BusController : MonoBehaviour
{
    public float laneDistance = 1.5f; 
    public float switchSpeed = 10f;

    private float targetX;
    private bool isRightLane = false; 

    private DeathPopupManager deathPopupManager;

    void Start()
    {
        // Set initial lane
        targetX = -laneDistance;

        // Find the DeathPopupManager in the scene
        deathPopupManager = Object.FindFirstObjectByType<DeathPopupManager>();
        if (deathPopupManager == null)
        {
            Debug.LogWarning("DeathPopupManager not found in the scene!");
        }
    }

    void Update()
    {
        // Switch lanes left/right
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            isRightLane = true;
            targetX = laneDistance;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            isRightLane = false;
            targetX = -laneDistance;
        }

        // Smooth transition between lanes
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, switchSpeed * Time.deltaTime);
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
