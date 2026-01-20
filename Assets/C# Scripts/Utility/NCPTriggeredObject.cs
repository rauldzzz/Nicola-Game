using UnityEngine;

public class NPCTriggeredObject : MonoBehaviour
{
    [Header("References")]
    public NPC targetNPC;
    public string minigameLevelName;

    [Header("Settings")]
    [Tooltip("If true, the object will start visible even before dialogue/minigame.")]
    public bool startVisible = false;

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


        gameObject.SetActive(startVisible);
    }

    private void Start()
    {

        if (targetNPC != null && targetNPC.HasFinishedDialogue)
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

    // Call this when the minigame is completed
    public void OnMinigameCompleted(bool completed)
    {
        gameObject.SetActive(!completed);
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
