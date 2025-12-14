using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest interactable
    public GameObject interactionIcon;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // Key to press for interaction

<<<<<<< HEAD
    private PlayerMovement playerMovement;
    private Animator playerAnimator;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<Animator>();
        interactionIcon.SetActive(false);
=======
    private GridMovementHold playerMovement;

    void Start()
    {
        // Find the player and get its movement script
        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovementHold>();
        
        if (interactionIcon != null)
            interactionIcon.SetActive(false);
>>>>>>> 9c93b760a21d309631025e916e46978f992ea22a
    }

    void Update()
    {
<<<<<<< HEAD
        // Only allow interaction if player is in range and NOT moving
        bool isMoving = playerAnimator != null && playerAnimator.GetBool("IsWalking");
        if (interactableInRange != null && !isMoving)
        {
            if (Input.GetKeyDown(interactionKey))
                interactableInRange.Interact();
        }
        if (interactableInRange != null && interactionIcon != null)
        {
            interactionIcon.SetActive(!isMoving);
        }
=======
        if (interactableInRange != null)
        {
            // Show or hide the interaction icon depending on player movement
            if (interactionIcon != null)
                interactionIcon.SetActive(!playerMovement.isMoving);
>>>>>>> 9c93b760a21d309631025e916e46978f992ea22a

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
<<<<<<< HEAD
        bool isMoving = playerAnimator != null && playerAnimator.GetBool("IsWalking");
        if (context.performed && interactableInRange != null && !isMoving)
=======
        if (context.performed && interactableInRange != null && (playerMovement == null || !playerMovement.isMoving))
>>>>>>> 9c93b760a21d309631025e916e46978f992ea22a
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
