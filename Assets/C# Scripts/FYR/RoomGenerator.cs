using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * RoomGenerator
 * -------------
 * Procedurally generates a dungeon-like map using RoomData prefabs.
 * - Places start room, normal rooms, required special rooms, end room, and dead-ends
 * - Tracks occupied grid cells to prevent overlapping
 * - Connects rooms via entrances
 * - Spawns player and enemy prefabs
 */
public class RoomGenerator : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData startRoom;          // Starting room of the map
    public RoomData[] normalRooms;      // Regular rooms that can appear in the map
    public RoomData[] endRooms;         // Special end rooms
    public RoomData[] deadEndRooms;     // Dead-end rooms to fill open entrances

    [Header("Prefabs")]
    public Transform playerPrefab;      // Player spawn prefab
    public Transform enemyPrefab;       // Enemy prefab to spawn in rooms

    // Tracks which grid cells are occupied to prevent overlaps
    private Dictionary<Vector2Int, Room> occupiedCells = new();

    // All rooms instantiated in this map
    private List<Room> allRooms = new();

    // Graph connecting rooms (key = room, value = adjacent rooms)
    private Dictionary<Room, List<Room>> roomGraph = new();

    // Directions used for entrances
    private readonly Entrance.Direction[] directions =
    {
        Entrance.Direction.Up,
        Entrance.Direction.Down,
        Entrance.Direction.Left,
        Entrance.Direction.Right
    };

    void Start()
    {
        GenerateMap();
    }

    // Main map generation logic
    void GenerateMap()
    {
        occupiedCells.Clear();
        allRooms.Clear();
        roomGraph.Clear();

        // 1. Place Start Room
        Room start = Instantiate(startRoom.prefab).GetComponent<Room>();
        start.type = RoomType.Start;
        start.transform.position = Vector3.zero;
        start.gridPos = GetBottomLeftGridPos(start, Vector3.zero);
        MarkRoomCells(start, start.gridPos);
        allRooms.Add(start);
        roomGraph[start] = new List<Room>();

        // Find all open entrances of start room
        List<(Room room, Entrance.Direction dir)> openEntrances = new();
        foreach (var dir in directions)
        {
            var e = start.GetEntrance(dir);
            if (e != null && e.gameObject.activeSelf)
                openEntrances.Add((start, dir));
        }

        // Track required rooms 3-6 to ensure placement
        HashSet<string> requiredRooms = new() { "Room3Data", "Room4Data", "Room5Data", "Room6Data" };

        // 2. Place normal rooms
        List<RoomData> normalQueue = new(normalRooms);
        ShuffleList(normalQueue);

        foreach (var data in normalQueue)
        {
            ShuffleList(openEntrances);
            foreach (var (parent, dir) in openEntrances.ToArray())
            {
                // Room5 and Room6 cannot be placed directly next to start
                if (parent == start && (data.name == "Room5Data" || data.name == "Room6Data")) continue;

                if (TryPlaceNextRoomAtOpenEntrance(parent, dir, data, out Room placed))
                {
                    placed.type = data.type;
                    allRooms.Add(placed);
                    openEntrances.Remove((parent, dir));

                    if (requiredRooms.Contains(data.name)) requiredRooms.Remove(data.name);

                    // Add new open entrances from the placed room
                    foreach (var d in directions)
                    {
                        var e = placed.GetEntrance(d);
                        if (e != null && e.gameObject.activeSelf)
                            openEntrances.Add((placed, d));
                    }
                    break;
                }
            }
        }

        // 2b. Ensure required rooms 3-6 are placed at least once
        foreach (string roomName in requiredRooms)
        {
            RoomData data = normalRooms.FirstOrDefault(r => r.name == roomName);
            if (data == null) continue;

            ShuffleList(openEntrances);
            bool placedRoom = false;
            foreach (var (parent, dir) in openEntrances.ToArray())
            {
                if (parent == start && (data.name == "Room5Data" || data.name == "Room6Data")) continue;

                if (TryPlaceNextRoomAtOpenEntrance(parent, dir, data, out Room placed))
                {
                    placed.type = data.type;
                    allRooms.Add(placed);
                    openEntrances.Remove((parent, dir));

                    foreach (var d in directions)
                    {
                        var e = placed.GetEntrance(d);
                        if (e != null && e.gameObject.activeSelf)
                            openEntrances.Add((placed, d));
                    }

                    placedRoom = true;
                    break;
                }
            }

            if (!placedRoom)
                Debug.LogWarning($"Could not place required room {roomName} due to lack of space.");
        }

        // 3. Place end room at furthest room
        Room furthestRoom = GetFurthestRoomFromStart(start);
        if (furthestRoom != null)
        {
            PlaceEndRoomAt(furthestRoom);
            openEntrances.RemoveAll(e => e.room == furthestRoom);
        }
        else
        {
            Debug.LogWarning("No room found for end room placement!");
        }

        // 4. Place dead-ends at remaining open entrances
        foreach (var (parent, dir) in openEntrances.ToArray())
        {
            RoomData dead = deadEndRooms.FirstOrDefault(d => d.prefab.GetComponent<Room>().GetEntrance(OppositeDirection(dir)) != null);
            if (dead != null && TryPlaceNextRoomAtOpenEntrance(parent, dir, dead, out Room placed))
            {
                placed.type = RoomType.DeadEnd;
                allRooms.Add(placed);
            }
        }

        // 5. Connect entrances (currently empty, may handle logic later)
        ConnectEntrances();

        // 5b. Fill remaining open entrances with dead-ends
        FillRemainingOpenEntrancesWithDeadEnds();

        // 6. Spawn player and enemy entities
        SpawnEntities();
    }

    // Finds the room furthest away from the start using BFS
    Room GetFurthestRoomFromStart(Room start)
    {
        Dictionary<Room, int> distances = new();
        Queue<Room> queue = new();
        queue.Enqueue(start);
        distances[start] = 0;

        Room furthest = start;
        int maxDist = 0;

        while (queue.Count > 0)
        {
            Room current = queue.Dequeue();
            int dist = distances[current];

            if (dist > maxDist)
            {
                maxDist = dist;
                furthest = current;
            }

            if (roomGraph.ContainsKey(current))
            {
                foreach (var neighbor in roomGraph[current])
                {
                    if (!distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = dist + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return furthest;
    }

    // Attempts to place an end room connected to a given room
    void PlaceEndRoomAt(Room parent)
    {
        foreach (var dir in directions)
        {
            var openEntrance = parent.GetEntrance(dir);
            if (openEntrance == null || !openEntrance.gameObject.activeSelf) continue;

            Entrance.Direction requiredDir = OppositeDirection(dir);
            RoomData endData = endRooms.FirstOrDefault(e =>
                (e.type == RoomType.EndUp && requiredDir == Entrance.Direction.Up) ||
                (e.type == RoomType.EndDown && requiredDir == Entrance.Direction.Down) ||
                (e.type == RoomType.EndLeft && requiredDir == Entrance.Direction.Left) ||
                (e.type == RoomType.EndRight && requiredDir == Entrance.Direction.Right));

            if (endData == null) continue;

            Room prefab = endData.prefab.GetComponent<Room>();
            Vector3 parentPos = openEntrance.transform.position;
            Vector3 childLocal = prefab.GetEntrance(requiredDir).transform.localPosition;
            Vector3 spawnPos = parentPos - childLocal;

            if (!CanPlaceRoom(prefab, spawnPos)) continue;

            Room endRoom = Instantiate(endData.prefab).GetComponent<Room>();
            endRoom.type = endData.type;
            endRoom.transform.position = spawnPos;
            endRoom.gridPos = GetBottomLeftGridPos(endRoom, spawnPos);
            MarkRoomCells(endRoom, endRoom.gridPos);
            allRooms.Add(endRoom);

            openEntrance.Close();
            endRoom.GetEntrance(requiredDir)?.Close();

            if (!roomGraph.ContainsKey(parent)) roomGraph[parent] = new List<Room>();
            roomGraph[parent].Add(endRoom);
            roomGraph[endRoom] = new List<Room> { parent };

            Debug.Log($"End room placed at furthest room: {endRoom.name}");
            break;
        }
    }

    #region Core placement & Helpers

    // Tries to place a room prefab at a parent's open entrance
    bool TryPlaceNextRoomAtOpenEntrance(Room parent, Entrance.Direction parentDir, RoomData roomData, out Room placed)
    {
        placed = null;
        Room prefab = roomData.prefab.GetComponent<Room>();
        Entrance.Direction needed = OppositeDirection(parentDir);

        if (prefab.GetEntrance(needed) == null) return false;

        Vector3 parentPos = parent.GetEntrance(parentDir).transform.position;
        Vector3 childLocal = prefab.GetEntrance(needed).transform.localPosition;
        Vector3 spawnPos = parentPos - childLocal;

        if (!CanPlaceRoom(prefab, spawnPos)) return false;

        Room child = Instantiate(roomData.prefab).GetComponent<Room>();
        child.transform.position = spawnPos;
        child.gridPos = GetBottomLeftGridPos(child, spawnPos);
        MarkRoomCells(child, child.gridPos);

        parent.GetEntrance(parentDir)?.Close();
        child.GetEntrance(needed)?.Close();

        if (!roomGraph.ContainsKey(parent)) roomGraph[parent] = new();
        if (!roomGraph.ContainsKey(child)) roomGraph[child] = new();
        roomGraph[parent].Add(child);
        roomGraph[child].Add(parent);

        placed = child;
        return true;
    }

    // Converts world position to bottom-left grid coordinates for the room
    Vector2Int GetBottomLeftGridPos(Room room, Vector3 pos) =>
        new(Mathf.FloorToInt(pos.x - room.roomWorldSize.x / 2f), Mathf.FloorToInt(pos.y - room.roomWorldSize.y / 2f));

    // Returns all grid cells occupied by a room given its bottom-left position
    List<Vector2Int> GetOccupiedCells(Room room, Vector2Int bl)
    {
        List<Vector2Int> cells = new();
        for (int x = 0; x < room.roomSize.x; x++)
            for (int y = 0; y < room.roomSize.y; y++)
                cells.Add(bl + new Vector2Int(x, y));
        return cells;
    }

    // Checks if a room can be placed at a given position
    bool CanPlaceRoom(Room room, Vector3 pos)
    {
        Vector2Int bl = GetBottomLeftGridPos(room, pos);
        foreach (var c in GetOccupiedCells(room, bl))
            if (occupiedCells.ContainsKey(c)) return false;
        return true;
    }

    // Marks grid cells as occupied by the given room
    void MarkRoomCells(Room room, Vector2Int bl)
    {
        foreach (var c in GetOccupiedCells(room, bl))
            occupiedCells[c] = room;
    }

    // Returns the opposite of a given direction
    Entrance.Direction OppositeDirection(Entrance.Direction d) =>
        d == Entrance.Direction.Up ? Entrance.Direction.Down :
        d == Entrance.Direction.Down ? Entrance.Direction.Up :
        d == Entrance.Direction.Left ? Entrance.Direction.Right : Entrance.Direction.Left;

    // Simple in-place list shuffling
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void ConnectEntrances() { }

    // Spawns player and enemies at the designated spawn points
    void SpawnEntities()
    {
        if (allRooms.Count == 0) return;
        Room start = allRooms[0];
        if (playerPrefab != null && start.playerSpawn != null)
        {
            var existingPlayer = FindAnyObjectByType<PlayerHealth>();
            if (existingPlayer != null) existingPlayer.transform.position = start.playerSpawn.position;
            else Instantiate(playerPrefab, start.playerSpawn.position, Quaternion.identity);
        }

        if (enemyPrefab != null)
        {
            foreach (var room in allRooms)
            {
                if (room.enemySpawns == null) continue;
                foreach (var spawn in room.enemySpawns)
                    if (spawn != null) Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
            }
        }
    }

    // Iterates remaining open entrances and fills them with dead-end rooms
    void FillRemainingOpenEntrancesWithDeadEnds()
    {
        List<Room> roomsToCheck = new List<Room>(allRooms);

        foreach (var room in roomsToCheck)
        {
            foreach (var dir in directions)
            {
                var entrance = room.GetEntrance(dir);
                if (entrance == null || !entrance.gameObject.activeSelf) continue;

                RoomData deadEndData = deadEndRooms.FirstOrDefault(d =>
                {
                    Room r = d.prefab.GetComponent<Room>();
                    return r.GetEntrance(OppositeDirection(dir)) != null;
                });

                if (deadEndData == null) continue;

                if (TryPlaceNextRoomAtOpenEntrance(room, dir, deadEndData, out Room placed))
                {
                    placed.type = RoomType.DeadEnd;
                    allRooms.Add(placed); // safe because we iterate over a copy
                }
            }
        }
    }

    #endregion
}