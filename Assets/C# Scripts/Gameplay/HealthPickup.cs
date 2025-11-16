using System.Collections;  
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 1;      // How much health to restore
    public AudioClip collectSound;
    public float popDuration = 0.2f; 
    public float popScale = 1.5f;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                collected = true;

                playerHealth.currentHealth += healAmount;
                if (playerHealth.currentHealth > playerHealth.maxHealth)
                    playerHealth.currentHealth = playerHealth.maxHealth;

                // Play sound
                if (collectSound != null)
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);

                // Pop visual feedback
                StartCoroutine(PopAndDestroy());
            }
        }
    }

    private IEnumerator PopAndDestroy()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * popScale;
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popDuration);
            yield return null;
        }

        // Optional fade out
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            float fadeTime = 0.15f;
            elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, elapsed / fadeTime));
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
