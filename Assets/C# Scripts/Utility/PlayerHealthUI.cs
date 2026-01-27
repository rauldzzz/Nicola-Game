using UnityEngine;
using UnityEngine.UI;

/*
 * PlayerHealthUI
 * --------------
 * Updates the heart-based HUD to reflect the player's current health.
 * - Each heart in the array represents one health point.
 * - Full hearts indicate health remaining; empty hearts indicate lost health.
 */

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerHealth playerHealth;  // Reference to the player's health script

    [Header("Heart Images")]
    public Image[] hearts;             // Array of heart images
    public Sprite fullHeart;           // Sprite for a full heart
    public Sprite emptyHeart;          // Sprite for an empty heart

    void Start()
    {
        // Try to automatically find the player if reference not set
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            else
                Debug.LogError("PlayerHealthUI could not find Player in scene!");
        }
    }

    void Update()
    {
        UpdateHearts(); // Refresh the heart display every frame
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Set heart sprite based on current health
            hearts[i].sprite = (i < playerHealth.currentHealth) ? fullHeart : emptyHeart;
        }
    }
}