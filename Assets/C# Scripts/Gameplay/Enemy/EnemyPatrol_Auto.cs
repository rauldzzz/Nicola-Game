using System.Collections;
using UnityEngine;

/*
 * EnemyPatrol_Auto
 * ----------------
 * A self-contained 2D patrolling enemy.
 * - Automatically turns around when it detects a wall or a ledge
 * - Damages the player on contact with knockback
 * - Can be stomped (killed) by the player to destroy it
 * - Requires Rigidbody2D and SpriteRenderer components
 * - Was used in the 2D platformer minigame
 */
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyPatrol_Auto : MonoBehaviour, IStompable
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;                     // Horizontal patrol speed
    public float groundCheckDistance = 0.2f;         // Distance for ledge detection
    public float wallCheckDistance = 0.1f;           // Distance for wall detection
    public LayerMask groundLayer;                    // Layer mask for ground/walls

    [Header("Damage Settings")]
    public int damageAmount = 1;                     // Damage dealt to player on contact
    public float knockbackForce = 5f;                // Knockback applied to player

    [Header("Stomp Settings")]
    public float flashDuration = 0.2f;               // Flash duration when stomped
    public bool isStomped = false;                   // Prevent multiple stomps

    [Header("References")]
    public Transform groundCheck;                    // Empty child for ground/ledge detection
    public Transform wallCheck;                      // Empty child for wall detection
    public SpriteRenderer spriteRenderer;            // SpriteRenderer component

    private Rigidbody2D rb;
    private bool movingRight = true;                 // Current movement direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure spriteRenderer is assigned
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Create placeholder groundCheck if missing
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0.2f, -0.5f, 0f);
            groundCheck = gc.transform;
        }

        // Create placeholder wallCheck if missing
        if (wallCheck == null)
        {
            GameObject wc = new GameObject("WallCheck");
            wc.transform.SetParent(transform);
            wc.transform.localPosition = new Vector3(0.3f, 0f, 0f);
            wallCheck = wc.transform;
        }
    }

    void Update()
    {
        if (!isStomped) // Only patrol if not stomped
            Patrol();
    }

    /// <summary>
    /// Patrols horizontally and flips direction on ledge or wall detection
    /// </summary>
    void Patrol()
    {
        // Apply horizontal velocity
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);

        // Raycast downward to detect ledge
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // Raycast forward to detect wall
        Vector2 wallDir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, wallDir, wallCheckDistance, groundLayer);

        // Flip if no ground ahead or wall in front
        if (!groundHit.collider || wallHit.collider)
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips movement direction and mirrors sprite/check positions
    /// </summary>
    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        // Mirror check positions
        Vector3 gcLocalPos = groundCheck.localPosition;
        Vector3 wcLocalPos = wallCheck.localPosition;
        gcLocalPos.x *= -1;
        wcLocalPos.x *= -1;
        groundCheck.localPosition = gcLocalPos;
        wallCheck.localPosition = wcLocalPos;
    }

    // Called when stomped by player
    public void Stomped()
    {
        if (isStomped) return;
        isStomped = true;
        StartCoroutine(StompDeathRoutine());
    }

    // Handles the stomp death sequence
    private IEnumerator StompDeathRoutine()
    {
        rb.linearVelocity = Vector2.zero; // Stop movement

        // Disable collider to prevent further interactions
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Stop patrol logic
        this.enabled = false;

        // Flash white to indicate stomp
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        Destroy(gameObject);
    }
    
    // Draws gizmos for ground and wall detection in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }
    }
}