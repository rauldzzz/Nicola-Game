using UnityEngine;

/*
 * CoinCollectCondition
 * -------------------
 * Completion condition that requires the player to collect a certain number of coins.
 * - Checks against the CoinManager's total coins.
 * - Provides a status text showing progress.
 * - Wasnt used in the end since the 2D platformer minigame wasnt included in the final build.
 */

[System.Serializable]
public class CoinCollectCondition : CompletionCondition
{
    public int targetAmount = 10; // Number of coins needed to fulfill this condition

    public override bool IsFulfilled()
    {
        // Condition is fulfilled if player has collected enough coins
        return CoinManager.Instance != null && CoinManager.Instance.TotalCoins >= targetAmount;
    }

    public override string GetStatusText()
    {
        // Show progress as "current/target"
        int coins = CoinManager.Instance != null ? CoinManager.Instance.TotalCoins : 0;
        return $" {coins}/{targetAmount}";
    }
}