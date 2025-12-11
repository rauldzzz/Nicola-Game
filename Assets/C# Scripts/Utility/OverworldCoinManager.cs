using UnityEngine;
using TMPro;

public class OverworldCoinManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text coinText; // Assign your UI Text element here

    private void Start()
    {
        if (coinText == null)
        {
            Debug.LogWarning("OverworldCoinUI: coinText not assigned!");
            return;
        }

        UpdateUI();
    }

    private void OnEnable()
    {
        // Update UI whenever the overworld scene loads
        UpdateUI();
    }

    /// <summary>
    /// Call this whenever coins change
    /// </summary>
    public void UpdateUI()
    {
        coinText.text = SaveManager.Instance.totalCoins.ToString();
    }
}
