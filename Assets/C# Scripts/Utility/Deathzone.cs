using UnityEngine;

/*
 * DeathZone
 * ----------
 * Inflicts damage to the player and triggers respawn when the player enters the zone.
 * - Checks for PlayerHealth and applies damage if not invulnerable.
 * - Calls RespawnManager to reset the player's position to the last checkpoint.
 * - Was used in the 2D platformer minigame.
 */

public class DeathZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1; // How much damage the player takes on contact

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage if player is not invulnerable
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Respawn the player at the last checkpoint
            RespawnManager.Instance.RespawnPlayer(other.gameObject);

            Debug.Log("Player touched DeathZone: damage applied and respawn triggered");
        }
    }
}