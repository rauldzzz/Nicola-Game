using UnityEngine;

/*
 * TriggerCutscene
 * ----------------
 * Simple trigger to start an overlay cutscene when the player enters a collider.
 * - Calls StartCutscene() on the assigned OverlayCutscene
 * - Deactivates itself after triggering so it only happens once
 */
public class TriggerCutscene : MonoBehaviour
{
    public OverlayCutscene cutsceneController; // Reference to the cutscene handler

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger if the player enters
        if (other.CompareTag("Player"))
        {
            cutsceneController.StartCutscene(); 
            gameObject.SetActive(false);        // Disable trigger so it doesn't repeat
        }
    }
}