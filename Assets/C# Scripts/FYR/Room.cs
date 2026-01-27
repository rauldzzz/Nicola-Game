using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Room
 * ----
 * Represents a room in the procedural dungeon map.
 * - Holds references to tilemaps, entrances, and spawn points
 * - Calculates its size in both grid cells and world units
 * - Provides utility to access entrances by direction
 */
public class Room : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap wallTilemap;        // Tilemap used for room walls

    [Header("Entrances")]
    public Entrance entranceUp;        // Reference to top entrance
    public Entrance entranceDown;      // Reference to bottom entrance
    public Entrance entranceLeft;      // Reference to left entrance
    public Entrance entranceRight;     // Reference to right entrance

    [Header("Spawns")]
    public Transform playerSpawn;      // Player spawn point
    public Transform[] enemySpawns;    // Enemy spawn points

    [Header("Calculated Size")]
    public Vector2Int roomSize;        // Room size in grid cells (calculated from Tilemap)
    public Vector2 roomWorldSize;      // Room size in world units

    [HideInInspector] public Vector2Int gridPos; // Bottom-left position in the map grid
    [HideInInspector] public RoomType type;      // Type of room (Start, Normal, End, DeadEnd, etc.)

    void Awake()
    {
        if (wallTilemap != null)
        {
            // Calculate the room size from the tilemap bounds
            BoundsInt b = wallTilemap.cellBounds;
            roomSize = new Vector2Int(b.size.x, b.size.y);
            roomWorldSize = new Vector2(roomSize.x, roomSize.y);
        }
    }

    /// <summary>
    /// Returns the Entrance object for a given direction.
    /// </summary>
    public Entrance GetEntrance(Entrance.Direction dir)
    {
        return dir switch
        {
            Entrance.Direction.Up => entranceUp,
            Entrance.Direction.Down => entranceDown,
            Entrance.Direction.Left => entranceLeft,
            Entrance.Direction.Right => entranceRight,
            _ => null
        };
    }
}