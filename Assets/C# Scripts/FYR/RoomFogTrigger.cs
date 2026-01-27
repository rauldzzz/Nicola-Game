using UnityEngine;
using System.Collections;

/*
 * RoomFogTrigger
 * ---------------
 * Handles "fog of war" for rooms in the dungeon.
 * - Each room starts covered by a fog sprite (fogCover)
 * - When the player enters, the fog fades out smoothly
 * - Uses a coroutine to interpolate alpha over time
 */
public class RoomFogTrigger : MonoBehaviour
{
    [Header("Fog Settings")]
    public GameObject fogCover;       // Child object representing fog overlay
    public float fadeDuration = 1f;   // Time in seconds to fade out the fog

    private bool revealed = false;    // Has the fog been revealed yet?
    private SpriteRenderer fogRenderer; // Cached renderer of the fog sprite

    private void Awake()
    {
        if (fogCover != null)
            fogRenderer = fogCover.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger once
        if (revealed) return;

        // Reveal fog when player enters
        if (other.CompareTag("Player"))
        {
            RevealFog();
        }
    }

    // Public method to reveal the fog
    public void RevealFog()
    {
        revealed = true;

        if (fogRenderer != null)
        {
            StartCoroutine(FadeOutFog());
        }
        else if (fogCover != null)
        {
            // fallback if no SpriteRenderer: just deactivate
            fogCover.SetActive(false);
        }
    }

    // Coroutine to gradually fade out the fog's alpha
    private IEnumerator FadeOutFog()
    {
        float timer = 0f;
        Color originalColor = fogRenderer.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            fogRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, t));
            timer += Time.deltaTime;
            yield return null; // wait for next frame
        }

        // Ensure fully transparent at the end
        fogRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        fogCover.SetActive(false);
    }
}