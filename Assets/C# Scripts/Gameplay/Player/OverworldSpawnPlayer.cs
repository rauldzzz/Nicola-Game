using UnityEngine;

/*
 * OverworldSpawnPlayer
 * -------------------
 * Handles placing the player in the overworld scene.
 * - If a saved overworld position exists, spawn the player there.
 * - Otherwise, spawn at the default spawn point.
 */
public class OverworldSpawnPlayer : MonoBehaviour
{
    public Transform defaultSpawnPoint; // Default position if no saved position exists

    void Start()
    {
        // Use saved overworld position if it exists, otherwise use default spawn
        if (SaveManager.Instance == null || SaveManager.Instance.overworldPlayerPosition == Vector3.zero)
        {
            transform.position = defaultSpawnPoint.position;
        }
        else
        {
            transform.position = SaveManager.Instance.overworldPlayerPosition;
        }
    }
}