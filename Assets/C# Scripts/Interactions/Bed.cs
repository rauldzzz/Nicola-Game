using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/*
 * Bed
 * ---
 * Handles the player interacting with a bed to trigger a fade-to-black sequence.
 * - Disables player movement during fade.
 * - Fires a UnityEvent (OnBedUsed) for custom actions during sleep.
 * - Fades screen out, holds, then fades back in.
 * - Prevents multiple uses during one interaction.
 */

public class Bed : MonoBehaviour, IInteractable
{
    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public float holdDuration = 0.5f;

    [Header("Fade UI")]
    public Image fadeImage;

    [Header("Events")]
    public UnityEvent OnBedUsed;

    private bool isFading = false;
    private bool hasBeenUsed = false; // Prevents interacting twice

    public bool CanInteract()
    {
        return !isFading && !hasBeenUsed;
    }

    public void Interact()
    {
        if (!isFading && !hasBeenUsed)
            StartCoroutine(BedRoutine());
    }

    private IEnumerator BedRoutine()
    {
        isFading = true;
        hasBeenUsed = true; // Lock it immediately

        // Freeze player
        PlayerMovement playerMovement = 
            GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
        
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Fade out
        yield return StartCoroutine(Fade(1f));

        // Trigger custom actions
        OnBedUsed?.Invoke();

        // Hold black screen
        yield return new WaitForSeconds(holdDuration);

        // Fade in
        yield return StartCoroutine(Fade(0f));

        // Restore movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        isFading = false;
    }

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