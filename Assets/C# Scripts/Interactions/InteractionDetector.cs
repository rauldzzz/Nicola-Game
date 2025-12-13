using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest interactable
    public GameObject interactionIcon;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // Key to press for interaction

    private GridMovementHold playerMovement;

    void Start()
    {
        // Find the player and get its movement script
        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovementHold>();
        
        if (interactionIcon != null)
            interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (interactableInRange != null)
        {
            // Show or hide the interaction icon depending on player movement
            if (interactionIcon != null)
                interactionIcon.SetActive(!playerMovement.isMoving);

            // Only allow interaction if the player is not moving
            if ((playerMovement == null || !playerMovement.isMoving) && Input.GetKeyDown(interactionKey))
            {
                interactableInRange.Interact();
            }
        }
    }

    // Optional Input System method
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && interactableInRange != null && (playerMovement == null || !playerMovement.isMoving))
        {
            interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect interactables in range
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;

            if (interactionIcon != null)
                interactionIcon.SetActive(!playerMovement.isMoving);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Clear interactable reference when leaving
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            // End dialogue if leaving an NPC
            if (interactable is NPC npc)
                npc.EndDialogue();

            interactableInRange = null;

            if (interactionIcon != null)
                interactionIcon.SetActive(false);
        }
    }
}
