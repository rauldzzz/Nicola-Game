using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData startRoom;
    public RoomData[] normalRooms;   // 1-6
    public RoomData[] endRooms;      // 1 per direction
    public RoomData[] deadEndRooms;  // 7-10

    [Header("Prefabs")]
    public Transform playerPrefab;
    public Transform enemyPrefab;

    private Dictionary<Vector2Int, Room> occupiedCells = new();
    private List<Room> allRooms = new();
    private readonly Entrance.Direction[] directions =
        { Entrance.Direction.Up, Entrance.Direction.Down, Entrance.Direction.Left, Entrance.Direction.Right };

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        occupiedCells.Clear();
        allRooms.Clear();

        // 1. Place Start Room 
        if (!CheckRoomData(startRoom, "Start Room")) return;
        Room start = Instantiate(startRoom.prefab).GetComponent<Room>();
        start.transform.position = Vector3.zero;
        start.gridPos = GetBottomLeftGridPos(start, Vector3.zero);
        MarkRoomCells(start, start.gridPos);
        allRooms.Add(start);

        // Open entrances list 
        List<(Room room, Entrance.Direction dir)> openEntrances = new();
        foreach (var dir in directions)
        {
            var e = start.GetEntrance(dir);
            if (e != null && e.gameObject.activeSelf)
                openEntrances.Add((start, dir));
        }

        // 2. Place Normal Rooms (1-6) 
        List<RoomData> normalQueue = new List<RoomData>(normalRooms);
        ShuffleList(normalQueue);

        int room1Count = 0;
        int room2Count = 0;
        bool firstRoomPlaced = false;

        foreach (var roomData in normalQueue)
        {
            // Skip Room 5/6 for the first room
            if (!firstRoomPlaced && (roomData.name == "Room5Data" || roomData.name == "Room6Data"))
                continue;

            // Skip Room 1 or 2 if already appeared 3 times
            if (roomData.name == "Room1Data" && room1Count >= 3) continue;
            if (roomData.name == "Room2Data" && room2Count >= 3) continue;

            bool placedRoom = false;
            ShuffleList(openEntrances);

            foreach (var (parent, dir) in openEntrances.ToArray())
            {
                if (TryPlaceNextRoomAtOpenEntrance(parent, dir, roomData, out Room placed))
                {
                    allRooms.Add(placed);
                    placedRoom = true;

                    firstRoomPlaced = true;

                    // Update counts
                    if (roomData.name == "Room1Data") room1Count++;
                    if (roomData.name == "Room2Data") room2Count++;

                    // Add child’s open entrances
                    foreach (var newDir in directions)
                    {
                        var e = placed.GetEntrance(newDir);
                        if (e != null && e.gameObject.activeSelf)
                            openEntrances.Add((placed, newDir));
                    }

                    // Remove used parent entrance
                    openEntrances.Remove((parent, dir));
                    break;
                }
            }

            if (!placedRoom)
                Debug.LogWarning($"Could not place normal room {roomData.name}");
        }


        // 3. Place End Room 
        bool endPlaced = false;
        ShuffleList(openEntrances);

        foreach (var (parent, dir) in openEntrances.ToArray())
        {
            if (parent == startRoom)
                continue;

            RoomData endData = FindFittingEndRoomAtEntrance(parent, dir);
            if (endData != null)
            {
                if (TryPlaceNextRoomAtOpenEntrance(parent, dir, endData, out Room placed))
                {
                    allRooms.Add(placed);
                    endPlaced = true;

                    openEntrances.Remove((parent, dir));
                    break;
                }
            }
        }

        if (!endPlaced)
            Debug.LogWarning("Could not place any end room!");


        // 4. Fill remaining open entrances with dead-ends
        foreach (var (parent, dir) in openEntrances)
        {
            RoomData deadData = FindDeadEndForDirection(OppositeDirection(dir));
            if (deadData != null)
                TryPlaceNextRoomAtOpenEntrance(parent, dir, deadData, out _);
        }

        // 5. Connect entrances  
        ConnectEntrances();

        // 6. Spawn Player and Enemies  
        SpawnEntities();

        Debug.Log("Map generation complete!");
    }

    bool CheckRoomData(RoomData data, string name)
    {
        if (data == null)
        {
            Debug.LogError($"{name} RoomData is not assigned!");
            return false;
        }
        if (data.prefab == null)
        {
            Debug.LogError($"{name} prefab inside RoomData is not assigned!");
            return false;
        }
        if (data.prefab.GetComponent<Room>() == null)
        {
            Debug.LogError($"{name} prefab does not have a Room component!");
            return false;
        }
        return true;
    }

    // Main placement function  
    bool TryPlaceNextRoomAtOpenEntrance(Room parent, Entrance.Direction parentOpenDir, RoomData roomData, out Room placed)
    {
        placed = null;
        Room childPrefab = roomData.prefab.GetComponent<Room>();
        Entrance.Direction requiredDir = OppositeDirection(parentOpenDir);

        // Child must have matching entrance
        if (childPrefab.GetEntrance(requiredDir) == null)
            return false;

        Vector3 parentEntrancePos = parent.GetEntrance(parentOpenDir).transform.position;
        Vector3 childEntranceLocal = childPrefab.GetEntrance(requiredDir).transform.localPosition;
        Vector3 spawnPos = parentEntrancePos - childEntranceLocal;

        // Check for overlaps  
        if (!CanPlaceRoom(childPrefab, spawnPos))
            return false;

        // Size-aware checks  
        if (!RoomSizeFits(childPrefab, spawnPos))
            return false;

        // Instantiate room
        Room child = Instantiate(roomData.prefab).GetComponent<Room>();
        child.transform.position = spawnPos;
        child.gridPos = GetBottomLeftGridPos(child, spawnPos);
        MarkRoomCells(child, child.gridPos);
        placed = child;

        // Close connected entrances
        parent.GetEntrance(parentOpenDir)?.Close();
        child.GetEntrance(requiredDir)?.Close();

        return true;
    }

    // Size-aware placement logic  
    bool RoomSizeFits(Room room, Vector3 pos)
    {
        Vector2Int bottomLeft = GetBottomLeftGridPos(room, pos);

        // Big squares: prevent overlap with other big squares
        bool isBig = room.roomSize.x > 3 && room.roomSize.y > 3;
        bool isHorizontalRect = room.roomSize.x > room.roomSize.y && room.roomSize.x > 2;
        bool isVerticalRect = room.roomSize.y > room.roomSize.x && room.roomSize.y > 2;

        foreach (var cell in GetOccupiedCells(room, bottomLeft))
        {
            if (occupiedCells.ContainsKey(cell))
            {
                Room existing = occupiedCells[cell];
                if (isBig && existing.roomSize.x > 3 && existing.roomSize.y > 3)
                    return false;
                if (isHorizontalRect && existing.roomSize.x > existing.roomSize.y && existing.roomSize.x > 2)
                    return false;
                if (isVerticalRect && existing.roomSize.y > existing.roomSize.x && existing.roomSize.y > 2)
                    return false;
            }
        }
        return true;
    }

    RoomData FindFittingEndRoomAtEntrance(Room parent, Entrance.Direction parentOpenDir)
    {
        if (endRooms == null || endRooms.Length == 0) return null;
        List<RoomData> shuffled = new List<RoomData>(endRooms);
        ShuffleList(shuffled);

        Entrance.Direction requiredDir = OppositeDirection(parentOpenDir);

        foreach (var endData in shuffled)
        {
            Room r = endData.prefab.GetComponent<Room>();
            if (r.GetEntrance(requiredDir) == null)
                continue;

            Vector3 parentEntrancePos = parent.GetEntrance(parentOpenDir).transform.position;
            Vector3 childEntranceLocal = r.GetEntrance(requiredDir).transform.localPosition;
            Vector3 spawnPos = parentEntrancePos - childEntranceLocal;
            if (CanPlaceRoom(r, spawnPos) && RoomSizeFits(r, spawnPos))
                return endData;
        }

        return null;
    }

    RoomData FindDeadEndForDirection(Entrance.Direction dir)
    {
        if (deadEndRooms == null || deadEndRooms.Length == 0) return null;

        List<RoomData> shuffled = new List<RoomData>(deadEndRooms);
        ShuffleList(shuffled);

        foreach (var d in shuffled)
            if (d.prefab.GetComponent<Room>().GetEntrance(dir) != null)
                return d;

        return null;
    }

    Vector2Int GetBottomLeftGridPos(Room room, Vector3 worldPos)
    {
        Vector2 bottomLeft = new Vector2(
            worldPos.x - room.roomWorldSize.x / 2f,
            worldPos.y - room.roomWorldSize.y / 2f
        );
        return new Vector2Int(Mathf.FloorToInt(bottomLeft.x), Mathf.FloorToInt(bottomLeft.y));
    }

    List<Vector2Int> GetOccupiedCells(Room room, Vector2Int bottomLeftGridPos)
    {
        List<Vector2Int> cells = new();
        for (int x = 0; x < room.roomSize.x; x++)
            for (int y = 0; y < room.roomSize.y; y++)
                cells.Add(bottomLeftGridPos + new Vector2Int(x, y));
        return cells;
    }

    bool CanPlaceRoom(Room room, Vector3 pos)
    {
        Vector2Int bottomLeft = GetBottomLeftGridPos(room, pos);
        foreach (var cell in GetOccupiedCells(room, bottomLeft))
            if (occupiedCells.ContainsKey(cell))
                return false;
        return true;
    }

    void MarkRoomCells(Room room, Vector2Int bottomLeftGridPos)
    {
        foreach (var cell in GetOccupiedCells(room, bottomLeftGridPos))
            occupiedCells[cell] = room;
    }

    Entrance.Direction OppositeDirection(Entrance.Direction dir)
        => dir switch
        {
            Entrance.Direction.Up => Entrance.Direction.Down,
            Entrance.Direction.Down => Entrance.Direction.Up,
            Entrance.Direction.Left => Entrance.Direction.Right,
            Entrance.Direction.Right => Entrance.Direction.Left,
            _ => throw new System.Exception("Invalid direction")
        };

    Vector2Int DirectionToVector(Entrance.Direction dir)
        => dir == Entrance.Direction.Up ? Vector2Int.up :
           dir == Entrance.Direction.Down ? Vector2Int.down :
           dir == Entrance.Direction.Left ? Vector2Int.left :
           Vector2Int.right;

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void ConnectEntrances()
    {
        foreach (var room in allRooms)
        {
            foreach (var dir in directions)
            {
                var entrance = room.GetEntrance(dir);
                Vector2Int neighborPos = room.gridPos + DirectionToVector(dir);
                if (!occupiedCells.ContainsKey(neighborPos))
                {
                    entrance?.Close();
                }
                else
                {
                    Room neighbor = occupiedCells[neighborPos];
                    var neighborEntrance = neighbor.GetEntrance(OppositeDirection(dir));
                    entrance?.Open();
                    neighborEntrance?.Open();
                }
            }
        }
    }

    void SpawnEntities()
    {
        Room startRoom = allRooms[0];
        PlayerHealth existingPlayer = Object.FindAnyObjectByType<PlayerHealth>();

        if (existingPlayer != null && startRoom.playerSpawn != null)
            existingPlayer.transform.position = startRoom.playerSpawn.position;
        else if (playerPrefab != null && startRoom.playerSpawn != null)
            Instantiate(playerPrefab, startRoom.playerSpawn.position, Quaternion.identity);

        foreach (var room in allRooms)
        {
            if (enemyPrefab != null && room.enemySpawns != null)
            {
                foreach (var point in room.enemySpawns)
                {
                    if (point != null)
                        Instantiate(enemyPrefab, point.position, Quaternion.identity);
                }
            }
        }
    }
}