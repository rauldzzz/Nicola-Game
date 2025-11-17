using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement2DPolished_Controls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;           // Horizontal movement speed
    public float jumpForce = 14f;          // Initial jump velocity
    public float variableJumpMultiplier = 0.5f; // Reduces jump height if jump is released early

    [Header("Coyote Time Settings")]
    public float coyoteTime = 0.1f;        // Time window to still jump after leaving ground
    private float coyoteTimeCounter;       // Countdown timer for coyote jump

    [Header("Ground Check Settings")]
    public Transform groundCheck;          // Empty object placed at player's feet
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private bool isGrounded;
    private bool isJumping;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- MOVEMENT INPUT ---
        // Combine A/D with Left/Right Arrow keys manually
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveInput = 1f;

        // --- GROUND CHECK ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset coyote timer when grounded
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // --- JUMP INPUT (W or Space) ---
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }

        // --- VARIABLE JUMP HEIGHT ---
        if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space)) && rb.linearVelocity.y > 0 && isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpMultiplier);
            isJumping = false;
        }

        // --- FLIP SPRITE BASED ON DIRECTION ---
        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    void FixedUpdate()
    {
        // --- APPLY HORIZONTAL MOVEMENT ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        // Draw the ground check radius in the editor for debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
