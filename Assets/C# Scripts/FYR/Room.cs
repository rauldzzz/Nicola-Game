using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap wallTilemap;   // Tilemap with Obstacles

    [Header("Spawns")]
    public Transform playerSpawn;
    public Transform[] enemySpawns;

    /// <summary>
    /// Tells all Movement-Scripts in the Room what the current obsctacle Tilemap is.
    /// </summary>
    public void AssignTilemaps(GridMovementHold player, GridEnemyMovement[] enemies)
    {
        if (player != null)
            player.obstacleTilemap = wallTilemap;

        foreach (var enemy in enemies)
            enemy.obstacleTilemap = wallTilemap;
    }
}
