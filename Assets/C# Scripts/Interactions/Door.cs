using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;               // Where to place the player after entering
    public Vector2 teleportOffset = Vector2.zero;  // Additional X/Y offset
    public float fadeDuration = 0.5f;             // Fade in/out duration
    public float holdDuration = 0.2f;             // Hold fully black screen

    [Header("Fade UI")]
    public Image fadeImage;                        // Full-screen black UI image

    [Header("Lock Settings (Optional)")]
    [Tooltip("Set a keyID to require a key. Leave empty for no key required.")]
    public string requiredKeyID = "";

    private bool isTeleporting = false;

    /// <summary>
    /// Returns whether this interactable can currently be used
    /// </summary>
    public bool CanInteract()
    {
        if (isTeleporting) return false;

        // Check if key is required
        if (!string.IsNullOrEmpty(requiredKeyID))
        {
            return SaveManager.Instance.collectedKeys.Contains(requiredKeyID);
        }

        return true;
    }

    /// <summary>
    /// Called when the player interacts with this door
    /// </summary>
    public void Interact()
    {
        if (!CanInteract())
        {
            Debug.Log("Door is locked! You need the key.");
            return;
        }

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
        isTeleporting = true;

        // Freeze player movement
        GridMovementHold playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovementHold>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Fade out
        yield return StartCoroutine(Fade(1f));

        // Teleport player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.position = teleportTarget.position + (Vector3)teleportOffset;

        // Hold black screen
        yield return new WaitForSeconds(holdDuration);

        // Fade back in
        yield return StartCoroutine(Fade(0f));

        // Restore player movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        isTeleporting = false;
    }

    /// <summary>
    /// Smooth fade for the UI image
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
