using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildingEntrance : MonoBehaviour, IInteractable
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;             // Where to place the player after entering
    public Vector2 teleportOffset = Vector2.zero; // Additional X/Y offset for fine-tuning player spawn
    public float fadeDuration = 0.5f;           // Duration of fade-out and fade-in
    public float holdDuration = 0.2f;           // Time to stay fully black after teleport

    [Header("Fade UI")]
    public Image fadeImage;                      // Full-screen black UI image for fading

    private bool isTeleporting = false;         // Tracks if teleport is currently happening

    /// <summary>
    /// Returns whether this interactable can currently be used
    /// </summary>
    public bool CanInteract()
    {
        return !isTeleporting; // Cannot interact while teleport is happening
    }

    /// <summary>
    /// Called when the player interacts with this building entrance
    /// </summary>
    public void Interact()
    {
        // Safety check for missing references
        if (teleportTarget == null || fadeImage == null)
        {
            Debug.LogWarning("Teleport target or fade image not assigned!");
            return;
        }

        // Start teleport coroutine if not already teleporting
        if (!isTeleporting)
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    /// <summary>
    /// Handles fading, teleporting, holding, and restoring player control
    /// </summary>
    private IEnumerator TeleportPlayer()
    {
        isTeleporting = true; // Lock interactions

        // Freeze player movement by disabling the movement script
        GridMovementHold_Commented playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovementHold_Commented>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Fade out to black
        yield return StartCoroutine(Fade(1f));

        // Teleport the player to the target position plus optional offset
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 newPosition = teleportTarget.position + new Vector3(teleportOffset.x, teleportOffset.y, 0f);
            player.transform.position = newPosition;
        }

        // Hold black screen for holdDuration seconds so player is hidden
        yield return new WaitForSeconds(holdDuration);

        // Fade back in
        yield return StartCoroutine(Fade(0f));

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isTeleporting = false; // Unlock interactions
    }

    /// <summary>
    /// Handles smooth fade of the UI image to the target alpha
    /// </summary>
    /// <param name="targetAlpha">0 = fully transparent, 1 = fully black</param>
    private IEnumerator Fade(float targetAlpha)
    {
        // Make sure the fade image is active
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        // Interpolate alpha over fadeDuration
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null; // Wait until next frame
        }

        // Snap to final alpha
        color.a = targetAlpha;
        fadeImage.color = color;

        // If fully transparent, hide the image to avoid blocking UI
        if (targetAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}
