using UnityEngine;

/*
 * CompletionCondition
 * ------------------
 * Base class for any condition that must be met to complete a level.
 * - Derived classes implement the actual check and UI status.
 */

[System.Serializable]
public abstract class CompletionCondition
{
    public string description; // Optional description for UI or debugging

    // Check whether the condition has been fulfilled
    public abstract bool IsFulfilled();

    // Return a string representing progress for the UI (e.g., "3/5 enemies defeated")
    public abstract string GetStatusText();
}