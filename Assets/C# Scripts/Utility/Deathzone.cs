using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1; // Damage dealt when touching the death zone

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal 1 damage if the player has a PlayerHealth component
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Respawn the player at the last checkpoint
            RespawnManager.Instance.RespawnPlayer(other.gameObject);

            Debug.Log("Player touched DeathZone: 1 damage + respawn");
        }
    }
}
