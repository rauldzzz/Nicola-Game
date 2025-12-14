using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest interactable
    public GameObject interactionIcon;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // The key to press for interaction

    private PlayerMovement playerMovement;
    private Animator playerAnimator;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<Animator>();
        interactionIcon.SetActive(false);
    }

    void Update()
    {
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

    }


    public void Oninteract(InputAction.CallbackContext context)
    {
        bool isMoving = playerAnimator != null && playerAnimator.GetBool("IsWalking");
        if (context.performed && interactableInRange != null && !isMoving)
        {
            interactableInRange.Interact();
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
            if (interactable is NPC npc)
                npc.EndDialogue();

            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
