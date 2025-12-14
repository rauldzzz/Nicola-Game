using UnityEngine;

public class EnemyDamage_Trigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;               // How much damage to deal
    public float invulnerabilityDuration = 1f; // How long the player is invulnerable after getting hit

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                // Deal damage
                playerHealth.TakeDamage(damageAmount);
                // Keine physische Interaktion, also kein Knockback
            }
        }
    }
}
