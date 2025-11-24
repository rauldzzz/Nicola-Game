using UnityEngine;

/// <summary>
/// Base class for a level completion condition
/// </summary>
[System.Serializable]
public abstract class CompletionCondition
{
    public string description;

    // Check if condition is fulfilled
    public abstract bool IsFulfilled();

    // Return a status string for the UI (e.g., "Enemies: 3/5")
    public abstract string GetStatusText();
}
