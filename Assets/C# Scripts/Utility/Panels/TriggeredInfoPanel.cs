using UnityEngine;

/*
 * TriggeredInfoPanel
 * ------------------
 * Handles a UI panel that appears when triggered in-game.
 * - Opens the panel and temporarily disables player movement.
 * - Closes the panel and restores player movement.
 * - Can be closed by pressing the E key.
 */

public class TriggeredInfoPanel : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerScript; // Reference to player movement script
    private Rigidbody2D playerRb;                         // Reference to player's Rigidbody2D

    private void Awake()
    {
        if (playerScript != null)
            playerRb = playerScript.GetComponent<Rigidbody2D>();

        // Ensure the panel is hidden at the start
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        // Close the panel when E is pressed
        if (this.gameObject.activeSelf && Input.GetKeyDown(KeyCode.E))
            ClosePanel();
    }

    // Show the panel and disable player movement
    public void OpenPanel()
    {
        this.gameObject.SetActive(true);

        if (playerScript != null)
        {
            // Stop the player immediately
            if (playerRb != null)
                playerRb.linearVelocity = Vector2.zero;

            // Disable player input/movement
            playerScript.enabled = false;
        }
    }

    // Hide the panel and re-enable player movement
    public void ClosePanel()
    {
        if (playerScript != null)
            playerScript.enabled = true;

        this.gameObject.SetActive(false);
    }
}