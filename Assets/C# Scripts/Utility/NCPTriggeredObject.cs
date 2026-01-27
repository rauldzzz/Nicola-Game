using UnityEngine;

/*
 * NPCTriggeredObject
 * -----------------
 * Shows or hides a GameObject based on an NPC's dialogue completion
 * and optionally whether a minigame has been completed.
 * - Subscribes to the target NPC's dialogue finished event.
 * - Can start visible before dialogue/minigame if configured.
 * - Automatically updates visibility based on SaveManager data.
 */

public class NPCTriggeredObject : MonoBehaviour
{
    [Header("References")]
    public NPC targetNPC;                 // NPC to listen for dialogue completion
    public string minigameLevelName;      // Optional minigame that can block object visibility

    [Header("Settings")]
    [Tooltip("If true, the object will start visible even before dialogue/minigame.")]
    public bool startVisible = false;

    private void Awake()
    {
        // Subscribe to the NPC's dialogue event
        if (targetNPC != null)
        {
            targetNPC.OnDialogueFinished.AddListener(ShowObject);
            Debug.Log($"[{name}] Subscribed to {targetNPC.name}'s dialogue event.");
        }
        else
        {
            Debug.LogWarning($"[{name}] targetNPC not assigned!");
        }

        // Set initial visibility
        gameObject.SetActive(startVisible);
    }

    private void Start()
    {
        // If the NPC dialogue was already completed before start, show the object
        if (targetNPC != null && targetNPC.HasFinishedDialogue)
            ShowObject();
    }

    // Show the object if the minigame isn't completed
    private void ShowObject()
    {
        if (!IsMinigameCompleted())
        {
            gameObject.SetActive(true);
            Debug.Log($"[{name}] Object shown after dialogue.");
        }
    }

    // Call this externally when a minigame is completed
    public void OnMinigameCompleted(bool completed)
    {
        gameObject.SetActive(!completed);
    }

    // Check SaveManager to see if the minigame is already completed
    private bool IsMinigameCompleted()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning($"[{name}] SaveManager not found, assuming minigame not completed.");
            return false;
        }

        return SaveManager.Instance.completedLevels.Contains(minigameLevelName);
    }
}