using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 1;      // How much health to restore
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth += healAmount;
                if (playerHealth.currentHealth > playerHealth.maxHealth)
                    playerHealth.currentHealth = playerHealth.maxHealth;

                // Play sound
                if (collectSound != null)
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
