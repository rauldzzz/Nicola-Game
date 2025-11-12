using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;           // Total hearts / health
    public int currentHealth;           // Current health
    public float invulnerabilityDuration = 1f; // Duration after taking damage

    [Header("Visual Feedback")]
    public Color flashColor = Color.white; // Color to flash
    public float flashSpeed = 0.1f;      // Speed of flashing

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvulnerable = false;

    public bool IsInvulnerable => isInvulnerable;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityFlash());
        }
    }

    private IEnumerator InvulnerabilityFlash()
    {
        isInvulnerable = true;
        float elapsed = 0f;

        while (elapsed < invulnerabilityDuration)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashSpeed);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashSpeed);

            elapsed += flashSpeed * 2;
        }

        spriteRenderer.color = originalColor;
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player died! (Later trigger death popup)");
        // TODO: Trigger death UI and disable controls
    }
}
