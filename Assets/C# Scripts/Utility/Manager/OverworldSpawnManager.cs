using UnityEngine;

/// <summary>
/// Handles spawning the player in the overworld at the correct position.
/// Uses SaveManager.overworldPlayerPosition if available,
/// otherwise falls back to a default spawn point.
/// </summary>
public class OverworldSpawnManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Optional. If empty, player will stay at its scene position.")]
    public Transform defaultSpawnPoint;

    [Tooltip("Tag used to find the player")]
    public string playerTag = "Player";

    [Header("Settings")]
    [Tooltip("Set player position on Start")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("OverworldSpawnManager: SaveManager not found!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogWarning("OverworldSpawnManager: Player not found!");
            return;
        }

        Vector3 savedPos = SaveManager.Instance.overworldPlayerPosition;

        if (savedPos != Vector3.zero)
        {
            player.transform.position = savedPos;
            Debug.Log($"OverworldSpawnManager: Spawned player at saved position {savedPos}");
        }
        else if (defaultSpawnPoint != null)
        {
            player.transform.position = defaultSpawnPoint.position;
            Debug.Log("OverworldSpawnManager: Spawned player at default spawn point");
        }
        else
        {
            Debug.Log("OverworldSpawnManager: No saved position and no default spawn point");
        }
    }
}
