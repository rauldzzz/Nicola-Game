using UnityEngine;

/*
 * BusController
 * -------------
 * Handles horizontal lane switching for the bus during gameplay and
 * automatic movement during the arrival cutscene.
 * Also detects collisions with obstacles and triggers the death popup.
 */
public class BusController : MonoBehaviour
{
    // Distance from the center to each driving lane
    public float laneDistance = 1.5f;

    // Speed used for smooth lane switching
    public float switchSpeed = 10f;

    // X position the bus should currently move towards
    private float targetX;

    // Reserved for lane state tracking (currently unused)
    private bool isRightLane = false;

    // When true, player input is disabled and the bus moves automatically
    private bool isCutscene = false;

    // Reference to the UI manager that handles the death popup
    private DeathPopupManager deathPopupManager;

    void Start()
    {
        // Set the initial lane based on the starting X position
        if (transform.position.x > 0)
        {
            targetX = laneDistance;
        }
        else
        {
            targetX = -laneDistance;
        }

        // Locate the DeathPopupManager in the scene (if present)
        deathPopupManager = Object.FindFirstObjectByType<DeathPopupManager>();
    }

    void Update()
    {
        // Normal player-controlled movement
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

            // Only move on the X axis; Y is locked to the road height
            Vector3 targetPos = new Vector3(targetX, -5f, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, switchSpeed * Time.deltaTime);
        }
        // Cutscene movement (arrival at the station)
        else
        {
            // Move to the right lane and center vertically
            Vector3 stationPos = new Vector3(laneDistance, 0f, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, stationPos, 2f * Time.deltaTime);
        }
    }

    // Enables cutscene mode and disables player input
    public void StartArrivalSequence()
    {
        isCutscene = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // "Activator" represents obstacles that cause a game over
        if (other.CompareTag("Activator"))
        {
            if (deathPopupManager != null)
            {
                deathPopupManager.ShowDeathPopup();
            }
            else
            {
                // Safety fallback if the UI manager is missing
                Time.timeScale = 0;
            }
        }
    }
}