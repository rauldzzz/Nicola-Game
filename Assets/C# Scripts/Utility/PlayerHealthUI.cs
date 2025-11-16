using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the heart HUD based on the player's current health.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerHealth playerHealth;  // Reference to PlayerHealth script

    [Header("Heart Images")]
    public Image[] hearts;             // Array of heart images (3 for full HP)
    public Sprite fullHeart;           // Full heart sprite
    public Sprite emptyHeart;          // Empty heart sprite

    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not assigned in PlayerHealthUI!");
        }
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
