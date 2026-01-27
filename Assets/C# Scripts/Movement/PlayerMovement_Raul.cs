using UnityEngine;
using UnityEngine.InputSystem;

/*
 * PlayerMovement
 * --------------
 * Handles player movement using the new Input System.
 * - Reads movement input and moves the Rigidbody2D.
 * - Updates Animator parameters for walking animations.
 * - This was used in the overworld scenes.
 */

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Move the player based on input and speed
        rb.linearVelocity = movementInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("IsWalking", true);

        if (context.canceled)
        {
            // Store last input direction when movement stops for idle animation
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", movementInput.x);
            animator.SetFloat("LastInputY", movementInput.y);
        }

        // Read movement input from the Input System
        movementInput = context.ReadValue<Vector2>();

        // Update Animator parameters for real-time movement animation
        animator.SetFloat("InputX", movementInput.x);
        animator.SetFloat("InputY", movementInput.y);
    }
}