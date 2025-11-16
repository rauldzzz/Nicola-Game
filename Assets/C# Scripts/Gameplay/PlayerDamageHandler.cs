using UnityEngine;

[RequireComponent(typeof(PlayerHealth), typeof(Rigidbody2D))]
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public string enemyTag = "Enemy";
    public string hazardTag = "Hazard";

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float bounceForce = 8f; // How high the player bounces when stomping

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
            // Use contact normals to detect stomping (for forgiving capsule hits)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // Player hitting from above
                {
                    isStomping = true;
                    break;
                }
            }
        }

        if (isStomping)
        {
            // Call enemy's Stomped logic if it exists
            var stompable = collision.gameObject.GetComponent<IStompable>();
            if (stompable != null)
                stompable.Stomped();

            // Bounce the player
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

            return; // Exit early so player does not take damage
        }
        else
        {
            // Player takes damage
            if (!playerHealth.IsInvulnerable)
            {
                playerHealth.TakeDamage(1);

                // Apply knockback if it's an enemy
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
