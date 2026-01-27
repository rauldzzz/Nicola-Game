using UnityEngine;

/*
 * InfoPanelTrigger
 * ----------------
 * Triggers a UI info panel when the player enters a collider.
 * - Can be set to trigger only once.
 * - Optionally destroy the trigger after activation.
 */

public class InfoPanelTrigger : MonoBehaviour
{
    [SerializeField] private TriggeredInfoPanel infoPanelScript; // Reference to the panel script
    private bool hasBeenTriggered = false; // Ensure it triggers only once

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger when the player enters and it hasn't already fired
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            infoPanelScript.OpenPanel(); // Open the info panel
            hasBeenTriggered = true;

            // Optional: destroy trigger so it never activates again
            // Destroy(gameObject);
        }
    }
}