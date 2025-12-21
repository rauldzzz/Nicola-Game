using UnityEngine;

public class NPCTriggeredObject : MonoBehaviour
{
    [Header("References")]
    public NPC targetNPC;               
    public string minigameLevelName;    

    private void Awake()
    {
        if (targetNPC != null)
        {
            targetNPC.OnDialogueFinished.AddListener(ShowObject);
            Debug.Log($"[{name}] Subscribed to {targetNPC.name}'s dialogue event.");
        }
        else
        {
            Debug.LogWarning($"[{name}] targetNPC not assigned!");
        }

        // Start hidden
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // Show immediately if dialogue already finished and minigame not completed
        if (targetNPC != null && targetNPC.HasFinishedDialogue && !IsMinigameCompleted())
        {
            ShowObject();
        }
    }

    private void ShowObject()
    {
        if (!IsMinigameCompleted())
        {
            gameObject.SetActive(true);
            Debug.Log($"[{name}] Object shown after dialogue.");
        }
    }

    // Call this when the minigame is finished
    public void OnMinigameCompleted()
    {
        if (IsMinigameCompleted())
        {
            gameObject.SetActive(false);
            Debug.Log($"[{name}] Minigame '{minigameLevelName}' completed, object hidden forever.");
        }
    }

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
