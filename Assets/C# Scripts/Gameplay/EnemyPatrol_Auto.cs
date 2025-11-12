using UnityEngine;

/// <summary>
/// A self-contained 2D patrolling enemy that automatically turns around when it detects
/// a wall or ledge. It damages the player on contact.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyPatrol_Auto : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;                     // Horizontal patrol speed
    public float groundCheckDistance = 0.2f;         // How far ahead to check for ground (ledge detection)
    public float wallCheckDistance = 0.1f;           // How far ahead to check for walls
    public LayerMask groundLayer;                    // Layer mask for ground/walls

    [Header("Damage Settings")]
    public int damageAmount = 1;                     // How much damage to deal to player
    public float knockbackForce = 5f;                // Knockback applied to player when touched

    [Header("References")]
    public Transform groundCheck;                    // Empty child at the enemy’s feet
    public Transform wallCheck;                      // Empty child in front of the enemy

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool movingRight = true;                 // Direction flag

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    /// <summary>
    /// Damages player on collision.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                playerHealth.TakeDamage(damageAmount);

                // Optional knockback
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
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
