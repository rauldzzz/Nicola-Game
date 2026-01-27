using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * RespawnManager
 * --------------
 * Handles player respawning at the last checkpoint.
 * - Fades the screen to black, teleports the player, then fades back in.
 * - Temporarily disables movement and physics during respawn.
 * - Singleton for easy global access.
 * - Was used in the 2D platformer minigame.
 */

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance; // Singleton reference

    [Header("Respawn Settings")]
    public Image fadeImage;       // Black overlay used for fade effect
    public float fadeDuration = 0.5f; // Time to fade in/out
    public float holdDuration = 0.3f; // How long the screen stays black

    private Transform currentCheckpoint; // Last checkpoint reached
    private bool isRespawning = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Set the current checkpoint for respawning
    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    // Start the respawn process for the player
    public void RespawnPlayer(GameObject player)
    {
        if (!isRespawning && currentCheckpoint != null)
            StartCoroutine(RespawnRoutine(player));
    }

    // Handles fading, teleporting, and restoring player movement
    private IEnumerator RespawnRoutine(GameObject player)
    {
        isRespawning = true;

        // Disable movement and physics temporarily
        var rb = player.GetComponent<Rigidbody2D>();
        var movement = player.GetComponent<PlayerMovement2DPolished_Controls>();

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f; // Prevent falling during fade
        }

        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Move player to checkpoint
        player.transform.position = currentCheckpoint.position;

        // Hold screen black
        yield return new WaitForSeconds(holdDuration);

        // Fade back in
        yield return StartCoroutine(Fade(0f));

        // Re-enable physics and movement
        if (rb != null)
            rb.gravityScale = 2f; // Restore normal gravity
        if (movement != null)
            movement.enabled = true;

        isRespawning = false;
    }

    // Smoothly fade the overlay to the target alpha
    private IEnumerator Fade(float targetAlpha)
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;

        if (targetAlpha == 0f)
            fadeImage.gameObject.SetActive(false);
    }
}