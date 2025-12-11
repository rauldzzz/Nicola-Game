using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int coinValue = 1;              // How much this coin adds to the counter
    public AudioClip collectSound;         // Optional sound
    public float popDuration = 0.2f;       // Pop animation duration
    public float popScale = 1.5f;          // Pop scale multiplier

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            // Increase coin count
            CoinManager.Instance.AddCoins(coinValue);

            // Play coin sound
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Play pop animation and destroy
            StartCoroutine(PopAndDestroy());
        }
    }

    private IEnumerator PopAndDestroy()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * popScale;
        float elapsed = 0f;

        // Scale up quickly
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popDuration);
            yield return null;
        }

        // Fade out if using SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            float fadeTime = 0.15f;
            elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                                     Mathf.Lerp(1f, 0f, elapsed / fadeTime));
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
