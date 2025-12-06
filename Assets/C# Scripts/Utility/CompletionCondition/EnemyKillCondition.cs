using UnityEngine;

/// <summary>
/// Requires the player to kill a certain number of enemies with a specific tag
/// </summary>
[System.Serializable]
public class EnemyKillCondition : CompletionCondition
{
    public string enemyTag = "Enemy";
    public int targetAmount = 1;

    private int currentAmount = 0;

    public void EnemyKilled()
    {
        currentAmount++;
    }

    public override bool IsFulfilled()
    {
        return currentAmount >= targetAmount;
    }

    public override string GetStatusText()
    {
        return $" {currentAmount}/{targetAmount}";
    }
}
