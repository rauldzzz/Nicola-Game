using UnityEngine;

/*
 * EnemyKillCondition
 * -----------------
 * Completion condition that requires the player to kill a certain number of enemies.
 * - Can track enemies with a specific tag.
 * - Provides a progress string for the UI.
 * - Wasnt used in the end since the 2D platformer minigame wasnt included in the final build.
 */

[System.Serializable]
public class EnemyKillCondition : CompletionCondition
{
    public string enemyTag = "Enemy"; // Tag used to identify relevant enemies
    public int targetAmount = 1;      // Number of kills required

    private int currentAmount = 0;    // Tracks how many enemies have been killed so far

    // Call this whenever an enemy is killed
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
        // Display progress as "current/target"
        return $" {currentAmount}/{targetAmount}";
    }
}