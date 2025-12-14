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
        if (coinText == null)
        {
            // Esto ayudará a ver si realmente olvidaste la asignación
            Debug.LogError("ERROR CRÍTICO: coinText no está asignado en el Inspector en el OverworldCoinManager.");
            return;
        }
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
