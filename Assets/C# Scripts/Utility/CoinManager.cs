using System.Collections;
using UnityEngine;
using TMPro;

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
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        totalCoins = SaveManager.Instance.totalCoins; // Load saved total
        UpdateUI();
    }

    /// <summary>
    /// Add coins and update UI
    /// </summary>
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();
        PlayCoinUIAnimation();
    }

    /// <summary>
    /// Update the coin text
    /// </summary>
    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString();
    }

    /// <summary>
    /// Simple pop animation for the coin icon UI
    /// </summary>
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
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            coinIconUI.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popTime);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            coinIconUI.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / popTime);
            yield return null;
        }
    }
}
