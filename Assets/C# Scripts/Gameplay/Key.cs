using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Key : MonoBehaviour
{
    [Header("Key Settings")]
    public string keyID = "";               // Unique identifier for this key
    public AudioClip collectSound;          // Sound when collected

    [Header("Animation Settings")]
    public float popDuration = 0.3f;        // Scale up time
    public float popScale = 1.7f;           // Scale multiplier
    public float bounceHeight = 0.5f;       // Upwards bounce in world units
    public float bounceDuration = 0.3f;     // Duration of the bounce

    private bool collected = false;

    private void Start()
    {
        // Generate a unique keyID if none set
        if (string.IsNullOrEmpty(keyID))
            keyID = System.Guid.NewGuid().ToString();

        // Destroy if already collected
        if (SaveManager.Instance.collectedKeys.Contains(keyID))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            // Add to SaveManager
            SaveManager.Instance.collectedKeys.Add(keyID);

            // Play sound
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Animate and destroy
            StartCoroutine(CollectAnimation());
        }
    }

    private IEnumerator CollectAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * popScale;
        Vector3 originalPos = transform.position;
        Vector3 targetPos = originalPos + Vector3.up * bounceHeight;

        float elapsed = 0f;

        // Pop and bounce
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;

            // Scale
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // Vertical bounce (using sine for smooth curve)
            transform.position = Vector3.Lerp(originalPos, targetPos, Mathf.Sin(t * Mathf.PI));

            yield return null;
        }

        // Optional: small sparkle fade out
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            elapsed = 0f;
            float fadeTime = 0.2f;
            Color startColor = sr.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, elapsed / fadeTime));
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
