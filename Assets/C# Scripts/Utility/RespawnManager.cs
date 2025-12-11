using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance; // Singleton reference

    [Header("Respawn Settings")]
    [Tooltip("Fade image used for transition effect (black screen)")]
    public Image fadeImage;

    [Tooltip("How long the fade in/out takes")]
    public float fadeDuration = 0.5f;

    [Tooltip("How long the screen stays black before fading back in")]
    public float holdDuration = 0.3f;

    private Transform currentCheckpoint; // Last checkpoint reached
    private bool isRespawning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Sets the current respawn position.
    /// </summary>
    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    /// <summary>
    /// Begins the respawn process for the player.
    /// </summary>
    public void RespawnPlayer(GameObject player)
    {
        if (!isRespawning && currentCheckpoint != null)
            StartCoroutine(RespawnRoutine(player));
    }

    /// <summary>
    /// Handles fading, teleporting, and safely restoring movement and physics.
    /// </summary>
    private IEnumerator RespawnRoutine(GameObject player)
    {
        isRespawning = true;

        // Disable movement + physics temporarily
        var rb = player.GetComponent<Rigidbody2D>();
        var movement = player.GetComponent<PlayerMovement2DPolished_Controls>();

        if (movement != null)
            movement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f; // Prevent falling while screen fades
        }

        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Teleport to the last checkpoint
        player.transform.position = currentCheckpoint.position;

        // Wait while screen is black
        yield return new WaitForSeconds(holdDuration);

        // Fade back in
        yield return StartCoroutine(Fade(0f));

        // Re-enable physics and movement
        if (rb != null)
            rb.gravityScale = 2f; // set this back to your normal gravity value
        if (movement != null)
            movement.enabled = true;

        isRespawning = false;
    }

    /// <summary>
    /// Smooth fade in/out of the black overlay image.
    /// </summary>
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
