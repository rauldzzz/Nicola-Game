using UnityEngine;

/*
 * PlayerDamageHandler
 * -------------------
 * Handles player interactions with enemies and hazards.
 * - Detects collisions with enemies or hazards.
 * - Determines if player is stomping an enemy or taking damage.
 * - If stomping, triggers enemy's Stomped logic and bounces the player.
 * - If hit from the side or below, applies damage and knockback.
 * - Requires PlayerHealth and Rigidbody2D components.
 */

[RequireComponent(typeof(PlayerHealth), typeof(Rigidbody2D))]
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public string enemyTag = "Enemy";
    public string hazardTag = "Hazard";

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;      // Knockback applied when hit by enemy
    public float bounceForce = 8f;         // Vertical bounce when stomping an enemy

    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react to enemies or hazards
        if (!collision.gameObject.CompareTag(enemyTag) && !collision.gameObject.CompareTag(hazardTag))
            return;

        bool isStomping = false;

        if (collision.gameObject.CompareTag(enemyTag))
        {
            // Detect if player is hitting enemy from above
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isStomping = true;
                    break;
                }
            }
        }

        if (isStomping)
        {
            // Trigger enemy stomp logic if available
            var stompable = collision.gameObject.GetComponent<IStompable>();
            if (stompable != null)
                stompable.Stomped();

            // Bounce the player upward
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

            return; // Exit early to prevent damage
        }
        else
        {
            // Take damage if not stomping
            if (!playerHealth.IsInvulnerable)
            {
                playerHealth.TakeDamage(1);

                // Apply knockback if enemy
                if (collision.gameObject.CompareTag(enemyTag))
                {
                    Vector2 knockDir = (transform.position - collision.transform.position).normalized;
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}