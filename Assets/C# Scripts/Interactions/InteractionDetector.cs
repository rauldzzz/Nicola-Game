using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest interactable
    public GameObject interactionIcon;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // Key to press for interaction

    void Start()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (interactableInRange != null)
        {
            // Show interaction icon
            if (interactionIcon != null)
                interactionIcon.SetActive(true);

            // Interact on key press
            if (Input.GetKeyDown(interactionKey))
            {
                interactableInRange.Interact();
            }
        }
    }

    // Optional Input System method
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && interactableInRange != null)
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
                interactionIcon.SetActive(true);
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
