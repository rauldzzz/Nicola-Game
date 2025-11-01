using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest Interactable
    public GameObject interactionIcon;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // The key to press for interaction

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        // Optional: Allow keyboard interaction without the Input System action
        if (interactableInRange != null && Input.GetKeyDown(interactionKey))
        {
            interactableInRange.Interact();
        }
    }

    public void Oninteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            // If this interactable is an NPC, end its dialogue
            if (interactable is NPC npc)
            {
                npc.EndDialogue(); // automatically ends dialogue if player leaves
            }

            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
