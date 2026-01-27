using UnityEngine;

/*
 * EnemyDamage_Trigger
 * -------------------
 * Handles damaging the player when they touch the enemy.
 * - Works on both OnTriggerEnter2D and OnTriggerStay2D
 * - Only damages if the player is not currently invulnerable
 * - Does not apply knockback or physical interaction
 */
public class EnemyDamage_Trigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;               
    public float invulnerabilityDuration = 1f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // Checks if the collider is the player and deals damage if possible
    private void TryDamagePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                // Deal damage
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
