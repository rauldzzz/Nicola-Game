using UnityEngine;

/*
 * EnemyDamage
 * ------------
 * Handles damaging the player on physical collision.
 * - Deals damage and applies knockback when colliding with the player
 * - Only triggers if the player is not invulnerable
 * - Uses Rigidbody2D physics to apply the knockback force
 * - Was used in the 2D platformer minigame
 * - Stationary hazards like spikes can use this script
 */
public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;               // Amount of damage to deal
    public float knockbackForce = 5f;          // Force applied to push the player away
    public float invulnerabilityDuration = 1f; // Duration the player is invulnerable after taking damage (managed by PlayerHealth)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only trigger on collision with player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get PlayerHealth component
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // Check for null and invulnerability
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                // Deal damage
                playerHealth.TakeDamage(damageAmount);

                // Apply knockback using Rigidbody2D
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // Calculate direction from enemy to player
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    
                    // Reset any existing velocity before applying force
                    playerRb.linearVelocity = Vector2.zero;
                    
                    // Apply impulse force
                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
