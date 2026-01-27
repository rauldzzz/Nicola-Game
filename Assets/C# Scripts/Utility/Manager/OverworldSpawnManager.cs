using UnityEngine;

/*
 * OverworldSpawnManager
 * --------------------
 * Handles spawning the player in the overworld at the correct position.
 * - Uses SaveManager.overworldPlayerPosition if available.
 * - Falls back to a default spawn point if no saved position exists.
 * - Can optionally spawn the player automatically on Start.
 */

public class OverworldSpawnManager : MonoBehaviour
{
    [Header("References")]
    public Transform defaultSpawnPoint; // Optional fallback spawn point
    public string playerTag = "Player"; // Tag to find the player object

    [Header("Settings")]
    public bool spawnOnStart = true; // Set true to spawn automatically on Start

    private void Start()
    {
        if (spawnOnStart)
            SpawnPlayer();
    }

    // Spawns the player at the saved position or default point
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
            // Spawn at the last saved position
            player.transform.position = savedPos;
            Debug.Log($"OverworldSpawnManager: Spawned player at saved position {savedPos}");
        }
        else if (defaultSpawnPoint != null)
        {
            // Spawn at the default point if no saved position
            player.transform.position = defaultSpawnPoint.position;
            Debug.Log("OverworldSpawnManager: Spawned player at default spawn point");
        }
        else
        {
            // No valid spawn location found
            Debug.Log("OverworldSpawnManager: No saved position and no default spawn point");
        }
    }
}