using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Settings")]
    public TMP_Text coinText;

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

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString();
    }
}
