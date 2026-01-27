using UnityEngine;
using TMPro;

/*
 * OverworldCoinManager
 * --------------------
 * Updates the UI to show the total coins collected in the overworld.
 * - Reads the coin count from SaveManager.
 * - Updates on Start and whenever the component is enabled.
 * - Was used in an older version of the overworld scenes.
 */

public class OverworldCoinManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text coinText; // Reference to the UI Text element for coins

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
        if (coinText == null)
        {
            // Helps detect if the UI Text reference was forgotten
            Debug.LogError("OverworldCoinManager: coinText not assigned in the Inspector.");
            return;
        }

        // Refresh UI whenever the component is enabled
        UpdateUI();
    }

    // Update the coin counter text
    public void UpdateUI()
    {
        coinText.text = SaveManager.Instance.totalCoins.ToString();
    }
}