using System.Collections;
using UnityEngine;
using TMPro;

/*
 * CoinManager
 * -----------
 * Handles tracking the player's coins and updating the UI.
 * - Uses a singleton pattern for easy access.
 * - Updates coin text and optionally plays a pop animation on the coin icon.
 * - Loads saved coin count from SaveManager on start.
 * - Was used in the old overworld and 2D platformer minigame.
 */

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Settings")]
    public TMP_Text coinText;                // Reference to coin counter text
    public RectTransform coinIconUI;         // Optional icon for pop animation

    [Header("Animation Settings")]
    public float popScale = 1.2f;
    public float popTime = 0.15f;

    private int totalCoins = 0;
    public int TotalCoins => totalCoins;

    private void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Load saved total coins
        totalCoins = SaveManager.Instance.totalCoins;
        UpdateUI();
    }

    // Add coins, update UI, and play pop animation
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();
        PlayCoinUIAnimation();
    }

    // Refresh coin counter text
    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString();
    }

    // Play pop animation on coin icon
    public void PlayCoinUIAnimation()
    {
        if (coinIconUI != null)
            StartCoroutine(PopCoinUI());
    }

    private IEnumerator PopCoinUI()
    {
        Vector3 originalScale = coinIconUI.localScale;
        Vector3 targetScale = originalScale * popScale;

        float elapsed = 0f;
        // Scale up
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            coinIconUI.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popTime);
            yield return null;
        }

        elapsed = 0f;
        // Scale back down
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            coinIconUI.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / popTime);
            yield return null;
        }
    }
}