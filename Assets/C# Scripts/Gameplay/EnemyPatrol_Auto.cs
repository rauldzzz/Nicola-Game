using System.Collections;
using UnityEngine;

/// <summary>
/// A self-contained 2D patrolling enemy that automatically turns around when it detects
/// a wall or ledge. It damages the player on contact and can be stomped.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyPatrol_Auto : MonoBehaviour, IStompable
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;                     // Horizontal patrol speed
    public float groundCheckDistance = 0.2f;         // How far ahead to check for ground (ledge detection)
    public float wallCheckDistance = 0.1f;           // How far ahead to check for walls
    public LayerMask groundLayer;                    // Layer mask for ground/walls

    [Header("Damage Settings")]
    public int damageAmount = 1;                     // How much damage to deal to player
    public float knockbackForce = 5f;                // Knockback applied to player when touched

    [Header("Stomp Settings")]
    public float flashDuration = 0.2f;              // Flash duration when stomped
    public bool isStomped = false;                  // Flag to prevent multiple stomps

    [Header("References")]
    public Transform groundCheck;                    // Empty child at the enemy’s feet
    public Transform wallCheck;                      // Empty child in front of the enemy
    public SpriteRenderer spriteRenderer;           // SpriteRenderer reference

    private Rigidbody2D rb;
    private bool movingRight = true;                 // Direction flag

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure spriteRenderer is assigned
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Safety check: create placeholders if not assigned
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0.2f, -0.5f, 0f);
            groundCheck = gc.transform;
        }

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
    /// Moves the enemy and checks for ledges or walls to turn around automatically.
    /// </summary>
    void Patrol()
    {
        // Apply horizontal velocity
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);

        // Raycast downward to check for ground (ledge detection)
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // Raycast forward to check for wall
        Vector2 wallDir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, wallDir, wallCheckDistance, groundLayer);

        // Flip if no ground ahead or wall in front
        if (!groundHit.collider || wallHit.collider)
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips direction and sprite horizontally.
    /// </summary>
    void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        // Also mirror check positions
        Vector3 gcLocalPos = groundCheck.localPosition;
        Vector3 wcLocalPos = wallCheck.localPosition;

        gcLocalPos.x *= -1;
        wcLocalPos.x *= -1;

        groundCheck.localPosition = gcLocalPos;
        wallCheck.localPosition = wcLocalPos;
    }

    // Called by PlayerDamageHandler when stomped
    public void Stomped()
    {
        if (isStomped) return;
        isStomped = true;
        StartCoroutine(StompDeathRoutine());
    }

    private IEnumerator StompDeathRoutine()
    {
        // Stop movement immediately
        rb.velocity = Vector2.zero;

        // Disable collider so player doesn't hit it again
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Optionally disable patrol script to stop movement
        this.enabled = false;

        // Flash white
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        Destroy(gameObject);
    }
    
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
