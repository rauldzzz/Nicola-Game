using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive = false; // Tracks if this checkpoint is currently active

    [Header("Visual Settings")]
    public SpriteRenderer flagRenderer; // The sprite of the flag or checkpoint
    public Sprite activeSprite;         // Sprite when checkpoint is active
    public Sprite defaultSprite;        // Sprite when checkpoint is inactive

    private void Awake()
    {
        // If flagRenderer is not assigned, try to get it from this GameObject
        if (flagRenderer == null)
            flagRenderer = GetComponent<SpriteRenderer>();

        // Set initial sprite
        if (flagRenderer != null)
            flagRenderer.sprite = defaultSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only activate when the player enters
        if (other.CompareTag("Player") && !isActive)
        {
            ActivateCheckpoint();
        }
    }

    /// <summary>
    /// Activates this checkpoint, updates RespawnManager, and resets other checkpoints.
    /// </summary>
    private void ActivateCheckpoint()
    {
        isActive = true;

        // Register this checkpoint as the respawn point
        RespawnManager.Instance.SetCheckpoint(transform);

        // Reset all other checkpoints
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (var cp in allCheckpoints)
        {
            if (cp != this)
            {
                cp.SetInactive();
            }
        }

        // Set this checkpoint to active sprite
        if (flagRenderer != null)
            flagRenderer.sprite = activeSprite;

        Debug.Log("Checkpoint activated: " + name);
    }

    /// <summary>
    /// Sets this checkpoint back to default (inactive) state.
    /// </summary>
    public void SetInactive()
    {
        isActive = false;
        if (flagRenderer != null)
            flagRenderer.sprite = defaultSprite;
    }
}
