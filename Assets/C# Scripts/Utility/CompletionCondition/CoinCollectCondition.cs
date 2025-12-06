using UnityEngine;

/// <summary>
/// Requires the player to collect a certain number of coins
/// </summary>
[System.Serializable]
public class CoinCollectCondition : CompletionCondition
{
    public int targetAmount = 10;

public override bool IsFulfilled()
{
    return CoinManager.Instance != null && CoinManager.Instance.TotalCoins >= targetAmount;
}

public override string GetStatusText()
{
    int coins = CoinManager.Instance != null ? CoinManager.Instance.TotalCoins : 0;
    return $" {coins}/{targetAmount}";
}

}
