using UnityEngine;

public class InfoPanelTrigger : MonoBehaviour
{
    [SerializeField] private TriggeredInfoPanel infoPanelScript;
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering is the Player and it hasn't fired yet
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            infoPanelScript.OpenPanel();
            hasBeenTriggered = true; 
            
            // Optional: Destroy this trigger object so it never fires again
            // Destroy(gameObject); 
        }
    }
}