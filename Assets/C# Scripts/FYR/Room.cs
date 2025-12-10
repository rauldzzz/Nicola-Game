using UnityEngine;
using UnityEngine.Tilemaps;


public class Room : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap wallTilemap;

    [Header("Entrances")]
    public Entrance entranceUp;
    public Entrance entranceDown;
    public Entrance entranceLeft;
    public Entrance entranceRight;

    [Header("Spawns")]
    public Transform playerSpawn;
    public Transform[] enemySpawns;

    [Header("Calculated Size")]
    public Vector2Int roomSize;     // exact size in cells
    public Vector2 roomWorldSize;   // exact world size in units


    [HideInInspector] public Vector2Int gridPos; // position on the Map grid


    void Awake()
    {
         if (wallTilemap != null)
        {
            BoundsInt b = wallTilemap.cellBounds;
            roomSize = new Vector2Int(b.size.x, b.size.y);
            roomWorldSize = new Vector2(roomSize.x, roomSize.y);
        }
    }


    /// <summary>
    /// Get the entrance in the given direction.
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
