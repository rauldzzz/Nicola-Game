using UnityEngine;
using System.Collections;

public class RoomFogTrigger : MonoBehaviour
{
    [Header("Fog Settings")]
    public GameObject fogCover;       // assign the child sprite object
    public float fadeDuration = 1f;   // time in seconds for fade

    private bool revealed = false;
    private SpriteRenderer fogRenderer;

    private void Awake()
    {
        if (fogCover != null)
            fogRenderer = fogCover.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (revealed) return;

        if (other.CompareTag("Player"))
        {
            RevealFog();
        }
    }

    public void RevealFog()
    {
        revealed = true;

        if (fogRenderer != null)
        {
            StartCoroutine(FadeOutFog());
        }
        else if (fogCover != null)
        {
            // fallback if no SpriteRenderer
            fogCover.SetActive(false);
        }
    }

    private IEnumerator FadeOutFog()
    {
        float timer = 0f;
        Color originalColor = fogRenderer.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            fogRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, t));
            timer += Time.deltaTime;
            yield return null;
        }

        // ensure completely transparent at the end
        fogRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        fogCover.SetActive(false);
    }
}
