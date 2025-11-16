using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;            // How much damage to deal
    public float knockbackForce = 5f;       // Knockback force applied to the player
    public float invulnerabilityDuration = 1f; // How long the player is invulnerable after getting hit

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the PlayerHealth component
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                // Deal damage
                playerHealth.TakeDamage(damageAmount);

                // Apply knockback
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
